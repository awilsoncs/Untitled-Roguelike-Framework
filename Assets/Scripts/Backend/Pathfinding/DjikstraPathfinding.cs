using System.Collections.Generic;

public class DjikstraPathfinding : IPathfinding
{
    public List<(int, int)> GetPath(in float[][] costs, (int, int) start, (int, int) end)
    {
        // http://www.roguebasin.com/index.php/Pathfinding
        // generate a cost map
        var frontier = new PriorityQueue<float, (int, int)>(); // P
        var visitedNodes = new Dictionary<(int, int), float>(); // V
        var path = new List<(int, int)>();
        frontier.Enqueue(costs[start.Item1][start.Item2], start);
        while (frontier.Count > 0) {
            var node = frontier.Dequeue();
            if (node.Equals(end)) {
                path.Add(node);
                return path;
            }

            // calculate successors
            foreach (var successor in GetSuccessors(node)) {
                // check bounds
                // calculate totalCost(nx) = (totalCost(x) + cost(nx))
                float totalCost = 0f;
                // if (
                //     !visitedNodes.ContainsKey(successor)
                //     || (
                //         frontier.Contains(successor)
                //         && frontier.PriorityOf(successor) > totalCost)
                //     || (visitedNodes[successor] > totalCost)
                // ) {

                // }
            }
        }

        return new List<(int, int)>();
    }

    private List<(int, int)> GetSuccessors((int, int) node) {
        var x = node.Item1;
        var y = node.Item2;
        return new List<(int, int)> {
            (x+1, y), (x-1, y), (x, y+1), (x, y-1) 
        };
    }
}