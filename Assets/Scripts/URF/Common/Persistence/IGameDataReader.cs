namespace URF.Common.Persistence {
  using UnityEngine;

  public interface IGameDataReader {
    int Version {
      get;
    }

    bool ReadBool();
    float ReadFloat();
    int ReadInt();
    string ReadString();
    Vector3 ReadVector3();
  }
}
