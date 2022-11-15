using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Common.Logging;
using URF.Common.Persistence;
using URF.Server.FieldOfView;
using URF.Server.GameState;
using URF.Server.Pathfinding;
using URF.Server.RandomGeneration;
using URF.Server.RulesSystems;
using EventHandler = URF.Server.RulesSystems.EventHandler;
using Random = UnityEngine.Random;

namespace URF.Server {
  public class GameServer : BaseGameEventChannel {

    [SerializeField] private int mapWidth = 40;

    [SerializeField] private int mapHeight = 20;

    [SerializeField] private PersistentStorage persistentStorage;

    private IGameState _gameState;

    private IEntityFactory _entityFactory;

    private Random.State _mainRandomState;

    private readonly List<IRulesSystem> _rulesSystems = new();

    private readonly Dictionary<GameEventType, List<EventHandler>> _eventHandlers = new();

    private readonly Dictionary<GameEventType, List<ActionHandler>> _actionHandlers = new();

    private bool _isFieldOfViewDirty;

    private bool _inGameUpdateLoop;

    private List<IEntity> _killList;

    private PluginBundle _pluginBundle;

    private void Start() {
      _mainRandomState = Random.state;
      _entityFactory = new EntityFactory();
      _gameState = new GameState.GameState(mapWidth, mapHeight, _entityFactory);
      foreach(GameEventType value in Enum.GetValues(typeof(GameEventType))) {
        _eventHandlers[value] = new List<EventHandler>();
        _actionHandlers[value] = new List<ActionHandler>();
      }

      _pluginBundle = new PluginBundle(new UnityRandom(), new RaycastingFov(),
        new UnityDebugLogging(), new DjikstraPathfinding(), _entityFactory, persistentStorage);

      RegisterSystem(new GameStartSystem());
      RegisterSystem(new DebugSystem());
      RegisterSystem(new EntityInfoSystem());
      RegisterSystem(new MovementSystem());
      RegisterSystem(new CombatSystem());
      RegisterSystem(new IntelligenceSystem());
      RegisterSystem(new FieldOfViewSystem());
      RegisterSystem(new SerializationSystem());
    }

    private void StartGame() {
      _pluginBundle.Random.Rotate();
    }

    protected override void HandleAction(object sender, IActionEventArgs ev) {
      // actions are only forwarded downwards.
      // todo implement all player actions again
      switch(ev.EventType) {
        case GameEventType.Configure:
          _gameState = new GameState.GameState(mapWidth, mapHeight, _entityFactory);
          StartGame();
          break;
        case GameEventType.Load:
          _gameState = new GameState.GameState(mapWidth, mapHeight, _entityFactory);
          break;
      }

      foreach(ActionHandler actionHandler in _actionHandlers[ev.EventType]) {
        actionHandler(_gameState, ev);
      }
    }

    private void HandleEvent(object sender, IGameEventArgs e) {
      foreach(EventHandler eventHandler in _eventHandlers[e.EventType]) {
        eventHandler(_gameState, e);
      }
      // Forward to outside listeners
      OnGameEvent(e);
    }

    private void RegisterSystem(IRulesSystem system) {
      // https://stackoverflow.com/questions/3467765/find-methods-that-have-custom-attribute-using-reflection
      _rulesSystems.Add(system);
      // Gather up listener methods
      RegisterRulesSystemListeners(system);
      // grant references to the plugins
      system.ApplyPlugins(_pluginBundle);
      _entityFactory.UpdateEntitySpec(system.Components);
      system.GameEvent += HandleEvent;
      system.GameAction += HandleAction;
    }

    private void RegisterRulesSystemListeners(IRulesSystem system) {
      // Dig up methods in a rules system with EventHandler and ActionHandler attribute,
      // then store those in the server's event handler registry.
      IEnumerable<MethodInfo> eventHandlerMethods = system.GetType().GetMethods().Where(x =>
        Attribute.GetCustomAttributes(x, typeof(EventHandlerAttribute)).Length > 0);

      foreach(MethodInfo method in eventHandlerMethods) {
        object[] eventHandlers = method.GetCustomAttributes(typeof(EventHandlerAttribute), false);
        foreach(object attribute in eventHandlers) {
          EventHandlerAttribute eha = (EventHandlerAttribute)attribute;
          _eventHandlers[eha.EventType].Add((gs, ev) => {
            method.Invoke(system, new object[] { gs, ev });
          });
        }
      }

      IEnumerable<MethodInfo> actionHandlerMethods = system.GetType().GetMethods().Where(x =>
        Attribute.GetCustomAttributes(x, typeof(ActionHandlerAttribute)).Length > 0);

      foreach(MethodInfo method in actionHandlerMethods) {
        object[] actionHandlers = method.GetCustomAttributes(typeof(ActionHandlerAttribute), false);
        foreach(object attribute in actionHandlers) {
          ActionHandlerAttribute eha = (ActionHandlerAttribute)attribute;
          _actionHandlers[eha.EventType].Add((gs, ev) => {
            method.Invoke(system, new object[] { gs, ev });
          });
        }
      }
    }

  }
}
