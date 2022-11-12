using UnityEngine;

namespace URF.Client {
  public partial class GameClient : PersistableObject {

    public override void Save(GameDataWriter writer) {
      writer.Write(Random.state);
      _gameState.Save(writer);
    }

    public override void Load(GameDataReader reader) {
      int version = reader.Version;
      if(version > saveVersion) {
        Debug.LogError("Unsupported future save version " + version);
        return;
      }
      Random.state = reader.ReadRandomState();
      ClearEnemyPositions();
      ClearGame();
      _gameState.Load(reader);
    }

  }
}
