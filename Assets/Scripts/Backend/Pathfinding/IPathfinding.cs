using System.Collections.Generic;

/// <summary>
/// Provide all pathfinding and related graph traversal logic.
/// </summary>
public interface IPathfinding {

    /// <summary>
    /// Get an optimal path from the start to the end, inclusive, over the
    /// given cost map. 
    /// </summary>
    /// <param name="costs">An array of costs to enter the node</param>
    /// <param name="start">The starting node</param>
    /// <param name="end">The ending node</param>
    List<(int, int)> GetPath(in float[][] costs, (int, int) start, (int, int) end);
}