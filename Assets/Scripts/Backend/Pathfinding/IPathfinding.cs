using System.Collections.Generic;


public interface IPathfinding {
    // todo wrap these params in an object that defines the pathfinding problem
    List<(int, int)> GetPath(IGameState gameState, int x0, int y0, int x1, int y1, string mode);
}