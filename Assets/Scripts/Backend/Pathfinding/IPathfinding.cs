using System.Collections.Generic;


public interface IPathfinding {
    // note: this interface is subject to change as the framework evolves,
    // pathfinding is rarely a simple use case of reaching another entity,
    // and often different systems want to leverage pathfinding for different
    // reasons.
    List<(int, int)> GetPath(IGameState gameState, int x0, int y0, int x1, int y1);
}