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
            default:
                gameClient.PostEvent(new GameErrorEvent($"Unknown command type {cm.CommandType}"));
                return;
        }
    }

    private void HandleMoveCommand(MoveCommand cm) {
        int mx = cm.Direction.Item1;
        int my = cm.Direction.Item2;
        if (mainCharacter == null) {
            gameClient.PostEvent(new GameErrorEvent("mainCharacter has not been set!"));
            return;
        }
        MoveEntity(mainCharacter.ID, mainCharacter.X+mx, mainCharacter.Y+my);
    }

    private void HandleStartGameCommand(StartGameCommand cm) {
        if (RNG == null) {
            gameClient.PostEvent(new GameErrorEvent("Cannot begin game without RNG."));
            return;
        }
        DungeonBuilder.Build(this, RNG);
        gameClient.PostEvent(new GameStartedEvent());
    }
}