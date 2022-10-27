using System.Collections.Generic;

public class DjikstraPathfinding : IPathfinding
{
    public List<(int, int)> GetPath(IGameState gameState, int x0, int y0, int x1, int y1)
    {
        // http://www.roguebasin.com/index.php/Pathfinding
        // generate a cost map
        var frontier = new PriorityQueue<(int, int), float>(); // P
        var visitedNodes = new HashSet<(int, int)>(); // V

        return new List<(int, int)>();
    }
}