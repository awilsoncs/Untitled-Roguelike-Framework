using System.IO;
using UnityEngine;

namespace URF.Common.Persistence {

  public class GameDataWriter : IGameDataWriter {

    private readonly BinaryWriter _writer;

    public GameDataWriter(BinaryWriter writer) {
      _writer = writer;
    }

    public void Write(float value) {
      _writer.Write(value);
    }

    public void Write(bool value) {
      _writer.Write(value);
    }

    public void Write(int value) {
      _writer.Write(value);
    }

    public void Write(string value) {
      value ??= "";
      _writer.Write(value);
    }

    public void Write(Vector3 value) {
      _writer.Write(value.x);
      _writer.Write(value.y);
      _writer.Write(value.z);
    }

  }
}
