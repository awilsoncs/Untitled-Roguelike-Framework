using UnityEngine;

[CreateAssetMenu]
public class DjikstraPathfindingPlugin : PathfindingPlugin {
    DjikstraPathfinding _impl;
    public override IPathfinding Impl => _impl;

    public DjikstraPathfindingPlugin() {
        _impl = new DjikstraPathfinding();
    }
}