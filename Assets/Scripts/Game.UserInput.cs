using UnityEngine;

public partial class Game : IGameClient {
    private void HandleUserInput() {
        if (Input.GetKeyDown(leftKey)) Move(-1, 0);
        else if (Input.GetKeyDown(rightKey)) Move(1, 0);
        else if (Input.GetKeyDown(upKey)) Move(0, 1);
        else if (Input.GetKeyDown(downKey)) Move(0, -1);
        else if (Input.GetKeyDown(saveKey)) {
            Debug.Log("Player asked for save...");
            storage.Save(this, saveVersion);
        }
        else if (Input.GetKeyDown(loadKey)) {
            Debug.Log("Player asked for load...");
            storage.Load(this);
        }
        else if (Input.GetKeyDown(newGameKey)) {
            Debug.Log("Player asked for a reload...");
            ClearGame();
            BeginNewGame();
        }
    }

    private void Move(int x, int y) {
        gameState.PushCommand(new MoveCommand(x, y));
    }
}