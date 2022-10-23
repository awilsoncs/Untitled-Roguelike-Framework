using UnityEngine;
public abstract class RandomGeneratorPlugin : ScriptableObject {
    public abstract IRandomGenerator Impl {get;}
}