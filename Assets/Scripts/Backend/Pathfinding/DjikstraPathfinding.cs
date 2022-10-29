using System.Collections.Generic;

/// <summary>
/// Pathfinding implemented using Djikstra's Algorithm.
/// </summary>
public class DjikstraPathfinding : IPathfinding
{
    public List<(int, int)> GetPath(in float[][] costs, (int, int) start, (int, int) end)
    {
        // http://www.roguebasin.com/index.php/Pathfinding
        // generate a cost map
        var frontier = new PriorityQueue<float, (int, int)>(); // P
        // contains visited nodes and the lowest known cost to reach them
        var visitedNodes = new Dictionary<(int, int), float>(); // V
        var nodesByBestPredecessor = new Dictionary<(int, int), (int, int)>();

        frontier.Enqueue(costs[start.Item1][start.Item2], start);
        visitedNodes[start] = 0f;
        while (frontier.Count > 0) {
            var node = frontier.Dequeue();
            if (node.Equals(end)) {
                return UnwindBestPath(nodesByBestPredecessor, node);
            }
            var nodeCost = visitedNodes[node];
            foreach (var successor in GetSuccessors(node)) {
                if (OutOfBounds(costs.Length, costs[0].Length, successor)) {
                    continue;
                }

                float totalCost = nodeCost + costs[successor.Item1][successor.Item2];
                if (IsImprovedPath(frontier, visitedNodes, successor, totalCost)) {
                    UpdateNodeInventoryWithNewPath(
                        frontier,
                        visitedNodes,
                        nodesByBestPredecessor,
                        node,
                        successor,
                        totalCost
                    );
                }
            }
        }
        
        // Empty list indicates no path could be found.
        return new List<(int, int)>();
    }

    /// <summary>
    /// Work backwards through the best predecessors to find the optimal path.
    /// </summary>
    private static List<(int, int)> UnwindBestPath(Dictionary<(int, int), (int, int)> nodesByBestPredecessor, (int, int) node)
    {
        var backTrack = node;
        var path = new List<(int, int)>();
        path.Add(node);
        while (nodesByBestPredecessor.ContainsKey(backTrack)) {
            // set the backtrack cursor to the best predecessor
            backTrack = nodesByBestPredecessor[backTrack];
            // add the predecessor to the path
            path.Add(backTrack);
        }
        path.Reverse();
        return path;
    }

    /// <summary>
    /// Return true if this is the best known way to reach the given successor.
    /// </summary>
    /// <returns></returns>
    private static bool IsImprovedPath(
        PriorityQueue<float, (int, int)> frontier,
        Dictionary<(int, int), float> visitedNodes,
        (int, int) successor, float totalCost
    ) {
        return !visitedNodes.ContainsKey(successor)
            || (frontier.Contains(successor) && frontier.PriorityOf(successor) > totalCost)
            || (visitedNodes[successor] > totalCost);
    }

    /// <summary>
    /// Update data with the best known way to reach the given successor.
    /// </summary>
    private static void UpdateNodeInventoryWithNewPath(
        PriorityQueue<float, (int, int)> frontier,
        Dictionary<(int, int), float> visitedNodes,
        Dictionary<(int, int), (int, int)> nodesByBestPredecessor,
        (int, int) node, (int, int) successor,
        float totalCost
    ) {
        if (frontier.Contains(successor)) {
            frontier.UpdatePriority(totalCost, successor);
        }
        else {
            frontier.Enqueue(totalCost, successor);
        }
        visitedNodes[successor] = totalCost;
        nodesByBestPredecessor[successor] = node;
    }

    /// <summary>
    /// Return whether this is out of bounds for the given cost array.
    /// </summary>
    private static bool OutOfBounds(int width, int length, (int, int) successor)
    {
        return successor.Item1 < 0 || successor.Item1 >= width
            || successor.Item2 < 0 || successor.Item2 >= length;
    }
    
    /// <summary>
    /// Get the (potential) successors of this node, not accounting for bounds.
    /// </summary>
    private List<(int, int)> GetSuccessors((int, int) node) {
        var x = node.Item1;
        var y = node.Item2;

        // todo this produces detectable artifacts in movement
        return new List<(int, int)> {
            (x+1, y), (x-1, y), (x, y+1), (x, y-1) 
        };
    }
}