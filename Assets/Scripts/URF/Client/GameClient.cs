using System.Collections.Generic;
using UnityEngine;
using URF.Client.GUI;
using URF.Common;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Game;
using URF.Server;

namespace URF.Client {
  /// <summary>
  /// GameClient client view
  /// </summary>
  [DisallowMultipleComponent]
  public partial class GameClient : IPlayerActionChannel {

    [SerializeField] private GuiComponents gui;
    
    private const float gridMultiple = 0.5f;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private PersistentStorage storage;
    
    [SerializeField] private BaseGameEventChannel gameEventChannel;
    
    [SerializeField] private PawnFactory pawnFactory;

    private readonly List<Pawn> _pawns = new();

    private readonly Dictionary<int, Pawn> _pawnsByID = new();

    private int _mainCharacterId;

    private (int, int) _mainCharacterPosition;

    private readonly Queue<IGameEventArgs> _gameEvents = new();

    // track attackable enemies so that we can attack instead of attempt to move
    private readonly Dictionary<int, (int, int)> _entityPosition = new();

    // todo add a convenience type here to simplify initialization
    private List<IEntity>[][] _entitiesByPosition;

    private void Start() {
      gameEventChannel.GameEvent += EnqueueGameEvent;
      gameEventChannel.Connect(this);
      BeginNewGame();
    }

    private void EnqueueGameEvent(object sender, IGameEventArgs e) {
      _gameEvents.Enqueue(e);
    }

    private void Update() {
      while(_gameEvents.Count > 0) {
        HandleGameEvent(_gameEvents.Dequeue());
      }
      HandleUserInput();
    }

    private void BeginNewGame() {
      ResetEverything();
      OnPlayerAction(new StartGameActionArgs());
    }

    private void ResetEverything() {
      _pawns.Clear();
      _pawnsByID.Clear();
      _gameEvents.Clear();
      _entityPosition.Clear();
    }

    public void ConfigureClientMap(Position mapSize) {
      _entitiesByPosition = new List<IEntity>[mapSize.X][];
      for(int x = 0; x < mapSize.X; x++) {
        _entitiesByPosition[x] = new List<IEntity>[mapSize.Y];
        for(int y = 0; y < mapSize.Y; y++) { _entitiesByPosition[x][y] = new List<IEntity>(); }
      }
    }

    private void ClearGame() {
      foreach(Pawn pawn in _pawns) { pawn.Recycle(pawnFactory); }
      _pawns.Clear();
      _pawnsByID.Clear();
    }

  }
}
