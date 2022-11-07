using System.Collections.Generic;
using UnityEngine;

using URFCommon;
namespace URFFrontend {

    /// <summary>
    /// Game client view
    /// </summary>
    [DisallowMultipleComponent]
    public partial class Game : IPersistableObject
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

        int mainCharacterId;
        (int, int) mainCharacterPosition;
        // track attackable enemies so that we can attack instead of attempt to move
        (int, bool)[][] entityMap;
        Dictionary<int, (int, int)> entityPosition;

        [SerializeField]
        [Tooltip("Custom behavior to inject into the game controller.")]
        BackendPlugins backendPlugins;

        [SerializeField]
        GUIComponents gui;
        
        private void Start() {
            // todo figure out how to get randomness to the server
            mainRandomState = Random.state;
            // roughly halfway across the map
            mainCamera.transform.position = new Vector3(mapWidth / (2 / GRID_MULTIPLE), mapHeight / (2 / GRID_MULTIPLE), -10);
            // perform initial setup
            pawns = new List<Pawn>();
            pawns_by_id = new Dictionary<int, Pawn>();
            entityFactory = new EntityFactory();
            // todo could we just pass in the backend plugin collection?
            gameState = new GameState(
                this,
                backendPlugins.randomPlugin.Impl,
                entityFactory,
                backendPlugins.fieldOfViewPlugin.Impl,
                backendPlugins.pathfindingPlugin.Impl,
                backendPlugins.loggingPlugin.Impl,
                mapWidth,
                mapHeight
            );
            ClearEnemyPositions();
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
            ClearEnemyPositions();
            gameState.PostEvent(new StartGameCommand());
        }

        private void ClearEnemyPositions() {
            entityMap = new (int, bool)[mapWidth][]; 
            for (int i = 0; i < mapWidth; i++) {
                entityMap[i] = new (int, bool)[mapHeight];
                for (int j = 0; j < mapHeight; j++) {
                    entityMap[i][j] = (-1, false);
                }
            }
            entityPosition = new Dictionary<int, (int, int)>();
        }

        private void ClearGame() {
            gameState = new GameState(
                this,
                backendPlugins.randomPlugin.Impl,
                entityFactory,
                backendPlugins.fieldOfViewPlugin.Impl,
                backendPlugins.pathfindingPlugin.Impl,
                backendPlugins.loggingPlugin.Impl,
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

}