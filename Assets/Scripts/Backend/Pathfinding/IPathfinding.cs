using System.Collections.Generic;


public interface IPathfinding {
    // note: this interface is subject to change as the framework evolves,
    // pathfinding is rarely a simple use case of reaching another entity,
    // and often different systems want to leverage pathfinding for different
    // reasons.
    List<(int, int)> GetPath(in float[][] costs, (int, int) start, (int, int) end);
}