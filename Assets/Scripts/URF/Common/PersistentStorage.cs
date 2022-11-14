using System.IO;
using UnityEngine;
using URF.Common.Persistence;

namespace URF.Common {
  /// <summary>
  /// Specify a save path and set up a binary interface to it.
  /// </summary>
  public class PersistentStorage : MonoBehaviour {

    private string _savePath;

    private void Awake() {
      _savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    public void Save(IPersistableObject o, int version) {
      using(BinaryWriter writer = new(File.Open(_savePath, FileMode.Create))) {
        writer.Write(version);
        o.Save(new GameDataWriter(writer));
      }
      Debug.Log($"GameClient saved to {_savePath}");
    }

    public void Load(IPersistableObject o) {
      byte[] data = File.ReadAllBytes(_savePath);
      BinaryReader reader = new(new MemoryStream(data));
      o.Load(new GameDataReader(reader, reader.ReadInt32()));
    }

  }
}
