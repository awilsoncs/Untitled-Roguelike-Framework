using UnityEngine;

[CreateAssetMenu]
public class UnityRandomPlugin : RandomGeneratorPlugin {
    public override IRandomGenerator Impl {get;}

    public UnityRandomPlugin() {
        Impl = new UnityRandom();
    }
}