using System.Collections.Generic;
using UnityEngine;

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

    public Entity GetEntity(int id) {
        return entities_by_id[id];
    }

    private void Kill (Entity entity) {
        if (inGameUpdateLoop) {
            killList.Add(entity);
        } else {
            KillImmediately(entity);
        }
    }

    private void KillImmediately (Entity entity) {
        // swap and pop the desired entity to avoid rearranging the whole tail
        // todo can remove this scan by maintaining the index on the entity
        int index = entities.FindIndex(x => x == entity);
        int lastIndex = entities.Count - 1;
        if (index < lastIndex) {
            entities[index] = entities[lastIndex];
        }
        entities.RemoveAt(lastIndex);
        entities_by_id.Remove(entity.ID);
        Destroy(entity.gameObject);
    }

    private void BeginNewGame() {
        // Set up the new game seed
        Random.state = mainRandomState;
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
        mainRandomState = Random.state;
        Random.InitState(seed);
        // todo do we care that objects to kill are still in the kill list?
        CreateEntityByName("player");
    }

    private void ClearGame() {
        // Clean up the existing scene objects
        for (int i = 0; i < entities.Count; i++) {
            entities[i].Recycle();
        }

        entities.Clear();
        entities_by_id.Clear();
    }

    public Entity CreateEntityByName(System.String s) {
        var entity = entityFactory.Get(s);
        entities.Add(entity);
        entities_by_id.Add(entity.ID, entity);
        SetPawnPosition(entity.ID, 0, 0);
        return entity;
    }

    public void LogMessage(System.String s) {
        Debug.Log(s);
    }

    /*
    * Return true if a position is in bounds, false otherwise.
    */
    public bool IsInBounds(int x, int y) {
        // todo straighten out the coordinate mismatch, game should be in map coords
        int xOffset = mapWidth / 2;
        int yOffset = mapHeight / 2;

        int xReal = x+xOffset;
        int yReal = y+yOffset;
        return (
            0 <= xReal && xReal < mapWidth
            && 0 <= yReal && yReal < mapHeight
        );
    }

    public void SetPawnPosition(int id, int x, int y) {
        Debug.Log($"Moving pawn {id} to ({x},{y})");
        int xOffset = mapWidth / 2;
        int yOffset = mapHeight / 2;
        Entity entity = entities_by_id[id];
        map[x+xOffset][y+yOffset].SetContents(entities_by_id[id]);
        Transform entity_transform = entity.gameObject.transform;
        entity_transform.position = new Vector3(x*GRID_MULTIPLE, y*GRID_MULTIPLE, 0f);
    }

    public void SetPawnPosition(int id, int x0, int y0, int x1, int y1) {
        int xOffset = mapWidth / 2;
        int yOffset = mapHeight / 2;

        // Unity world Coords and the map indices are misaligned by half the
        // map dimension.
        map[x0+xOffset][y0+yOffset].ClearContents();
        Debug.Log($"Moving pawn {id} to ({x1},{y1})");
        Entity entity = entities_by_id[id];
        map[x1+xOffset][y1+yOffset].SetContents(entities_by_id[id]);
        Transform entity_transform = entity.gameObject.transform;
        entity_transform.position = new Vector3(x1*GRID_MULTIPLE, y1*GRID_MULTIPLE, 0f);
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
            Debug.Log($"read ID: {entityID}");
            var entity = entityFactory.Get();

            entity.ID = entityID;
            entity.Load(reader);

            entities.Add(entity);
            entities_by_id.Add(entity.ID, entity);
            SetPawnPosition(entity.ID, entity.X, entity.Y);
            Debug.Log($"<< Loaded object {i}");
        }
        Debug.Log("Done loading objects.");
    }
}