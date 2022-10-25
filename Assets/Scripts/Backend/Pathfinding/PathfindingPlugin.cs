using UnityEngine;

public abstract class PathfindingPlugin : ScriptableObject {
    public abstract IPathfinding Impl {get;}

}