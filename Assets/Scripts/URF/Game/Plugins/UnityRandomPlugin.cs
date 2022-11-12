using UnityEngine;
using URF.Server.RandomGeneration;

namespace URF.Game.Plugins {
  [CreateAssetMenu(menuName = "URF Plugins/Random/Unity Random")]
  public class UnityRandomPlugin : RandomGeneratorPlugin {

    public override IRandomGenerator Impl { get; }

    public UnityRandomPlugin() {
      Impl = new UnityRandom();
    }

  }
}
