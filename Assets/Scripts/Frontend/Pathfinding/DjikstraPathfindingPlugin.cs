using UnityEngine;
using URFCommon;

namespace URFFrontend {
    // todo these should really live in a backend wrapper namespace
    [CreateAssetMenu]
    public class DjikstraPathfindingPlugin : PathfindingPlugin {
        public override IPathfinding Impl {get;}

        public DjikstraPathfindingPlugin() {
            Impl = new DjikstraPathfinding();
        }
    }
}
