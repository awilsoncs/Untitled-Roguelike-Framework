using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : PersistableObject
{
    const int saveVersion = 0;
    int turn = 0;

    List<Entity> entities;
    Dictionary<int, GameObject> entities_by_id;
    const float GRID_MULTIPLE = 0.5f;

    [SerializeField] PersistentStorage storage;
    [SerializeField] EntityFactory entityFactory;

    public static BoardController Instance { get; set; }

    void OnEnable () {
        Instance = this;
	}
    
    private void Start() {
        BeginGame();
    }

    private void Update() {
        foreach (var entity in entities) {
            entity.GameUpdate();
        }
    }

    private void BeginGame() {
        entities = new List<Entity>();
        entities_by_id = new Dictionary<int, GameObject>();
        CreateMainCharacter();
    }

    private void CreateMainCharacter() {
        var entity = entityFactory.Get(0);
        entities.Add(entity);
        entities_by_id.Add(0, entity.gameObject);
        SetPawnPosition(0, 0, 0);
    }

    public void LogMessage(String s) {
        Debug.Log(s);
    }

    public void SetPawnPosition(int id, int x, int y) {
        Debug.Log($"{turn}: Moving pawn {id} to ({x},{y})");
        Transform entity_transform = entities_by_id[id].transform;
        entity_transform.position = new Vector3(x*GRID_MULTIPLE, y*GRID_MULTIPLE, 0f);
    }

    public String GetUserInputAction() {
        // todo figure out why this moves twice
        turn += 1;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) return "left";
        if (Input.GetKeyDown(KeyCode.RightArrow)) return "right";
        if (Input.GetKeyDown(KeyCode.UpArrow)) return "up";
        if (Input.GetKeyDown(KeyCode.DownArrow)) return "down";
        if (Input.GetKeyDown(KeyCode.S)) return "save";
        if (Input.GetKeyDown(KeyCode.L)) return "load";
        if (Input.GetKeyDown(KeyCode.R)) return "reload";
        else {
            turn -= 1;
            return "none";
        }
    }
    
    public void SaveGame() {
        storage.Save(this, saveVersion);
    }
    
    public void LoadGame() {
        storage.Load(this);
    }

    public void ReloadGame() {
        ClearGame();
        BeginGame();
    }

    public void ClearGame() {
        foreach (var entity in entities) {
            Destroy(entity.gameObject);            
        }
        entities.Clear();
        entities_by_id.Clear();
    }

    public override void Save(GameDataWriter writer) {
        //
    }

    public override void Load(GameDataReader reader) {
        //
    }
}