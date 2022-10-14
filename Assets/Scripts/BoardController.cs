using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : PersistableObject
{
    const int saveVersion = 0;

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
        CreateEntityByIndex(0);
    }

    public Entity CreateEntityByIndex(int n) {
        var entity = entityFactory.Get(n);
        entities.Add(entity);
        entities_by_id.Add(entity.ID, entity.gameObject);
        SetPawnPosition(entity.ID, 0, 0);
        return entity;
    }

    public void LogMessage(String s) {
        Debug.Log(s);
    }

    public void SetPawnPosition(int id, int x, int y) {
        Debug.Log($"Moving pawn {id} to ({x},{y})");
        Transform entity_transform = entities_by_id[id].transform;
        entity_transform.position = new Vector3(x*GRID_MULTIPLE, y*GRID_MULTIPLE, 0f);
    }

    public String GetUserInputAction() {

        if (Input.GetKeyDown(KeyCode.LeftArrow)) return "left";
        if (Input.GetKeyDown(KeyCode.RightArrow)) return "right";
        if (Input.GetKeyDown(KeyCode.UpArrow)) return "up";
        if (Input.GetKeyDown(KeyCode.DownArrow)) return "down";
        if (Input.GetKeyDown(KeyCode.S)) return "save";
        if (Input.GetKeyDown(KeyCode.L)) return "load";
        if (Input.GetKeyDown(KeyCode.R)) return "reload";
        if (Input.GetKeyDown(KeyCode.P)) return "spawn";
        else return "none";
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
        writer.Write(entities.Count);
        foreach (var entity in entities) {
            writer.Write(entity.EntityType);
            entity.Save(writer);
        }
    }

    public override void Load(GameDataReader reader) {
        ClearGame();
        var entityCount = reader.ReadInt();
        for (int i = 0; i < entityCount; i++) {
            var entityType = reader.ReadInt();
            var entity = entityFactory.Get(entityType);
            entity.Load(reader);
            entities.Add(entity);
            entities_by_id.Add(entity.ID, entity.gameObject);
            SetPawnPosition(entity.ID, entity.X, entity.Y);
        }
    }
}