using System;
using System.Collections.Generic;
using URFCommon;
using System.Linq;

public partial class GameState : IGameState {
    public int MapWidth {get; set;}
    public int MapHeight {get; set;}
    private bool inGameUpdateLoop;
    private bool isFieldOfViewDirty;
    IEntity mainCharacter;

    // todo decide on a pattern here
    // ^ a) objects can access the game state's plugins directly
    // or b) only through game state method calls
    public IRandomGenerator RNG {get;}
    public IFieldOfView FieldOfView {get;}
    public IPathfinding Pathfinding {get;}
    private readonly IGameClient gameClient;
    private readonly IEntityFactory entityFactory;
    private readonly ILogging logging;

    private readonly Cell[][] map;
    private readonly List<IEntity> entities;
    private readonly Dictionary<int, IEntity> entitiesById;
    // Contains references to entities that need to be cleaned up after the game loop.
    private readonly List<IEntity> killList;

    /// <summary>
    /// Create a new GameState.
    /// </summary>
    /// <param name="client">Client communication port</param>
    /// <param name="random">Random number plugin</param>
    /// todo
    /// <param name="mapWidth"></param>
    /// <param name="mapHeight"></param>
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
        if (random == null) {
            PostError("GameState random plugin is null!");
        } else if (fieldOfView == null) {
            PostError("Field of View plugin is null");
        }
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        entities = new();
        killList = new();
        entitiesById = new();

        // perform injections
        RNG = random;
        this.entityFactory = entityFactory;
        gameClient = client;
        this.FieldOfView = fieldOfView;
        this.logging = logging;
        this.Pathfinding = pathfinding;

        // Set up the rules systems
        RulesSystems = new();
        eventHandlers = new();
        foreach (GameEventType t in Enum.GetValues(typeof(GameEventType))) {
            eventHandlers[t] = new();
        }

        RegisterSystem(new DebugSystem());
        RegisterSystem(new EntityInfoSystem());
        RegisterSystem(new MovementSystem());
        RegisterSystem(new GameStartSystem());
        RegisterSystem(new CombatSystem());
        RegisterSystem(new IntelligenceSystem());

        map = new Cell[MapWidth][];
        for (int i = 0; i < MapWidth; i++) {
            map[i] = new Cell[MapHeight];
            for (int j = 0; j < MapHeight; j++) {
                map[i][j] = new Cell();
            }
        }
    }

    public void GameUpdate() {
        if (isFieldOfViewDirty) {
            RecalculateFOV();
        }

        // todo refactor this to read from two queues:
            // 1st, the backend command queue
            // 2nd, when the 1st is empty, read from the receiving command queue
        inGameUpdateLoop = true;
        for (int i = 0; i < RulesSystems.Count; i++) {
            RulesSystems[i].GameUpdate(this);
        }
        inGameUpdateLoop = false;
        if (killList.Count > 0) {
            for (int i = 0; i < killList.Count; i++) {
                KillImmediately(killList[i]);
            }
            killList.Clear();
        }
    }

    public void Kill (IEntity entity) {
        // Eventually, perform actions related to clearing the BoardController
        // of the entity and recycle the entity
        if (inGameUpdateLoop) {
            // If we're in the game loop, we need to wait so that we don't
            // compromise the iteration. Note that this can mean that Entities
            // continue to exist until the end of the frame.
            killList.Add(entity);
        } else {
            // In this case, we're not in the game loop- it's safe to kill
            // the Entity immediately.
            KillImmediately(entity);
        }
    }

    private void KillImmediately (IEntity entity) {
        // Generally, should only be called from Kill and during Update cleanup.
        // swap and pop the desired entity to avoid rearranging the whole tail
        PostEvent(new EntityKilledEvent(entity));

        // todo can remove this scan by maintaining the index on the entity
        int index = entities.FindIndex(x => x == entity);
        int lastIndex = entities.Count - 1;
        if (index < lastIndex) {
            entities[index] = entities[lastIndex];
        }
        entities.RemoveAt(lastIndex);
        entitiesById.Remove(entity.ID);
        var pos = entity.GetComponent<Movement>().Position;
        Cell possibleLocation = GetCell(pos);
        if (possibleLocation.Contents.Contains(entity)) {
            possibleLocation.ClearContents();
        } else {
            // todo fix this flow?
            // this branch can be hit when killing an entity by stepping on it:
            // The entity will be removed by the cell update, but won't be
            // killed until the end of the game loop. While this isn't a 
            // problem in this case, it does reveal a flaw in the kill cycle.
            // Some entity information is removed early, which could lead
            // to problems.
        }
        entity.Recycle(entityFactory);

    }

    public Cell GetCell(Position p) {
        return map[p.X][p.Y];
    }

    /// <summary>
    /// Return whether this tile can legally be stepped into.
    /// </summary>
    /// <param name="x">The horizontal coordinate to check</param>
    /// <param name="y">The vertical position to check</param>
    /// <returns>True if the position is legal to step to, False otherwise</returns>
    private bool IsLegalMove(Position p) {
        return IsInBounds(p) && GetCell(p).IsPassable;
    }

    /// <summary>
    /// Return whether a position in game coordinates is in bounds.
    /// </summary>
    /// <param name="x">The horizontal coordinate to check</param>
    /// <param name="y">The vertical position to check</param>
    /// <returns>True if the position is in bounds, False otherwise</returns>
    private bool IsInBounds(Position p) {
        return (
            0 <= p.X && p.X < MapWidth
            && 0 <= p.Y && p.Y < MapHeight
        );
    }

    /// <summary>
    /// Places an Entity on the board.
    /// </summary>
    /// <param name="id">ID of the entity to move</param>
    /// <param name="x">Horizontal destination coordinate of the Entity</param>
    /// <param name="y">Vertical destination coordinate of the Entity</param>
    private void PlaceEntity(int id, Position p) {
        IEntity entity = entitiesById[id];
        Cell destination = GetCell(p);
        entity.GetComponent<Movement>().Position = p;
        destination.PutContents(entity);
        gameClient.PostEvent(new EntityMovedEvent(entity, p));
    }

    /// <summary>
    /// Moves an Entity from one place on the Board to another.
    /// </summary>
    /// <param name="id">ID of the entity to move</param>
    /// <param name="x1">New horizontal destination coordinate of the Entity</param>
    /// <param name="y1">New vertical destination coordinate of the Entity</param>
    public void MoveEntity(int id, Position newPos) {
        // todo would be nice if Entities didn't even need to know where they were
        if (!IsLegalMove(newPos)) {
            // This move isn't legal.
            PostError($"{entitiesById[id]} attempted illegal move...");
            return;
        }

        IEntity entity = entitiesById[id];
        var movement = entity.GetComponent<Movement>();
        var oldPos = movement.Position;
        Cell origin = GetCell(oldPos);
        Cell destination = GetCell(newPos);

        if (!origin.Contents.Contains(entitiesById[id])) {
            // Defensive coding, we shouldn't do anything if the IDs don't match.
            PostError($"Attempted to move wrong entity {id} vs {origin.Contents}");
            return;
        }

        if (origin == destination) {
            PostError("Attempted no-op move...");
            return;
        }

        // todo this isn't REALLY necessary. One alternative would be to store 
        // the info in cells and update the entity that moves into the cell
        // FOV should only CHANGE when the player moves.
        isFieldOfViewDirty = true;
        origin.RemoveEntity(entity);
        PlaceEntity(id, newPos);
    }

    public IEntity GetMainCharacter() {
        return mainCharacter;
    }

    /// <summary>
    /// Call to schedule the FOV to be recalculated at the end of this game
    /// loop.
    /// </summary>
    public void  RecalculateFOV() {
        // recalculate will not be forced until 
        if (inGameUpdateLoop) {
            // handle all updates at once after the game update loop
            isFieldOfViewDirty = true;
            return;
        }
        if (!isFieldOfViewDirty) {
            return;
        }
        RecalculateFOVImmediately();
    }

    public void RecalculateFOVImmediately() {
        isFieldOfViewDirty = false;
        var movement = mainCharacter.GetComponent<Movement>();
        var pos = movement.Position;

        var result = FieldOfView.CalculateFOV(this, pos);
        // todo would be nice to have a map iterator
        for(int x = 0; x < MapWidth; x++) {
            for (int y = 0; y < MapHeight; y++) {
                bool isVisible = result.IsVisible((x, y));
                var cell = map[x][y];
                for (int c = 0; c < cell.Contents.Count; c++) {
                    var entity = cell.Contents[c];
                    if (entity.IsVisible != isVisible) {
                        entity.IsVisible = isVisible;
                        PostEvent(new EntityVisibilityChangedEvent(entity, entity.IsVisible));
                    }
                }
            }
        }
    }

    public IEntity GetEntityById(int id) {
        return entitiesById[id];
    }

    public List<IEntity> GetEntities() {
        return entities;
    }

    public bool EntityExists(int id) {
        return entitiesById.ContainsKey(id);
    }

    public void PostEvent(IGameEvent ev) {
        if (!ev.IsCommand) {
            // commands are kept internal
            gameClient.PostEvent(ev);
        }
        foreach(var handler in eventHandlers[ev.EventType]) {
            handler(this, ev);
        }
    }

    public void PostError(string message) {
        logging.Log(message);
        PostEvent(new GameErrorEvent(message));
    }

    public void Log(string message) {
        if (logging != null) {
            logging.Log(message);
        }
    }

    public void Save (GameDataWriter writer) {
        writer.Write(entities.Count);
        for (int i = 0; i < entities.Count; i++) {
            writer.Write(entities[i].ID);
            entities[i].Save(writer);
        }
        writer.Write(mainCharacter.ID);
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <param name="reader"></param>
    public void Load (GameDataReader reader) {
        LoadGame(reader);
        RecalculateFOVImmediately();
    }

    void LoadGame (GameDataReader reader) {
        // Perform all logic related to loading the game into the BoardController.
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++) {
            var entityID = reader.ReadInt();
            var entity = entityFactory.Get();
            entity.ID = entityID;
            entity.GameState = this;
            entity.Load(reader);

            entities.Add(entity);
            entitiesById.Add(entity.ID, entity);
            gameClient.PostEvent(new EntityCreatedEvent(entity));
            var pos = entity.GetComponent<Movement>().Position;
            PlaceEntity(entity.ID, pos);
        }
        mainCharacter = entitiesById[reader.ReadInt()];
        PostEvent(new MainCharacterChangedEvent(mainCharacter));
    }

    public (int, int) GetMapSize() {
        return (MapWidth, MapHeight);
    }

    public bool IsTraversable(Position p) {
        return GetCell(p).IsPassable;
    }
}