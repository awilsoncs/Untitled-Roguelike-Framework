using URF.Common;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Server.EntityFactory;
using URF.Server.GameState;
using URF.Server.RandomGeneration;

namespace URF.Server.RulesSystems {
  public class DebugSystem : BaseRulesSystem {

    private IRandomGenerator _random;

    private IEntityFactory<Entity> _entityFactory;

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      _random = pluginBundle.Random;
      _entityFactory = pluginBundle.EntityFactory;
    }

    [ActionHandler(GameEventType.DebugCommand)]
    public void HandleDebugAction(IGameState gs, IActionEventArgs cm) {
      DebugActionEventArgs ev = (DebugActionEventArgs)cm;
      switch (ev.Method) {
        case DebugActionEventArgs.DebugMethod.SpawnCrab:
          IEntity crab = _entityFactory.Get("crab");
          Position position = (_random.GetInt(1, gs.MapWidth - 2),
            _random.GetInt(1, gs.MapHeight - 2));
          gs.CreateEntityAtPosition(crab, position);
          OnGameEvent(new EntityCreatedEventArgs(crab));
          OnGameEvent(new EntityMovedEventArgs(crab, position));
          return;
        default:
          OnGameEvent(new GameErroredEventArgs($"Unknown debug method {ev.Method}"));
          return;
      }
    }

  }
}
