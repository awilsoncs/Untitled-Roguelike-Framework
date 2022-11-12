using UnityEngine;
namespace URFFrontend {
    public partial class Game : PersistableObject {

        public override void Save(GameDataWriter writer) {
            writer.Write(Random.state);
            gameState.Save(writer);
        }

        public override void Load (GameDataReader reader) {
            int version = reader.Version;
            if (version > saveVersion) {
                Debug.LogError("Unsupported future save version " + version);
                return;
            }
            Random.state = reader.ReadRandomState();
            ClearEnemyPositions();
            ClearGame();
            gameState.Load(reader);
        }
    }
}