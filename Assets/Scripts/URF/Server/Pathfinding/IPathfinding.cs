using System.Collections.Generic;
using URF.Common;

namespace URF.Server.Pathfinding {
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
        /// <return>A list of positions detailing an optimal path from start to
        /// end. If the list is empty, there is no possible path.</return>
        List<Position> GetPath(in float[][] costs, Position start, Position end);
    }
}
