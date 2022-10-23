using System.Collections.Generic;


/// <summary>
/// Performs FOV calculation by raycasting to edge tiles.
/// </summary>
public class RaycastingFOV : IFieldOfView {
    public IFieldOfViewQueryResult CalculateFOV(IGameState gameState, int x0, int y0) {
        var results = new Dictionary<(int, int), bool>();
        Algorithms.PlotFunction pf = (int x, int y) => {
            if (gameState.GetCell(x, y).IsTransparent) {
                results[(x, y)] = true;
                return true;
            }
            return false;
        };

        // for each point on the boundary top and bottom
        for (int x1 = 0; x1 < gameState.MapWidth; x1++) {
            Algorithms.Line(x1, 0, x0, y0, pf);
            Algorithms.Line(x1, gameState.MapHeight-1, x0, y0, pf);
        }
        for (int y1 = 0; y1 < gameState.MapHeight; y1++) {
            Algorithms.Line(0, y1, x0, y0, pf);
            Algorithms.Line(gameState.MapWidth - 1, y1, x0, y0, pf);
        }
            // stop when you find something that blocks sight
            // update the query result

        return new FieldOfViewQueryResult(results);
    }

    // todo calculate line of sight
    // todo calculate boolean InLineOfSight
}