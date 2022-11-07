using UnityEngine;
using URFCommon;

namespace URFFrontend {
    [CreateAssetMenu]
    public class DjikstraPathfindingPlugin : PathfindingPlugin {
        public override IPathfinding Impl {get;}

        public DjikstraPathfindingPlugin() {
            Impl = new DjikstraPathfinding();
        }
    }
}
