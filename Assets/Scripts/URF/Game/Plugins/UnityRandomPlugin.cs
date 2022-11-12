using UnityEngine;
using URF.Server.RandomGeneration;

namespace URF.Game.Plugins
{
    [CreateAssetMenu]
    public class UnityRandomPlugin : RandomGeneratorPlugin {
        public override IRandomGenerator Impl {get;}

        public UnityRandomPlugin() {
            Impl = new UnityRandom();
        }
    }
}