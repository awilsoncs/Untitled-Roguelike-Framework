using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Server.GameState;
using URF.Server.RandomGeneration;

namespace URF.Server.RulesSystems {
  public class DebugSystem : BaseRulesSystem {

    private IRandomGenerator _random;

    private IEntityFactory _entityFactory;

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      _random = pluginBundle.Random;
      _entityFactory = pluginBundle.EntityFactory;
    }

    [ActionHandler(GameEventType.DebugCommand)]
    public void HandleDebugAction(IGameState gs, IActionEventArgs cm) {
      DebugActionEventArgs ev = (DebugActionEventArgs)cm;
      switch(ev.Method) {
        case DebugActionEventArgs.DebugMethod.SpawnCrab:
          IEntity crab = _entityFactory.Get("crab");
          gs.CreateEntityAtPosition(crab,
            (_random.GetInt(1, gs.MapWidth - 2), _random.GetInt(1, gs.MapHeight - 2)));
          OnGameEvent(new EntityCreatedEventArgs(crab));
          return;
        default:
          OnGameEvent(new GameErroredEventArgs($"Unknown debug method {ev.Method}"));
          return;
      }
    }

  }
}
