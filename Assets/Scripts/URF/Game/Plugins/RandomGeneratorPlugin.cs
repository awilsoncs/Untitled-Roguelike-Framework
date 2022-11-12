using UnityEngine;
using URF.Server.RandomGeneration;

namespace URF.Game.Plugins {
  public abstract class RandomGeneratorPlugin : ScriptableObject {

    public abstract IRandomGenerator Impl { get; }

  }
}
