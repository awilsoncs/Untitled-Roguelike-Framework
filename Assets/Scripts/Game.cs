using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent the logical space of the game.
/// </summary>
[DisallowMultipleComponent]
public partial class Game : PersistableObject
{
    const int saveVersion = 1;
    [SerializeField] Camera mainCamera;
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
    // todo shouldn't this be ICommandable?
    IGameState gameState;
    IEntityFactory entityFactory;

    [SerializeField]
    [Tooltip("Custom behavior to inject into the game controller.")]
    BackendPlugins backendPlugins;
    
    private void Start() {
        // todo figure out how to get randomness to the server
        mainRandomState = Random.state;
        // roughly halfway across the map
        mainCamera.transform.position = new Vector3(mapWidth / (2 / GRID_MULTIPLE), mapHeight / (2 / GRID_MULTIPLE), -10);
        // perform initial setup
        pawns = new List<Pawn>();
        pawns_by_id = new Dictionary<int, Pawn>();
        entityFactory = new EntityFactory();
        gameState = new GameState(
            this,
            backendPlugins.randomPlugin.Impl,
            entityFactory,
            backendPlugins.fieldOfViewPlugin.Impl,
            mapWidth,
            mapHeight
        );
        BeginNewGame();
    }

    private void Update() {
        HandleUserInput();
    }

    private void BeginNewGame() {
        // set up the new game seed
        Random.state = mainRandomState;
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
        mainRandomState = Random.state;
        Random.InitState(seed);

        gameState.PushCommand(new StartGameCommand());
    }

    private void ClearGame() {
        gameState = new GameState(
            this,
            backendPlugins.randomPlugin.Impl,
            entityFactory,
            backendPlugins.fieldOfViewPlugin.Impl,
            mapWidth,
            mapHeight
        );
        foreach (var pawn in pawns) {
            pawn.Recycle(pawnFactory);
        }
        pawns.Clear();
        pawns_by_id.Clear();
    }
}