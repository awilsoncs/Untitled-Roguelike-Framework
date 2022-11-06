using URFCommon;

public class DebugSystem : BaseRulesSystem
{
    [EventHandler(GameEventType.DebugCommand)]
    public void HandleDebugCommand(IGameState gs, IGameEvent cm) {
        var ev = (DebugCommand)cm;
        switch (ev.Method) {
            case DebugCommand.DebugMethod.SpawnCrab:
                gs.CreateEntityAtPosition(
                    "crab",
                    gs.RNG.GetInt(1, gs.MapWidth-2),
                    gs.RNG.GetInt(1, gs.MapHeight-2)
                );
                return;
            default:
                gs.PostError($"Unknown debug method {ev.Method}");
                return;
        }
    }
}