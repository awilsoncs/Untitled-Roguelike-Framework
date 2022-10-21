using UnityEngine;
public partial class Game : PersistableObject {

    public override void Save(GameDataWriter writer) {
        writer.Write(Random.state);
        gameState.Save(writer);
    }

    /// <summary>
    /// Load the game from the save file.
    /// </summary>
    /// <param name="reader"></param>
    public override void Load (GameDataReader reader) {
        int version = reader.Version;
        if (version > saveVersion) {
			Debug.LogError("Unsupported future save version " + version);
			return;
		}
        ClearGame();
        Random.State state = reader.ReadRandomState();
        gameState.Load(reader);
    }
}