using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent the logical space of the game.
/// </summary>
[DisallowMultipleComponent]
public partial class Game : PersistableObject, IGameClient
{
    const int saveVersion = 1;
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

    Random.State mainRandomState;
    IGameState gameState;
    
    private void Start() {
        // todo figure out how to get randomness to the server
        mainRandomState = Random.state;
        mainCamera.transform.position = new Vector3(mapWidth / (2 / GRID_MULTIPLE), mapHeight / (2 / GRID_MULTIPLE), -10);
        gameState = new GameState(this, MapWidth, MapHeight);
        pawns = new List<Pawn>();
        pawns_by_id = new Dictionary<int, Pawn>();
        BeginNewGame();
    }

    private void Update() {
        HandleUserInput();
        // UpdateView();
    }

    private void BeginNewGame() {
        // Set up the new game seed
        Random.state = mainRandomState;
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
        mainRandomState = Random.state;
        Random.InitState(seed);
        DungeonBuilder.Build(gameState);
    }

    private void ClearGame() {
        // todo consider asking the gameState to reset itself
        gameState = new GameState(this, mapWidth, mapHeight);
        foreach (var pawn in pawns) {
            pawn.Recycle(pawnFactory);
        }
        pawns.Clear();
        pawns_by_id.Clear();
    }
}