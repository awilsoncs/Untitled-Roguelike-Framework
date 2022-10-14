using System;

// Define an interface into the game controller for Mobs and Events.
public interface IBoardController {
    void LogMessage(String s);
    void SetPawnPosition(int id, int x, int y);
    String GetUserInputAction();
    void SaveGame();
    void LoadGame();
    void ReloadGame();
}