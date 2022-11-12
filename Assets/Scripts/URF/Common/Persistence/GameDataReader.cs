using System.IO;
using UnityEngine;

namespace URF.Common.Persistence {
  public class GameDataReader {

    public int Version { get; }

    private readonly BinaryReader _reader;

    public GameDataReader(BinaryReader reader, int version) {
      _reader = reader;
      Version = version;
    }

    public float ReadFloat() {
      return _reader.ReadSingle();
    }

    public bool ReadBool() {
      return _reader.ReadBoolean();
    }

    public int ReadInt() {
      return _reader.ReadInt32();
    }

    public string ReadString() {
      return _reader.ReadString();
    }

    public Vector3 ReadVector3() {
      Vector3 value;
      value.x = _reader.ReadSingle();
      value.y = _reader.ReadSingle();
      value.z = _reader.ReadSingle();
      return value;
    }

    public Random.State ReadRandomState() {
      return JsonUtility.FromJson<Random.State>(_reader.ReadString());
    }

  }
}
