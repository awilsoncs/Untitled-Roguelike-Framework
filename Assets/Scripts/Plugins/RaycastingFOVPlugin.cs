using UnityEngine;

[CreateAssetMenu]
public class RaycastingFOVPlugin : FieldOfViewPlugin {
    IFieldOfView _impl;
    public override IFieldOfView Impl => _impl;

    public RaycastingFOVPlugin() {
        _impl = new RaycastingFOV();
    }
}