using URFCommon;

public class GameStartSystem : BaseRulesSystem
{
    [EventHandler(GameEventType.StartGameCommand)]
    public void HandleGameStartCommand(IGameState gs, IGameEvent cm) {
        if (gs.RNG == null) {
            gs.PostError("Cannot begin game without RNG.");
            return;
        }
        if (gs.FieldOfView == null) {
            gs.PostError("Cannot begin game without FOV plugin.");
            return;
        }
        DungeonBuilder.Build(gs, gs.RNG);
        gs.RecalculateFOVImmediately();
        gs.PostEvent(new GameStartedEvent());
    }
}