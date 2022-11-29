namespace URF.Server {
  using System.Collections.Generic;
  using System.Linq;
  using UnityEngine;
  using URF.Common.GameEvents;
  using URF.Common.Logging;
  using URF.Common.Persistence;
  using URF.Server.FieldOfView;
  using URF.Server.GameState;
  using URF.Server.Pathfinding;
  using URF.Server.RandomGeneration;
  using URF.Server.RulesSystems;

  public class GameServer : MonobehaviourBaseGameEventChannel {

    [SerializeField] private int mapWidth = 40;

    [SerializeField] private int mapHeight = 20;

    [SerializeField] private PersistentStorage persistentStorage;

    private IGameState gameState;

    private readonly List<IRulesSystem> rulesSystems = new();

    private PluginBundle pluginBundle;

    private readonly Queue<IGameEvent> gameEvents = new();

    private void Start() {

      this.pluginBundle = new PluginBundle(
        new UnityRandom(),
        new RaycastingFov(),
        new UnityDebugLogging(),
        new DjikstraPathfinding(),
        new EntityFactory.EntityFactory<Entity>(),
        this.persistentStorage
      );

      this.RegisterSystem(new GameStartSystem());
      this.RegisterSystem(new DebugSystem());
      this.RegisterSystem(new EntityInfoSystem());
      this.RegisterSystem(new MovementSystem());
      this.RegisterSystem(new CombatSystem());
      this.RegisterSystem(new IntelligenceSystem());
      this.RegisterSystem(new FieldOfViewSystem());
      this.RegisterSystem(new SerializationSystem());

      this.SetUpGame();
    }

    public override void HandleEvent(object _, IGameEvent e) {
      this.gameEvents.Enqueue(e);
      if (this.gameEvents.Count > 1) {
      } else {
        while (this.gameEvents.Any()) {
          IGameEvent nextEvent = this.gameEvents.Dequeue();
          base.HandleEvent(this, nextEvent);
        }
      }
    }

    public override void HandleAll(IGameEvent ev) {
      this.OnGameEvent(ev);
    }

    private void SetUpGame() {
      this.gameState = new GameState.GameState(this.mapWidth, this.mapHeight);
      foreach (IRulesSystem system in this.rulesSystems) {
        system.GameState = this.gameState;
      }
    }

    public override void HandleConfigure(ConfigureAction configureEvent) {
      this.SetUpGame();
      this.pluginBundle.Random.Rotate();
    }

    public override void HandleLoad(LoadAction loadAction) {
      this.SetUpGame();
    }

    private void RegisterSystem(IRulesSystem system) {
      // grant references to the plugins
      system.ApplyPlugins(this.pluginBundle);
      this.pluginBundle.EntityFactory.UpdateEntitySpec(system.Components);

      // want to queue incoming events rather than directly execute them
      this.Connect(system);
      this.rulesSystems.Add(system);
    }

  }
}
