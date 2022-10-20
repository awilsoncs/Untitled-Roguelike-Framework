using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent the logical space of the game.
/// </summary>
[DisallowMultipleComponent]
public class BoardController : PersistableObject, IBoardController
{
    const int saveVersion = 1;
    List<IEntity> entities;
    Dictionary<int, IEntity> entities_by_id;
    // Contains references to entities that need to be cleaned up after the game loop.
    List<IEntity> killList;

    [SerializeField] Camera mainCamera;
    [SerializeField] KeyCode createKey = KeyCode.C;
    [SerializeField] KeyCode newGameKey = KeyCode.N;
    [SerializeField] KeyCode saveKey = KeyCode.S;
    [SerializeField] KeyCode loadKey = KeyCode.L;
    [SerializeField] KeyCode upKey = KeyCode.UpArrow;
    [SerializeField] KeyCode downKey = KeyCode.DownArrow;
    [SerializeField] KeyCode leftKey = KeyCode.LeftArrow;
    [SerializeField] KeyCode rightKey = KeyCode.RightArrow;
    [SerializeField] PersistentStorage storage;
    [SerializeField] public EntityFactory entityFactory;
    // map bounds
    [SerializeField] int mapWidth = 40;
    [SerializeField] int mapHeight = 20;
    public int MapWidth {
        get {
            return mapWidth;
        }
        set {
            this.mapWidth = value;
        }
    }
    public int MapHeight {
        get {
            return mapHeight;
        }
        set {
            this.mapHeight = value;
        }
    }
    Cell[][] map;
    const float GRID_MULTIPLE = 0.5f;
    Random.State mainRandomState;
    bool inGameUpdateLoop;
    bool isViewDirty;
    
    private void Start() {
        mainRandomState = Random.state;
        entities = new List<IEntity>();
        killList = new List<IEntity>();
        entities_by_id = new Dictionary<int, IEntity>();

        map = new Cell[mapWidth][];
        for (int i = 0; i < mapWidth; i++) {
            map[i] = new Cell[mapHeight];
            for (int j = 0; j < mapHeight; j++) {
                map[i][j] = new Cell(i, j);
            }
        }
        mainCamera.transform.position = new Vector3(mapWidth / (2 / GRID_MULTIPLE), mapHeight / (2 / GRID_MULTIPLE), -10);
        BeginNewGame();
    }

    private void Update() {
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
        RecalculateFOV();
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
    }

    private void BeginNewGame() {
        // Set up the new game seed
        Random.state = mainRandomState;
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
        mainRandomState = Random.state;
        Random.InitState(seed);
        DungeonBuilder.Build(this);
        RecalculateFOVImmediately();
    }

    private void ClearGame() {
        // Clean up the existing scene objects
        for (int i = 0; i < entities.Count; i++) {
            entities[i].Recycle(entityFactory);
        }

        entities.Clear();
        entities_by_id.Clear();
        for (int i = 0; i < mapWidth; i++) {
            for (int j = 0; j < mapHeight; j++) {
                map[i][j].ClearContents();
            }
        }
        killList.Clear();
    }

    /// <summary>
    /// Create an entity at a given location using the factory blueprint name.
    /// </summary>
    /// <param name="blueprintName">The name of the Entity type in the factory</param>
    /// <param name="x">The horizontal coordinate at which to create the Entity</param>
    /// <param name="y">The vertical coordinate at which to create the Entity</param>
    /// <returns>A reference to the created Entity</returns>
    public Entity CreateEntityAtLocation(System.String blueprintName, int x, int y) {
        // todo abstract entities with no location
        var entity = entityFactory.Get(blueprintName);
        entities.Add(entity);
        entities_by_id.Add(entity.ID, entity);
        PlacePawn(entity.ID, x, y);
        return entity;
    }

    /// <summary>
    /// Return whether a position in game coordinates is in bounds.
    /// </summary>
    /// <param name="x">The horizontal coordinate to check</param>
    /// <param name="y">The vertical position to check</param>
    /// <returns>True if the position is in bounds, False otherwise</returns>
    private bool IsInBounds(int x, int y) {
        return (
            0 <= x && x < mapWidth
            && 0 <= y && y < mapHeight
        );
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
    /// Provide a mapping from game coordinates to the map tiles.
    /// </summary>
    /// <param name="x">Horizontal coordinate in logical game coordinates.</param>
    /// <param name="y">Vertical coordinate in logical game coordinates.</param>
    /// <returns>Return the Cell at location (x, y)</returns>
    private Cell GetCell(int x, int y) {
        return map[x][y];
    }

    /// <summary>
    /// Places an Entity on the board.
    /// </summary>
    /// <param name="id">ID of the entity to move</param>
    /// <param name="x">Horizontal destination coordinate of the Entity</param>
    /// <param name="y">Vertical destination coordinate of the Entity</param>
    public void PlacePawn(int id, int x, int y) {
        Debug.Log($"Moving pawn {id} to ({x},{y})");
        IEntity entity = entities_by_id[id];
        Cell destination = GetCell(x, y);
        IEntity contentsToKill = destination.ClearContents();
        if (contentsToKill != null) {
            Kill(contentsToKill);
        }
        entity.X = x;
        entity.Y = y;
        destination.PutContents(entity);
        entity.SetSpritePosition(x*GRID_MULTIPLE, y*GRID_MULTIPLE);
    }

    /// <summary>
    /// Moves an Entity from one place on the Board to another.
    /// </summary>
    /// <param name="id">ID of the entity to move</param>
    /// <param name="x0">Original Horizontal coordinate of the Entity</param>
    /// <param name="y0">Original Vertical coordinate of the Entity</param>
    /// <param name="x1">Horizontal destination coordinate of the Entity</param>
    /// <param name="y1">Vertical destination coordinate of the Entity</param>
    public void MovePawn(int id, int x0, int y0, int x1, int y1) {
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
            Debug.LogError("Attempt to move mismatched pawn id.");
            return;
        }

        if (origin.GetContents() == destination.GetContents()) {
            return;
        }

        IEntity entity = origin.ClearContents();
        PlacePawn(id, x1, y1);
    }

    /// <summary>
    /// Return a string of the user action. Some actions are intercepted by the
    ///  BoardController and instead return none.
    /// </summary>
    /// <returns>A string descriptor of the user's action.</returns>
    public System.String GetUserInputAction() {
        if (Input.GetKeyDown(leftKey)) return "left";
        else if (Input.GetKeyDown(rightKey)) return "right";
        else if (Input.GetKeyDown(upKey)) return "up";
        else if (Input.GetKeyDown(downKey)) return "down";
        else if (Input.GetKeyDown(saveKey)) {
            Debug.Log("Player asked for save...");
            storage.Save(this, saveVersion);
        }
        else if (Input.GetKeyDown(loadKey)) {
            Debug.Log("Player asked for load...");
            storage.Load(this);
        }
        else if (Input.GetKeyDown(newGameKey)) {
            Debug.Log("Player asked for a reload...");
            ClearGame();
            BeginNewGame();
        }
        else if (Input.GetKeyDown(createKey)) return "spawn";
        return "none";
    }

    /// <summary>
    /// Call to schedule the FOV to be recalculated at the end of this game
    /// loop.
    /// </summary>
    public void  RecalculateFOV() {
        // recalculate will not be forced until 
        if (inGameUpdateLoop) {
            // handle all updates at once after the game update loop
            isViewDirty = true;
            return;
        }
        if (!isViewDirty) {
            return;
        }
        RecalculateFOVImmediately();
    }

    private void RecalculateFOVImmediately() {
        Debug.Log("Recalculating FOV!");
        isViewDirty = false;
    }

    public override void Save(GameDataWriter writer) {
        writer.Write(entities.Count);
        writer.Write(Random.state);
        for (int i = 0; i < entities.Count; i++) {
            writer.Write(entities[i].ID);
            writer.Write(entities[i].SpriteIndex);
            entities[i].Save(writer);
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <param name="reader"></param>
    public override void Load (GameDataReader reader) {
        int version = reader.Version;
        if (version > saveVersion) {
			Debug.LogError("Unsupported future save version " + version);
			return;
		}
        ClearGame();
        LoadGame(reader);
        RecalculateFOVImmediately();
    }

    void LoadGame (GameDataReader reader) {
        // Perform all logic related to loading the game into the BoardController.
        int version = reader.Version;
        Debug.Log($"Loader version {version}");
        int count = reader.ReadInt();
        Debug.Log($"Object count {count}");
        Random.State state = reader.ReadRandomState();

        Debug.Log($"Loading objects...");
        for (int i = 0; i < count; i++) {
            Debug.Log($">> Loading object {i}");

            var entityID = reader.ReadInt();
            var entity = entityFactory.Get();
            // todo it may be wise to eventually move the sprite off of the entity entirely
            var spriteIndex = reader.ReadInt();
            entity.SetSprite(entityFactory.GetSpriteByIndex(spriteIndex));
            entity.SpriteIndex = spriteIndex;

            entity.ID = entityID;
            entity.Load(reader);

            entities.Add(entity);
            entities_by_id.Add(entity.ID, entity);
            PlacePawn(entity.ID, entity.X, entity.Y);
            Debug.Log($"<< Loaded object {i}");
        }
        Debug.Log("Done loading objects.");
    }
}