namespace URF.Server.RulesSystems {
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server.EntityFactory;
  using URF.Server.RandomGeneration;

  public class DebugSystem : BaseRulesSystem {

    private IRandomGenerator random;

    private IEntityFactory<Entity> entityFactory;

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      this.random = pluginBundle.Random;
      this.entityFactory = pluginBundle.EntityFactory;
    }

    public override void HandleDebug(DebugAction ev) {
      switch (ev.Method) {
        case DebugAction.DebugMethod.SpawnCrab:
          IEntity crab = this.entityFactory.Get("crab");
          Position position = (this.random.GetInt(1, this.GameState.MapWidth - 2),
            this.random.GetInt(1, this.GameState.MapHeight - 2));
          this.GameState.CreateEntityAtPosition(crab, position);
          this.OnGameEvent(new EntityCreated(crab));
          this.OnGameEvent(new EntityMoved(crab, position));
          return;
        default:
          this.OnGameEvent(new GameErrored($"Unknown debug method {ev.Method}"));
          return;
      }
    }

  }
}
