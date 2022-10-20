using System;
using System.Collections.Generic;

public partial class GameState : IGameState {
    List<IEntity> entities;
    Dictionary<int, IEntity> entities_by_id;
    // Contains references to entities that need to be cleaned up after the game loop.
    List<IEntity> killList;
    EntityFactory entityFactory;
    public int MapWidth {get; set;}
    public int MapHeight {get; set;}
    Cell[][] map;
    bool inGameUpdateLoop;
    bool isFieldOfViewDirty;
    // The character controlled by player input.
    Entity playerAgent;
    // Interface through which client updates are posted.
    IGameClient gameClient;

    /// <summary>
    /// Create a new GameState.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="mapWidth"></param>
    /// <param name="mapHeight"></param>
    public GameState(IGameClient client, int mapWidth, int mapHeight) {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        entities = new List<IEntity>();
        killList = new List<IEntity>();
        entities_by_id = new Dictionary<int, IEntity>();

        map = new Cell[MapWidth][];
        for (int i = 0; i < MapWidth; i++) {
            map[i] = new Cell[MapHeight];
            for (int j = 0; j < MapHeight; j++) {
                map[i][j] = new Cell();
            }
        }
        gameClient = client;
        entityFactory = new EntityFactory();
    }

    private void GameUpdate() {
        inGameUpdateLoop = true;
        for (int i = 0; i < entities.Count; i++) {
            entities[i].GameUpdate(this);
        }
        inGameUpdateLoop = false;
        if (killList.Count > 0) {
            for (int i = 0; i < killList.Count; i++) {
                KillImmediately(killList[i]);
            }
            killList.Clear();
        }
    }

    private void Kill (IEntity entity) {
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
        // todo can remove this scan by maintaining the index on the entity
        int index = entities.FindIndex(x => x == entity);
        int lastIndex = entities.Count - 1;
        if (index < lastIndex) {
            entities[index] = entities[lastIndex];
        }
        entities.RemoveAt(lastIndex);
        entities_by_id.Remove(entity.ID);
        Cell possibleLocation = GetCell(entity.X, entity.Y);
        if (possibleLocation.GetContents() == entity) {
            possibleLocation.ClearContents();
        }
        entity.Recycle(entityFactory);

        // todo notify the GameView that this entity has been killed
        // todo update pawns in GameView
    }

    private Cell GetCell(int x, int y) {
        return map[x][y];
    }

        /// <summary>
    /// Create an entity at a given location using the factory blueprint name.
    /// </summary>
    /// <param name="blueprintName">The name of the Entity type in the factory</param>
    /// <param name="x">The horizontal coordinate at which to create the Entity</param>
    /// <param name="y">The vertical coordinate at which to create the Entity</param>
    /// <returns>A reference to the created Entity</returns>
    public IEntity CreateEntityAtLocation(String blueprintName, int x, int y) {
        // todo abstract entities with no location
        var entity = entityFactory.Get(blueprintName);
        entities.Add(entity);
        entities_by_id.Add(entity.ID, entity);
        gameClient.EntityCreated(entity.ID, blueprintName, x, y);
        return entity;
    }

    /// <summary>
    /// Return whether this tile can legally be stepped into.
    /// </summary>
    /// <param name="x">The horizontal coordinate to check</param>
    /// <param name="y">The vertical position to check</param>
    /// <returns>True if the position is legal to step to, False otherwise</returns>
    public bool IsLegalMove(int x, int y) {
        return IsInBounds(x, y) && GetCell(x, y).IsPassable();
    }

    /// <summary>
    /// Return whether a position in game coordinates is in bounds.
    /// </summary>
    /// <param name="x">The horizontal coordinate to check</param>
    /// <param name="y">The vertical position to check</param>
    /// <returns>True if the position is in bounds, False otherwise</returns>
    private bool IsInBounds(int x, int y) {
        return (
            0 <= x && x < MapWidth
            && 0 <= y && y < MapHeight
        );
    }

    /// <summary>
    /// Places an Entity on the board.
    /// </summary>
    /// <param name="id">ID of the entity to move</param>
    /// <param name="x">Horizontal destination coordinate of the Entity</param>
    /// <param name="y">Vertical destination coordinate of the Entity</param>
    public void PlaceEntity(int id, int x, int y) {
        IEntity entity = entities_by_id[id];
        Cell destination = GetCell(x, y);
        IEntity contentsToKill = destination.ClearContents();
        if (contentsToKill != null) {
            gameClient.EntityKilled(contentsToKill.ID);
            Kill(contentsToKill);
        }
        entity.X = x;
        entity.Y = y;
        destination.PutContents(entity);
        gameClient.EntityMoved(id, x, y);
    }

    /// <summary>
    /// Moves an Entity from one place on the Board to another.
    /// </summary>
    /// <param name="id">ID of the entity to move</param>
    /// <param name="x0">Original Horizontal coordinate of the Entity</param>
    /// <param name="y0">Original Vertical coordinate of the Entity</param>
    /// <param name="x1">Horizontal destination coordinate of the Entity</param>
    /// <param name="y1">Vertical destination coordinate of the Entity</param>
    public void MoveEntity(int id, int x0, int y0, int x1, int y1) {
        // todo could simplify this by tracking last position on the entity
        // todo would be nice if Entities didn't even need to know where they were
        if (!IsLegalMove(x1, y1)) {
            // This move isn't legal.
            return;
        }

        Cell origin = GetCell(x0, y0);
        Cell destination = GetCell(x1, y1);

        if (origin.GetContents() != entities_by_id[id]) {
            // Defensive coding, we shouldn't do anything if the IDs don't match.
            return;
        }

        if (origin.GetContents() == destination.GetContents()) {
            return;
        }

        IEntity entity = origin.ClearContents();
        PlaceEntity(id, x1, y1);
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

    private void RecalculateFOVImmediately() {
        isFieldOfViewDirty = false;
    }

    public void Save (GameDataWriter writer) {
        writer.Write(entities.Count);
        for (int i = 0; i < entities.Count; i++) {
            writer.Write(entities[i].ID);
            entities[i].Save(writer);
        }
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
        int version = reader.Version;
        // Debug.Log($"Loader version {version}");
        int count = reader.ReadInt();
        // Debug.Log($"Object count {count}");
        // Debug.Log($"Loading objects...");
        for (int i = 0; i < count; i++) {
            // Debug.Log($">> Loading object {i}");

            var entityID = reader.ReadInt();
            var entity = entityFactory.Get();
            entity.ID = entityID;
            entity.Load(reader);

            entities.Add(entity);
            entities_by_id.Add(entity.ID, entity);
            PlaceEntity(entity.ID, entity.X, entity.Y);
            // Debug.Log($"<< Loaded object {i}");
        }
        // Debug.Log("Done loading objects.");
    }
}