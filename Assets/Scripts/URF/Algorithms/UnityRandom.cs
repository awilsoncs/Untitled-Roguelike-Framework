using UnityEngine;
using URF.Common.Persistence;

namespace URF.Algorithms {
  public class UnityRandom : IRandomGenerator {

    private Random.State _mainRandomState;

    public UnityRandom() {
      _mainRandomState = Random.state;
    }

    public int GetInt(int begin, int end) {
      return Random.Range(begin, end);
    }

    public void Rotate() {
      Random.state = _mainRandomState;
      int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
      _mainRandomState = Random.state;
      Random.InitState(seed);
    }

    public void Save(IGameDataWriter writer) {
      writer.Write(JsonUtility.ToJson(Random.state));
    }

    public void Load(IGameDataReader reader) {
      Random.state = JsonUtility.FromJson<Random.State>(reader.ReadString());
    }

  }
}
