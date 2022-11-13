using System.Collections.Generic;
using UnityEngine;
using URF.Client.GUI;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Game;
using URF.Server.GameState;

namespace URF.Client {
  /// <summary>
  /// GameClient client view
  /// </summary>
  [DisallowMultipleComponent]
  public partial class GameClient {

    [SerializeField] [Tooltip("Custom behavior to inject into the game controller.")]
    private BackendPlugins backendPlugins;

    [SerializeField] private GuiComponents gui;

    // NOTE: move this stuff to the game configurator
    private const int saveVersion = 1;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private PersistentStorage storage;

    [SerializeField] private int mapWidth = 40;

    [SerializeField] private int mapHeight = 20;

    private Random.State _mainRandomState;

    private GameState _gameState;

    private IEntityFactory _entityFactory;
    // end note

    private int MapWidth => mapWidth;

    private int MapHeight => mapHeight;

    private int _mainCharacterId;

    private (int, int) _mainCharacterPosition;

    // track attackable enemies so that we can attack instead of attempt to move
    private readonly Dictionary<int, (int, int)> _entityPosition = new();

    // todo add a convenience type here to simplify initialization
    private List<IEntity>[][] _entitiesByPosition;

    private void Start() {
      _mainRandomState = Random.state;
      // roughly halfway across the map
      mainCamera.transform.position = new Vector3(mapWidth / (2 / GameClient.gridMultiple),
        mapHeight / (2 / GameClient.gridMultiple), -10);
      // perform initial setup
      _pawns.Clear();
      _pawnsByID.Clear();
      _entityFactory = new EntityFactory();
      _gameState = new GameState(this, backendPlugins.RandomPlugin.Impl, _entityFactory,
        backendPlugins.FieldOfViewPlugin.Impl, backendPlugins.PathfindingPlugin.Impl,
        backendPlugins.LoggingPlugin.Impl, mapWidth, mapHeight);
      _gameState.EntityAttacked += HandleEntityAttacked;
      ClearEnemyPositions();
      BeginNewGame();
    }

    private void Update() {
      HandleUserInput();
    }

    private void BeginNewGame() {
      // set up the new game seed
      Random.state = _mainRandomState;
      int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
      _mainRandomState = Random.state;
      Random.InitState(seed);
      ClearEnemyPositions();
      _gameState.PostEvent(new StartGameCommand());
    }

    private void ClearEnemyPositions() {
      _entityPosition.Clear();
      _entitiesByPosition = new List<IEntity>[MapWidth][];
      for(int x = 0; x < MapWidth; x++) {
        _entitiesByPosition[x] = new List<IEntity>[MapHeight];
        for(int y = 0; y < MapHeight; y++) { _entitiesByPosition[x][y] = new List<IEntity>(); }
      }
    }

    private void ClearGame() {
      _gameState = new GameState(this, backendPlugins.RandomPlugin.Impl, _entityFactory,
        backendPlugins.FieldOfViewPlugin.Impl, backendPlugins.PathfindingPlugin.Impl,
        backendPlugins.LoggingPlugin.Impl, mapWidth, mapHeight);
      foreach(Pawn pawn in _pawns) { pawn.Recycle(pawnFactory); }
      _pawns.Clear();
      _pawnsByID.Clear();
    }

  }
}
