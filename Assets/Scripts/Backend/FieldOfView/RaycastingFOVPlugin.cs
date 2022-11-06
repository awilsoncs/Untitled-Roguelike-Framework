using UnityEngine;

[CreateAssetMenu]
public class RaycastingFOVPlugin : FieldOfViewPlugin {
    public override IFieldOfView Impl {get;}
    public RaycastingFOVPlugin() {
        Impl = new RaycastingFOV();
    }
}