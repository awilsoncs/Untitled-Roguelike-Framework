using UnityEngine;
using URF.Server.RandomGeneration;

namespace URF.Game.Plugins {
  public class UnityRandom : IRandomGenerator {

    public int GetInt(int begin, int end) {
      return Random.Range(begin, end);
    }

  }
}
