namespace URF.Server.RulesSystems {
  using System.Linq;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server.EntityFactory;
  using URF.Algorithms;
  using URF.Common.Effects;
  using URF.Server.Resolvables;
  using System.Collections.Generic;

  public class DebugSystem : BaseRulesSystem {

    private IRandomGenerator random;

    private IEntityFactory<Entity> entityFactory;

    private IEntity mainCharacter;

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      this.random = pluginBundle.Random;
      this.entityFactory = pluginBundle.EntityFactory;
    }

    public override void HandleMainCharacterChanged(MainCharacterChanged ev) {
      this.mainCharacter = ev.Entity;
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
        case DebugAction.DebugMethod.Damage:
          this.OnGameEvent(
            new ResolvableEvent(
              new Resolvable(
                this.mainCharacter,
                TargetScope.OneCreature,
                new HashSet<IEffect>() { EffectType.DamageHealth.WithMagnitude(1000) }
              )
            )
          );
          break;
        default:
          this.OnGameEvent(new GameErrored($"Unknown debug method {ev.Method}"));
          return;
      }
    }

  }
}
