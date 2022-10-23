using System;
using System.Collections.Generic;

public partial class GameState : IGameState {
    List<IEntity> entities;
    Dictionary<int, IEntity> entitiesById;
    // Contains references to entities that need to be cleaned up after the game loop.
    List<IEntity> killList;
    public int MapWidth {get; set;}
    public int MapHeight {get; set;}
    Cell[][] map;
    bool inGameUpdateLoop;
    bool isFieldOfViewDirty;
    // The character controlled by player input.
    Entity playerAgent;
    IEntity mainCharacter;
    // Interface through which client updates are posted.
    IGameClient gameClient;
    public IRandomGenerator RNG {get;}
    IEntityFactory entityFactory;
    IFieldOfView fieldOfView;


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
        entities = new List<IEntity>();
        killList = new List<IEntity>();
        entitiesById = new Dictionary<int, IEntity>();

        // perform injections
        RNG = random;
        this.entityFactory = entityFactory;
        gameClient = client;
        this.fieldOfView = fieldOfView;

        map = new Cell[MapWidth][];
        for (int i = 0; i < MapWidth; i++) {
            map[i] = new Cell[MapHeight];
            for (int j = 0; j < MapHeight; j++) {
                map[i][j] = new Cell();
            }
        }
    }

    private void GameUpdate() {
        if (isFieldOfViewDirty) {
            RecalculateFOV();
        }
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
        entitiesById.Remove(entity.ID);
        Cell possibleLocation = GetCell(entity.X, entity.Y);
        if (possibleLocation.GetContents() == entity) {
            possibleLocation.ClearContents();
        }
        entity.Recycle(entityFactory);

        // todo notify the GameView that this entity has been killed
        // todo update pawns in GameView
    }

    public Cell GetCell(int x, int y) {
        return map[x][y];
    }

    /// <summary>
    /// Return whether this tile can legally be stepped into.
    /// </summary>
    /// <param name="x">The horizontal coordinate to check</param>
    /// <param name="y">The vertical position to check</param>
    /// <returns>True if the position is legal to step to, False otherwise</returns>
    private bool IsLegalMove(int x, int y) {
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
    private void PlaceEntity(int id, int x, int y) {
        IEntity entity = entitiesById[id];
        Cell destination = GetCell(x, y);
        IEntity contentsToKill = destination.ClearContents();
        if (contentsToKill != null) {
            gameClient.PostEvent(new EntityKilledEvent(contentsToKill.ID));
            Kill(contentsToKill);
        }
        entity.X = x;
        entity.Y = y;
        destination.PutContents(entity);
        gameClient.PostEvent(new EntityMovedEvent(id, x, y));
    }

    /// <summary>
    /// Moves an Entity from one place on the Board to another.
    /// </summary>
    /// <param name="id">ID of the entity to move</param>
    /// <param name="x1">New horizontal destination coordinate of the Entity</param>
    /// <param name="y1">New vertical destination coordinate of the Entity</param>
    private void MoveEntity(int id, int x, int y) {
        // todo would be nice if Entities didn't even need to know where they were
        if (!IsLegalMove(x, y)) {
            // This move isn't legal.
            PostError("Attempted illegal move...");
            return;
        }

        IEntity entity = entitiesById[id];
        Cell origin = GetCell(entity.X, entity.Y);
        Cell destination = GetCell(x, y);

        if (origin.GetContents() != entitiesById[id]) {
            // Defensive coding, we shouldn't do anything if the IDs don't match.
            PostError($"Attempted to move wrong entity {id} vs {origin.GetContents()}");
            return;
        }

        if (origin.GetContents() == destination.GetContents()) {
            PostError("Attempted no-op move...");
            return;
        }

        // todo this isn't REALLY necessary. One alternative would be to store 
        // the info in cells and update the entity that moves into the cell
        // FOV should only CHANGE when the player moves.
        isFieldOfViewDirty = true;
        origin.ClearContents();
        PlaceEntity(id, x, y);
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
        IFieldOfViewQueryResult result = fieldOfView.CalculateFOV(
            this, mainCharacter.X, mainCharacter.Y);
        for(int x = 0; x < MapWidth; x++) {
            for (int y = 0; y < MapHeight; y++) {
                bool isVisible = result.IsVisible(x, y);
                IEntity entity = GetCell(x, y).GetContents();
                if (entity != null && entity.IsVisible != isVisible) {
                    entity.IsVisible = isVisible;
                    PostEvent(new EntityVisibilityChangedEvent(entity.ID, entity.IsVisible));
                }
            }
        }
        PostEvent(new FieldOfViewUpdatedEvent());
    }

    private void PostEvent(IGameEvent ev) {
        gameClient.PostEvent(ev);
    }

    private void PostError(string message) {
        PostEvent(new GameErrorEvent(message));
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
        int version = reader.Version;
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++) {
            var entityID = reader.ReadInt();
            var entity = entityFactory.Get();
            entity.ID = entityID;
            entity.Load(reader);

            entities.Add(entity);
            entitiesById.Add(entity.ID, entity);
            gameClient.PostEvent(new EntityCreatedEvent(entity));
            PlaceEntity(entity.ID, entity.X, entity.Y);
        }
        mainCharacter = entitiesById[reader.ReadInt()];
    }
}