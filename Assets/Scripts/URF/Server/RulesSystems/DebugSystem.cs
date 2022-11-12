using URF.Common.GameEvents;
using URF.Server.GameState;

namespace URF.Server.RulesSystems {
  public class DebugSystem : BaseRulesSystem {

    [EventHandler(GameEventType.DebugCommand)]
    public void HandleDebugCommand(IGameState gs, IGameEvent cm) {
      DebugCommand ev = (DebugCommand)cm;
      switch(ev.Method) {
        case DebugCommand.DebugMethod.SpawnCrab:
          gs.CreateEntityAtPosition("crab",
            (gs.Random.GetInt(1, gs.MapWidth - 2), gs.Random.GetInt(1, gs.MapHeight - 2)));
          return;
        default:
          gs.PostError($"Unknown debug method {ev.Method}");
          return;
      }
    }

  }
}
