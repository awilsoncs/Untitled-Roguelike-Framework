using System.Collections.Generic;

/// <summary>
/// Performs FOV calculation by raycasting to edge tiles.
/// </summary>
public class RaycastingFOV : IFieldOfView {
    // todo this is quite slow and should eventually be replaced with a shadowcasting
    public IFieldOfViewQueryResult CalculateFOV(IGameState gameState, int x0, int y0) {
        var results = new Dictionary<(int, int), bool>();
        Algorithms.PlotFunction pf = (int x, int y) => {
            results[(x, y)] = true;
            return gameState.GetCell(x, y).IsTransparent;
        };

        // for each point on the boundary top and bottom
        for (int column = 0; column < gameState.MapWidth; column++) {
            Algorithms.Line(x0, y0, column, gameState.MapHeight-1, pf);
            Algorithms.Line(x0, y0, column, 0, pf);
        }
        for (int row = 0; row < gameState.MapHeight; row++) {
            Algorithms.Line(x0, y0, 0, row, pf);
            Algorithms.Line(x0, y0, gameState.MapWidth - 1, row, pf);
        }
            // stop when you find something that blocks sight
            // update the query result

        return new FieldOfViewQueryResult(results);
    }

    public bool IsVisible(IGameState gameState, (int, int) start, (int, int) end)
    {
        var results = new Dictionary<(int, int), bool>();
        Algorithms.PlotFunction pf = (int x, int y) => {
            results[(x, y)] = true;
            return gameState.GetCell(x, y).IsTransparent;
        };
        Algorithms.Line(start.Item1, start.Item2, end.Item1, end.Item2, pf);
        return results.GetValueOrDefault(end);
    }

    // todo calculate line of sight
}