namespace URF.Server.RulesSystems {
  using System.Linq;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server.EntityFactory;
  using URF.Algorithms;

  public class DebugSystem : BaseRulesSystem {

    private IRandomGenerator random;

    private IEntityFactory<Entity> entityFactory;

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      this.random = pluginBundle.Random;
      this.entityFactory = pluginBundle.EntityFactory;
    }

    public override void HandleTargetEvent(TargetEvent targetEvent) {
      if (targetEvent.Method == TargetEvent.TargetEventMethod.Response) {
        this.OnGameEvent(new GameErrored($"target: {targetEvent.Targets.Single()}"));
      }
    }

    public override void HandleDebug(DebugAction ev) {
      switch (ev.Method) {
        case DebugAction.DebugMethod.SpawnCrab:
          IEntity crab = this.entityFactory.Get("crab");
          Position position = (this.random.GetInt(1, this.GameState.MapSize.X - 2),
            this.random.GetInt(1, this.GameState.MapSize.Y - 2));
          this.GameState.CreateEntity(crab);
          this.GameState.PlaceEntityOnMap(crab, position);
          return;
        case DebugAction.DebugMethod.TriggerRequest:
          // The player wants to test requests
          this.OnGameEvent(new TargetEvent(TargetEvent.TargetEventMethod.Request));
          break;
        default:
          this.OnGameEvent(new GameErrored($"Unknown debug method {ev.Method}"));
          return;
      }
    }

  }
}
