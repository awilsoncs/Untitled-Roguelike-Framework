using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : PersistableObject, IBoardController
{
    const int saveVersion = 0;

    private List<IEntity> entities;
    private Dictionary<int, GameObject> pawn_reference;
    private List<GameObject> pawns;
    private const float GRID_MULTIPLE = 0.5f;

    [SerializeField]
    private GameObject mainCharacterPawn;

    [SerializeField]
    PersistentStorage storage;

    private void Start() {
        BeginGame();
    }

    private void Update() {
        foreach (var entity in entities) {
            entity.Update();
        }
    }

    private void BeginGame() {
        entities = new List<IEntity>();
        pawns = new List<GameObject>();
        pawn_reference = new Dictionary<int, GameObject>();
        CreateMainCharacter();
    }

    private void CreateMainCharacter() {
        var mob = new Entity(this, 0);
        var pawn = Instantiate(mainCharacterPawn);
        entities.Add(mob);
        pawn_reference.Add(0, pawn);
        pawns.Add(pawn);
        SetPawnPosition(0, 0, 0);
    }

    public void LogMessage(String s) {
        Debug.Log(s);
    }

    public void SetPawnPosition(int id, int x, int y) {
        Debug.Log($"Moving pawn {id} to ({x},{y})");
        Transform pawn_transform = pawn_reference[id].transform;
        pawn_transform.position = new Vector3(x*GRID_MULTIPLE, y*GRID_MULTIPLE, 0f);
    }

    public String GetUserInputAction() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) return "left";
        if (Input.GetKeyDown(KeyCode.RightArrow)) return "right";
        if (Input.GetKeyDown(KeyCode.UpArrow)) return "up";
        if (Input.GetKeyDown(KeyCode.DownArrow)) return "down";
        if (Input.GetKeyDown(KeyCode.S)) return "save";
        if (Input.GetKeyDown(KeyCode.L)) return "load";
        if (Input.GetKeyDown(KeyCode.R)) return "reload";
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
        foreach (var pawn in pawns) {
            Destroy(pawn);            
        }
        pawns.Clear();
        pawn_reference.Clear();
    }

    public override void Save(GameDataWriter writer) {
        //
    }

    public override void Load(GameDataReader reader) {
        //
    }
}