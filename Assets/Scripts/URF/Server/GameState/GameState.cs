using System;
using System.Collections.Generic;
using URF.Common;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Common.Logging;
using URF.Server.FieldOfView;
using URF.Server.Pathfinding;
using URF.Server.RandomGeneration;
using URF.Server.RulesSystems;
using EventHandler = URF.Server.RulesSystems.EventHandler;

namespace URF.Server.GameState {
  public partial class GameState : IGameState {

    public int MapWidth { get; }

    public int MapHeight { get; }

    // todo decide on a pattern here
    // ^ a) objects can access the game state's plugins directly
    // or b) only through game state method calls
    public IRandomGenerator Random { get; }

    public IFieldOfView FieldOfView { get; }

    public IPathfinding Pathfinding { get; }

    private bool _inGameUpdateLoop;

    private bool _isFieldOfViewDirty;

    private IEntity _mainCharacter;

    private readonly IGameClient _gameClient;

    private readonly IEntityFactory _entityFactory;

    private readonly ILogging _logging;

    private readonly Cell[][] _map;

    private readonly List<IEntity> _entities;

    private readonly Dictionary<int, IEntity> _entitiesById;

    // Contains references to entities that need to be cleaned up after the game loop.
    private readonly List<IEntity> _killList;

    public GameState(
      IGameClient client,
      IRandomGenerator random,
      IEntityFactory entityFactory,
      IFieldOfView fieldOfView,
      IPathfinding pathfinding,
      ILogging logging,
      int mapWidth,
      int mapHeight
    ) {
      if(random == null) { PostError("GameState random plugin is null!"); } else if(
        fieldOfView == null) { PostError("Field of View plugin is null"); }

      MapWidth = mapWidth;
      MapHeight = mapHeight;
      _entities = new();
      _killList = new();
      _entitiesById = new();

      // perform injections
      Random = random;
      _entityFactory = entityFactory;
      _gameClient = client;
      FieldOfView = fieldOfView;
      _logging = logging;
      Pathfinding = pathfinding;

      // Set up the rules systems
      foreach(GameEventType t in Enum.GetValues(typeof(GameEventType))) {
        _eventHandlers[t] = new List<EventHandler>();
      }

      RegisterSystem(new DebugSystem());
      RegisterSystem(new EntityInfoSystem());
      RegisterSystem(new MovementSystem());
      RegisterSystem(new GameStartSystem());
      RegisterSystem(new CombatSystem());
      RegisterSystem(new IntelligenceSystem());

      _map = new Cell[MapWidth][];
      for(int i = 0; i < MapWidth; i++) {
        _map[i] = new Cell[MapHeight];
        for(int j = 0; j < MapHeight; j++) { _map[i][j] = new Cell(); }
      }
    }

    public void GameUpdate() {
      if(_isFieldOfViewDirty) { RecalculateFOV(); }

      // todo refactor this to read from two queues:
      // 1st, the backend command queue
      // 2nd, when the 1st is empty, read from the receiving command queue
      _inGameUpdateLoop = true;
      foreach(IRulesSystem t in _rulesSystems) { t.GameUpdate(this); }

      _inGameUpdateLoop = false;
      if(_killList.Count <= 0) return;
      foreach(IEntity t in _killList) { KillImmediately(t); }
      _killList.Clear();
    }

    public void Kill(IEntity entity) {
      // Eventually, perform actions related to clearing the BoardController
      // of the entity and recycle the entity
      if(_inGameUpdateLoop) {
        // If we're in the game loop, we need to wait so that we don't
        // compromise the iteration. Note that this can mean that Entities
        // continue to exist until the end of the frame.
        _killList.Add(entity);
      } else {
        // In this case, we're not in the game loop- it's safe to kill
        // the Entity immediately.
        KillImmediately(entity);
      }
    }

    private void KillImmediately(IEntity entity) {
      // Generally, should only be called from Kill and during Update cleanup.
      // swap and pop the desired entity to avoid rearranging the whole tail
      PostEvent(new EntityKilledEvent(entity));

      // todo can remove this scan by maintaining the index on the entity
      int index = _entities.FindIndex(x => x == entity);
      int lastIndex = _entities.Count - 1;
      if(index < lastIndex) { _entities[index] = _entities[lastIndex]; }

      _entities.RemoveAt(lastIndex);
      _entitiesById.Remove(entity.ID);
      Position pos = entity.GetComponent<Movement>().EntityPosition;
      Cell possibleLocation = GetCell(pos);
      if(possibleLocation.Contents.Contains(entity)) {
        possibleLocation.RemoveEntity(entity);
      } else {
        // todo fix this flow?
        // this branch can be hit when killing an entity by stepping on it:
        // The entity will be removed by the cell update, but won't be
        // killed until the end of the game loop. While this isn't a 
        // problem in this case, it does reveal a flaw in the kill cycle.
        // Some entity information is removed early, which could lead
        // to problems.
      }

      entity.Recycle(_entityFactory);
    }

    public Cell GetCell(Position position) {
      (int x, int y) = position;
      return _map[x][y];
    }

    private bool IsLegalMove(Position position) {
      return IsInBounds(position) && GetCell(position).IsPassable;
    }

    private bool IsInBounds(Position p) {
      return (0 <= p.X && p.X < MapWidth && 0 <= p.Y && p.Y < MapHeight);
    }

    /// <summary>
    /// Places an Entity on the board.
    /// </summary>
    /// <param name="id">ID of the entity to move</param>
    /// <param name="x">Horizontal destination coordinate of the Entity</param>
    /// <param name="y">Vertical destination coordinate of the Entity</param>
    private void PlaceEntity(int id, Position p) {
      IEntity entity = _entitiesById[id];
      Cell destination = GetCell(p);
      entity.GetComponent<Movement>().EntityPosition = p;
      destination.PutContents(entity);
      _gameClient.PostEvent(new EntityMovedEvent(entity, p));
    }

    /// <summary>
    /// Moves an Entity from one place on the Board to another.
    /// </summary>
    /// <param name="id">ID of the entity to move</param>
    /// <param name="x1">New horizontal destination coordinate of the Entity</param>
    /// <param name="y1">New vertical destination coordinate of the Entity</param>
    public void MoveEntity(int id, Position newPos) {
      // todo would be nice if Entities didn't even need to know where they were
      if(!IsLegalMove(newPos)) {
        // This move isn't legal.
        PostError($"{_entitiesById[id]} attempted illegal move...");
        return;
      }

      IEntity entity = _entitiesById[id];
      var movement = entity.GetComponent<Movement>();
      var oldPos = movement.EntityPosition;
      Cell origin = GetCell(oldPos);
      Cell destination = GetCell(newPos);

      if(!origin.Contents.Contains(_entitiesById[id])) {
        // Defensive coding, we shouldn't do anything if the IDs don't match.
        PostError($"Attempted to move wrong entity {id} vs {origin.Contents}");
        return;
      }

      if(origin == destination) {
        PostError("Attempted no-op move...");
        return;
      }

      // todo this isn't REALLY necessary. One alternative would be to store 
      // the info in cells and update the entity that moves into the cell
      // FOV should only CHANGE when the player moves.
      _isFieldOfViewDirty = true;
      origin.RemoveEntity(entity);
      PlaceEntity(id, newPos);
    }

    public IEntity GetMainCharacter() {
      return _mainCharacter;
    }

    /// <summary>
    /// Call to schedule the FOV to be recalculated at the end of this game
    /// loop.
    /// </summary>
    public void RecalculateFOV() {
      // recalculate will not be forced until 
      if(_inGameUpdateLoop) {
        // handle all updates at once after the game update loop
        _isFieldOfViewDirty = true;
        return;
      }

      if(!_isFieldOfViewDirty) { return; }

      RecalculateFOVImmediately();
    }

    public void RecalculateFOVImmediately() {
      _isFieldOfViewDirty = false;
      var movement = _mainCharacter.GetComponent<Movement>();
      var pos = movement.EntityPosition;

      var result = FieldOfView.CalculateFOV(this, pos);
      // todo would be nice to have a map iterator
      for(int x = 0; x < MapWidth; x++) {
        for(int y = 0; y < MapHeight; y++) {
          bool isVisible = result.IsVisible((x, y));
          var cell = _map[x][y];
          foreach(var entity in cell.Contents) {
            if(entity.IsVisible != isVisible) {
              entity.IsVisible = isVisible;
              PostEvent(new EntityVisibilityChangedEvent(entity, entity.IsVisible));
            }
          }
        }
      }
    }

    public IEntity GetEntityById(int id) {
      return _entitiesById[id];
    }

    public List<IEntity> GetEntities() {
      return _entities;
    }

    public bool EntityExists(int id) {
      return _entitiesById.ContainsKey(id);
    }

    public void PostEvent(IGameEvent ev) {
      if(!ev.IsCommand) {
        // commands are kept internal
        _gameClient.PostEvent(ev);
      }

      foreach(var handler in _eventHandlers[ev.EventType]) { handler(this, ev); }
    }

    public void PostError(string message) {
      _logging.Log(message);
      PostEvent(new GameErrorEvent(message));
    }

    public void Log(string message) {
      if(_logging != null) { _logging.Log(message); }
    }

    public void Save(GameDataWriter writer) {
      writer.Write(_entities.Count);
      for(int i = 0; i < _entities.Count; i++) {
        writer.Write(_entities[i].ID);
        _entities[i].Save(writer);
      }

      writer.Write(_mainCharacter.ID);
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <param name="reader"></param>
    public void Load(GameDataReader reader) {
      LoadGame(reader);
      RecalculateFOVImmediately();
    }

    void LoadGame(GameDataReader reader) {
      // Perform all logic related to loading the game into the BoardController.
      int count = reader.ReadInt();
      for(int i = 0; i < count; i++) {
        var entityID = reader.ReadInt();
        var entity = _entityFactory.Get();
        entity.ID = entityID;
        entity.Load(reader);

        _entities.Add(entity);
        _entitiesById.Add(entity.ID, entity);
        _gameClient.PostEvent(new EntityCreatedEvent(entity));
        var pos = entity.GetComponent<Movement>().EntityPosition;
        PlaceEntity(entity.ID, pos);
      }

      _mainCharacter = _entitiesById[reader.ReadInt()];
      PostEvent(new MainCharacterChangedEvent(_mainCharacter));
    }

    public (int, int) GetMapSize() {
      return (MapWidth, MapHeight);
    }

    public bool IsTraversable(Position p) {
      return GetCell(p).IsPassable;
    }

  }
}
