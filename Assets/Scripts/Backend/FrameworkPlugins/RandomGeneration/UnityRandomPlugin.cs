using UnityEngine;

[CreateAssetMenu]
public class UnityRandomPlugin : RandomGeneratorPlugin {
    IRandomGenerator _impl;
    public override IRandomGenerator Impl => _impl;

    public UnityRandomPlugin() {
        _impl = new UnityRandom();
    }
}