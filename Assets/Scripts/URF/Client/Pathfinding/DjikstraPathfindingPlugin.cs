using UnityEngine;
using URF.Server.Pathfinding;

namespace URF.Client.Pathfinding {
  [CreateAssetMenu]
  public class DjikstraPathfindingPlugin : PathfindingPlugin {

    public override IPathfinding Impl { get; }

    public DjikstraPathfindingPlugin() {
      Impl = new DjikstraPathfinding();
    }

  }
}
