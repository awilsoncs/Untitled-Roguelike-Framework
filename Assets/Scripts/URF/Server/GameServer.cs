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

namespace URF.Server
{
  public class GameServer : BaseGameEventChannel
  {

    [SerializeField] private int mapWidth = 40;

    [SerializeField] private int mapHeight = 20;

    [SerializeField] private PersistentStorage persistentStorage;

    private readonly Dictionary<GameEventType, List<ActionHandler>> actionHandlers = new();

    private readonly Dictionary<GameEventType, List<EventHandler>> eventHandlers = new();

    private IGameState gameState;

    private bool inGameUpdateLoop;

    private bool isFieldOfViewDirty;

    private List<IEntity> killList;

    private PluginBundle pluginBundle;

    private void Start()
    {
      this.gameState = new GameState.GameState(this.mapWidth, this.mapHeight);
      foreach (GameEventType value in Enum.GetValues(typeof(GameEventType)))
      {
        this.eventHandlers[value] = new List<EventHandler>();
        this.actionHandlers[value] = new List<ActionHandler>();
      }

      this.pluginBundle = new PluginBundle(
        new UnityRandom(),
        new RaycastingFov(),
        new UnityDebugLogging(),
        new DjikstraPathfinding(),
        new EntityFactory(),
        this.persistentStorage);

      this.RegisterSystem(new GameStartSystem());
      this.RegisterSystem(new DebugSystem());
      this.RegisterSystem(new EntityInfoSystem());
      this.RegisterSystem(new MovementSystem());
      this.RegisterSystem(new CombatSystem());
      this.RegisterSystem(new IntelligenceSystem());
      this.RegisterSystem(new FieldOfViewSystem());
      this.RegisterSystem(new SerializationSystem());
    }

    private void StartGame()
    {
      this.pluginBundle.Random.Rotate();
    }

    protected override void HandleAction(object sender, IActionEventArgs ev)
    {
      switch (ev.EventType)
      {
        case GameEventType.Configure:
          this.gameState = new GameState.GameState(this.mapWidth, this.mapHeight);
          this.StartGame();
          break;
        case GameEventType.Load:
          this.gameState = new GameState.GameState(this.mapWidth, this.mapHeight);
          break;
      }

      foreach (ActionHandler actionHandler in this.actionHandlers[ev.EventType])
      {
        actionHandler(this.gameState, ev);
      }
    }

    private void HandleEvent(object sender, IGameEventArgs e)
    {
      foreach (EventHandler eventHandler in this.eventHandlers[e.EventType])
      {
        eventHandler(this.gameState, e);
      }

      // Forward to outside listeners
      this.OnGameEvent(e);
    }

    private void RegisterSystem(IRulesSystem system)
    {
      // Gather up listener methods
      this.RegisterRulesSystemListeners(system);

      // grant references to the plugins
      system.ApplyPlugins(this.pluginBundle);
      this.pluginBundle.EntityFactory.UpdateEntitySpec(system.Components);
      system.GameEvent += this.HandleEvent;
      system.GameAction += this.HandleAction;
    }

    private void RegisterRulesSystemListeners(IRulesSystem system)
    {
      // https://stackoverflow.com/questions/3467765/find-methods-that-have-custom-attribute-using-reflection
      // Dig up methods in a rules system with EventHandler and ActionHandler attribute,
      // then store those in the server's event handler registry.
      IEnumerable<MethodInfo> eventHandlerMethods = system.GetType().GetMethods().Where(
        x => Attribute.GetCustomAttributes(x, typeof(EventHandlerAttribute)).Length > 0);

      foreach (MethodInfo method in eventHandlerMethods)
      {
        object[] attributes = method.GetCustomAttributes(typeof(EventHandlerAttribute), false);
        foreach (object attribute in attributes)
        {
          EventHandlerAttribute eha = (EventHandlerAttribute)attribute;
          this.eventHandlers[eha.EventType].Add(
            (gs, ev) => { method.Invoke(system, new object[] { gs, ev }); });
        }
      }

      IEnumerable<MethodInfo> actionHandlerMethods = system.GetType().GetMethods().Where(
        x => Attribute.GetCustomAttributes(x, typeof(ActionHandlerAttribute)).Length > 0);

      foreach (MethodInfo method in actionHandlerMethods)
      {
        object[] attributes = method.GetCustomAttributes(typeof(ActionHandlerAttribute), false);
        foreach (object attribute in attributes)
        {
          ActionHandlerAttribute eha = (ActionHandlerAttribute)attribute;
          this.actionHandlers[eha.EventType].Add(
            (gs, ev) => { method.Invoke(system, new object[] { gs, ev }); });
        }
      }
    }

  }
}
