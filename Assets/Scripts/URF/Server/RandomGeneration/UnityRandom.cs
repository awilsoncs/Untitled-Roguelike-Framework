using UnityEngine;

namespace URF.Server.RandomGeneration {
  public class UnityRandom : IRandomGenerator {

    public int GetInt(int begin, int end) {
      return Random.Range(begin, end);
    }

  }
}
