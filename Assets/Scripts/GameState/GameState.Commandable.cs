using System.Collections.Generic;

/// <summary>
/// Define commands available to the client.
/// </summary>
public partial class GameState : IGameState {
    public void PushCommand(IGameCommand cm) {
        switch (cm.CommandType) {
            case GameCommandType.Move:
                HandleMoveCommand((MoveCommand)cm);
                return;
            case GameCommandType.StartGame:
                HandleStartGameCommand((StartGameCommand)cm);
                return;
            case GameCommandType.Debug:
                HandleDebugCommand((DebugCommand)cm);
                return;
            default:
                PostError($"Unknown command type {cm.CommandType}");
                return;
        }
    }

    private void HandleMoveCommand(MoveCommand cm) {
        int mx = cm.Direction.Item1;
        int my = cm.Direction.Item2;
        if (mainCharacter == null) {
            PostError("mainCharacter has not been set!");
            return;
        }
        MoveEntity(mainCharacter.ID, mainCharacter.X+mx, mainCharacter.Y+my);
    }

    private void HandleStartGameCommand(StartGameCommand cm) {
        if (RNG == null) {
            PostError("Cannot begin game without RNG.");
            return;
        }
        DungeonBuilder.Build(this, RNG);
        RecalculateFOVImmediately();
        gameClient.PostEvent(new GameStartedEvent());
    }

    private void HandleDebugCommand(DebugCommand cm) {
        switch (cm.Method) {
            case DebugCommand.DebugMethod.SpawnCrab:
                CreateEntityAtPosition(
                    "crab",
                    RNG.GetInt(1, MapWidth-2),
                    RNG.GetInt(1, MapHeight-2)
                );
                return;
            default:
                PostError($"Unknown debug method {cm.Method}");
                return;
        }
    }
}