using UnityEngine;
using URF.Server.Pathfinding;

namespace URF.Client.Pathfinding {
  /// <summary>
  /// URF Plugin base for pathfinding logic.
  /// </summary>
  public abstract class PathfindingPlugin : ScriptableObject {

    public abstract IPathfinding Impl { get; }

  }
}
