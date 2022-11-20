namespace URF.Common.Persistence {
  using UnityEngine;

  public interface IGameDataWriter {
    void Write(float value);
    void Write(bool value);
    void Write(int value);
    void Write(string value);
    void Write(Vector3 value);
  }
}

