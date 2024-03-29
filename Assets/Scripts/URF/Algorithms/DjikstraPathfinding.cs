namespace URF.Algorithms {
  using System.Collections.Generic;
  using URF.Common;
  using URF.Common.PriorityQueue;

  /// <summary>
  /// Pathfinding implemented using Djikstra's Algorithm.
  /// </summary>
  public class DjikstraPathfinding : IPathfinding {

    public List<Position> GetPath(in float[][] costs, Position start, Position end) {
      // http://www.roguebasin.com/index.php/Pathfinding
      // generate a cost map
      var frontier = new PriorityQueue<float, Position>(); // P
      // contains visited nodes and the lowest known cost to reach them
      var visitedNodes = new Dictionary<Position, float>(); // V
      var nodesByBestPredecessor = new Dictionary<Position, Position>();

      frontier.Enqueue(costs[start.X][start.Y], start);
      visitedNodes[start] = 0f;
      while (frontier.Count > 0) {
        Position node = frontier.Dequeue();
        if (node.Equals(end)) {
          return UnwindBestPath(nodesByBestPredecessor, node);
        }
        float nodeCost = visitedNodes[node];
        foreach (Position successor in GetSuccessors(node)) {
          if (OutOfBounds(costs.Length, costs[0].Length, successor)) {
            continue;
          }

          float totalCost = nodeCost + costs[successor.X][successor.Y];
          if (IsImprovedPath(frontier, visitedNodes, successor, totalCost)) {
            UpdateNodeInventoryWithNewPath(frontier, visitedNodes, nodesByBestPredecessor, node,
              successor, totalCost);
          }
        }
      }

      // Empty list indicates no path could be found.
      return new List<Position>();
    }

    /// <summary>
    /// Work backwards through the best predecessors to find the optimal path.
    /// </summary>
    private static List<Position> UnwindBestPath(
      IReadOnlyDictionary<Position, Position> nodesByBestPredecessor,
      Position node
    ) {
      Position backTrack = node;
      var path = new List<Position> { node };
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
      PriorityQueue<float, Position> frontier,
      IReadOnlyDictionary<Position, float> visitedNodes,
      Position successor,
      float totalCost
    ) {
      return !visitedNodes.ContainsKey(successor) ||
        (frontier.Contains(successor) && frontier.PriorityOf(successor) > totalCost) ||
        visitedNodes[successor] > totalCost;
    }

    /// <summary>
    /// Update data with the best known way to reach the given successor.
    /// </summary>
    private static void UpdateNodeInventoryWithNewPath(
      PriorityQueue<float, Position> frontier,
      IDictionary<Position, float> visitedNodes,
      IDictionary<Position, Position> nodesByBestPredecessor,
      Position node,
      Position successor,
      float totalCost
    ) {
      if (frontier.Contains(successor)) {
        frontier.UpdatePriority(totalCost, successor);
      } else {
        frontier.Enqueue(totalCost, successor);
      }
      visitedNodes[successor] = totalCost;
      nodesByBestPredecessor[successor] = node;
    }

    /// <summary>
    /// Return whether this is out of bounds for the given cost array.
    /// </summary>
    private static bool OutOfBounds(int width, int length, Position successor) {
      return successor.X < 0 || successor.X >= width || successor.Y < 0 || successor.Y >= length;
    }

    /// <summary>
    /// Get the (potential) successors of this node, not accounting for bounds.
    /// </summary>
    private static List<Position> GetSuccessors(Position node) {
      int x = node.X;
      int y = node.Y;

      // todo this produces detectable artifacts in movement
      return new List<Position> { (x + 1, y), (x - 1, y), (x, y + 1), (x, y - 1) };
    }

  }
}
