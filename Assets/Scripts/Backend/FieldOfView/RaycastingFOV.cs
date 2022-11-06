using System.Collections.Generic;
using URFCommon;

/// <summary>
/// Performs FOV calculation by raycasting to edge tiles.
/// </summary>
public class RaycastingFOV : IFieldOfView {
    // todo this is quite slow and should eventually be replaced with a shadowcasting
    public IFieldOfViewQueryResult CalculateFOV(IGameState gameState, Position pos) {
        var results = new Dictionary<Position, bool>();
        Algorithms.PlotFunction pf = (Position p) => {
            results[p] = true;
            return gameState.GetCell(p).IsTransparent;
        };

        // for each point on the boundary top and bottom
        for (int column = 0; column < gameState.MapWidth; column++) {
            Algorithms.Line(pos, (column, gameState.MapHeight-1), pf);
            Algorithms.Line(pos, (column, 0), pf);
        }
        for (int row = 0; row < gameState.MapHeight; row++) {
            Algorithms.Line(pos, (0, row), pf);
            Algorithms.Line(pos, (gameState.MapWidth - 1, row), pf);
        }
            // stop when you find something that blocks sight
            // update the query result

        return new FieldOfViewQueryResult(results);
    }

    public bool IsVisible(IGameState gameState, Position start, Position end)
    {
        var results = new Dictionary<Position, bool>();
        bool pf(Position p)
        {
            results[p] = true;
            return gameState.GetCell(p).IsTransparent;
        }
        Algorithms.Line(start, end, pf);
        return results.GetValueOrDefault(end, false);
    }

    // todo calculate line of sight
}