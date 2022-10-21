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
                gameClient.PostEvent(new MessageLoggedEvent($"Unknown command type {cm.CommandType}"));
                return;
        }
    }

    private void HandleMoveCommand(MoveCommand cm) {
        int mx = cm.Direction.Item1;
        int my = cm.Direction.Item2;
        if (mainCharacter == null) {
            gameClient.PostEvent(new MessageLoggedEvent("mainCharacter has not been set!"));
            return;
        }
        MoveEntity(mainCharacter.ID, mainCharacter.X+mx, mainCharacter.Y+my);
    }

    private void HandleStartGameCommand(StartGameCommand cm) {
        gameClient.PostEvent(new MessageLoggedEvent("Game starting!"));
        DungeonBuilder.Build(this);
    }
}