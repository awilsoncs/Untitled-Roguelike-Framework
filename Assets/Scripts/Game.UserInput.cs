using UnityEngine;

public partial class Game : PersistableObject, IGameClient {
    private void HandleUserInput() {
        if (Input.GetKeyDown(leftKey)) gameState.MoveLeft();
        else if (Input.GetKeyDown(rightKey)) gameState.MoveRight();
        else if (Input.GetKeyDown(upKey)) gameState.MoveUp();
        else if (Input.GetKeyDown(downKey)) gameState.MoveDown();
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
        else if (Input.GetKeyDown(createKey)) gameState.SpawnCrab();
    }
}