using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour, IGameController
{
    private List<IUpdateable> updateables;
    private Dictionary<int, GameObject> pawn_reference;
    private const float GRID_MULTIPLE = 1f;

    [SerializeField]
    private GameObject mainCharacterPawn;

    private void Update() {
        foreach (var updateable in updateables) {
            updateable.Update();
        }
    }

    private void Start() {
        updateables = new List<IUpdateable>();
        pawn_reference = new Dictionary<int, GameObject>();
        CreateMainCharacter();
    }

    private void CreateMainCharacter() {
        var mob = new Mob(this, 0);
        var pawn = Instantiate(mainCharacterPawn);
        updateables.Add(mob);
        pawn_reference.Add(0, pawn);
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
        else return "none";
    }
}
