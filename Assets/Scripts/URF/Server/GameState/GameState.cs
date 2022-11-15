using System.Collections.Generic;
using URF.Common;
using URF.Common.Entities;
using URF.Common.Persistence;
using URF.Server.RulesSystems;

namespace URF.Server.GameState {
  public class GameState : IGameState {

    public int MapWidth { get; }

    public int MapHeight { get; }

    // todo decide on a pattern here
    // ^ a) objects can access the game state's plugins directly
    // or b) only through game state method calls
    private bool _inGameUpdateLoop;
    
    private IEntity _mainCharacter;

    private readonly Cell[][] _map;

    private readonly List<IEntity> _entities;

    private readonly Dictionary<int, IEntity> _entitiesById;

    // Contains references to entities that need to be cleaned up after the game loop.
    private readonly List<IEntity> _killList;

    private readonly IEntityFactory _entityFactory;

    public GameState(
      int mapWidth,
      int mapHeight,
      IEntityFactory entityFactory
    ) {
      MapWidth = mapWidth;
      MapHeight = mapHeight;
      _entityFactory = entityFactory;

      _entities = new();
      _killList = new();
      _entitiesById = new();

      _map = new Cell[MapWidth][];
      for(int i = 0; i < MapWidth; i++) {
        _map[i] = new Cell[MapHeight];
        for(int j = 0; j < MapHeight; j++) { _map[i][j] = new Cell(); }
      }
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
    }

    public Cell GetCell(Position p) {
      (int x, int y) = p;
      return _map[x][y];
    }

    public void BeginUpdate() {
      _inGameUpdateLoop = true;
    }

    public void FinishUpdate() {
      _inGameUpdateLoop = false;
      foreach(IEntity entity in _killList) {
        KillImmediately(entity);
      }
    }

    private bool IsLegalMove(Position position) {
      return IsInBounds(position) && GetCell(position).IsPassable;
    }

    private bool IsInBounds(Position p) {
      return (0 <= p.X && p.X < MapWidth && 0 <= p.Y && p.Y < MapHeight);
    }
    
    private void PlaceEntity(int id, Position p) {
      IEntity entity = _entitiesById[id];
      Cell destination = GetCell(p);
      entity.GetComponent<Movement>().EntityPosition = p;
      destination.PutContents(entity);
    }

    public void MoveEntity(int id, Position position) {
      // todo would be nice if Entities didn't even need to know where they were
      if(!IsLegalMove(position)) {
        return;
      }

      IEntity entity = _entitiesById[id];
      Movement movement = entity.GetComponent<Movement>();
      Position oldPos = movement.EntityPosition;
      Cell origin = GetCell(oldPos);
      
      origin.RemoveEntity(entity);
      PlaceEntity(id, position);
    }

    public IEntity GetMainCharacter() {
      return _mainCharacter;
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

    public (int, int) GetMapSize() {
      return (MapWidth, MapHeight);
    }

    public bool IsTraversable(Position p) {
      return GetCell(p).IsPassable;
    }
    
    public void SetMainCharacter(int id) {
      IEntity entity = _entitiesById[id];
      _mainCharacter = entity;
    }

    public void CreateEntityAtPosition(IEntity entity, Position position) {
      _entities.Add(entity);
      _entitiesById.Add(entity.ID, entity);
      PlaceEntity(entity.ID, position);
    }

  }
}
