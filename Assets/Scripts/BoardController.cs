using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent the logical space of the game.
/// </summary>
[DisallowMultipleComponent]
public class BoardController : PersistableObject
{
    const int saveVersion = 1;
    List<Entity> entities;
    Dictionary<int, Entity> entities_by_id;
    // Contains references to entities that need to be cleaned up after the game loop.
    List<Entity> killList;

    [SerializeField] KeyCode createKey = KeyCode.C;
    [SerializeField] KeyCode newGameKey = KeyCode.N;
    [SerializeField] KeyCode saveKey = KeyCode.S;
    [SerializeField] KeyCode loadKey = KeyCode.L;
    [SerializeField] KeyCode upKey = KeyCode.UpArrow;
    [SerializeField] KeyCode downKey = KeyCode.DownArrow;
    [SerializeField] KeyCode leftKey = KeyCode.LeftArrow;
    [SerializeField] KeyCode rightKey = KeyCode.RightArrow;

    const float GRID_MULTIPLE = 0.5f;
    [SerializeField] PersistentStorage storage;
    [SerializeField] public EntityFactory entityFactory;
    // map bounds
    [SerializeField] int mapWidth = 10;
    [SerializeField] int mapHeight = 10;
    Cell[][] map;
    Random.State mainRandomState;
    bool inGameUpdateLoop;

    public static BoardController Instance { get; set; }

    void OnEnable () {
        Instance = this;
	}
    
    private void Start() {
        mainRandomState = Random.state;
        entities = new List<Entity>();
        killList = new List<Entity>();
        entities_by_id = new Dictionary<int, Entity>();

        map = new Cell[mapWidth][];
        for (int i = 0; i < mapWidth; i++) {
            map[i] = new Cell[mapHeight];
            for (int j = 0; j < mapHeight; j++) {
                map[i][j] = new Cell(i, j);
            }
        }

        BeginNewGame();
    }

    private void Update() {
        inGameUpdateLoop = true;
        for (int i = 0; i < entities.Count; i++) {
            entities[i].GameUpdate();
        }
        inGameUpdateLoop = false;
        if (killList.Count > 0) {
            for (int i = 0; i < killList.Count; i++) {
                KillImmediately(killList[i]);
            }
            killList.Clear();
        }
    }

    private void Kill (Entity entity) {
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

    private void KillImmediately (Entity entity) {
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
        entity.Recycle();
    }

    private void BeginNewGame() {
        // Set up the new game seed
        Random.state = mainRandomState;
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
        mainRandomState = Random.state;
        Random.InitState(seed);
        var player = CreateEntityAtLocation("player", 0, 0);
    }

    private void ClearGame() {
        // Clean up the existing scene objects
        for (int i = 0; i < entities.Count; i++) {
            entities[i].Recycle();
        }

        entities.Clear();
        entities_by_id.Clear();
        for (int i = 0; i < mapWidth; i++) {
            for (int j = 0; j < mapHeight; j++) {
                map[i][j].ClearContents();
            }
        }
        // todo do we care about the kill list?
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
        entity.X = x;
        entity.Y = y;
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
        int xOffset = mapWidth / 2;
        int yOffset = mapHeight / 2;

        int xReal = x+xOffset;
        int yReal = y+yOffset;
        return (
            0 <= xReal && xReal < mapWidth
            && 0 <= yReal && yReal < mapHeight
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
    /// 
    /// Note: This may be worth resolving at some point in the future,
    /// but, as long as the map remains internal to BoardController, it's
    /// sufficient to route through GetCell().
    /// </summary>
    /// <param name="x">Horizontal coordinate in logical game coordinates.</param>
    /// <param name="y">Vertical coordinate in logical game coordinates.</param>
    /// <returns>Return the Cell at location (x, y)</returns>
    private Cell GetCell(int x, int y) {
        int xOffset = mapWidth / 2;
        int yOffset = mapHeight / 2;
        int mapXDestination = x+xOffset;
        int mapYDestination = y+yOffset;
        return map[mapXDestination][mapYDestination];
    }

    /// <summary>
    /// Places an Entity on the board.
    /// </summary>
    /// <param name="id">ID of the entity to move</param>
    /// <param name="x">Horizontal destination coordinate of the Entity</param>
    /// <param name="y">Vertical destination coordinate of the Entity</param>
    public void PlacePawn(int id, int x, int y) {
        Debug.Log($"Moving pawn {id} to ({x},{y})");
        Entity entity = entities_by_id[id];
        Cell destination = GetCell(x, y);
        Entity contentsToKill = destination.ClearContents();
        if (contentsToKill != null) {
            Kill(contentsToKill);
        }
        destination.PutContents(entity);
        Transform entity_transform = entity.gameObject.transform;
        entity_transform.position = new Vector3(x*GRID_MULTIPLE, y*GRID_MULTIPLE, 0f);
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
        // Get the two relevant cells.
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

        Entity entity = origin.ClearContents();
        PlacePawn(id, x1, y1);

    }

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

    public override void Save(GameDataWriter writer) {
        writer.Write(entities.Count);
        writer.Write(Random.state);
        for (int i = 0; i < entities.Count; i++) {
            writer.Write(entities[i].ID);
            entities[i].Save(writer);
        }
    }

    public override void Load (GameDataReader reader) {
        int version = reader.Version;
        if (version > saveVersion) {
			Debug.LogError("Unsupported future save version " + version);
			return;
		}
        ClearGame();
        LoadGame(reader);
    }

    void LoadGame (GameDataReader reader) {
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