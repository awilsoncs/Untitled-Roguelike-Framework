using UnityEngine;
using URFCommon;

namespace URFFrontend{
    /// <summary>
    /// URF Plugin base for pathfinding logic.
    /// </summary>
    public abstract class PathfindingPlugin : ScriptableObject {
        public abstract IPathfinding Impl {get;}
    }
}