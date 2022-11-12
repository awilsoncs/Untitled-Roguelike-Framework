using System.Collections.Generic;
using URF.Common;
using URF.Server.GameState;

namespace URF.Server.FieldOfView {
  /// <summary>
  /// Performs FOV calculation by raycasting to edge tiles.
  /// </summary>
  public class RaycastingFov : IFieldOfView {

    private static Algorithms.PlotFunction GetPlotter(
      IBuildable gameState,
      IDictionary<Position, bool> results
    ) {
      return p => {
        results[p] = true;
        return gameState.GetCell(p).IsTransparent;
      };
    }

    public IFieldOfViewQueryResult CalculateFOV(IGameState gameState, Position pos) {
      var results = new Dictionary<Position, bool>();
      Algorithms.PlotFunction pf = GetPlotter(gameState, results);
      // for each point on the boundary top and bottom
      for(int column = 0; column < gameState.MapWidth; column++) {
        Algorithms.Line(pos, (column, gameState.MapHeight - 1), pf);
        Algorithms.Line(pos, (column, 0), pf);
      }

      for(int row = 0; row < gameState.MapHeight; row++) {
        Algorithms.Line(pos, (0, row), pf);
        Algorithms.Line(pos, (gameState.MapWidth - 1, row), pf);
      }
      // stop when you find something that blocks sight
      // update the query result

      return new FieldOfViewQueryResult(results);
    }

    public bool IsVisible(IGameState gameState, Position start, Position end) {
      var results = new Dictionary<Position, bool>();
      Algorithms.PlotFunction pf = GetPlotter(gameState, results);
      Algorithms.Line(start, end, pf);
      return ExtensionMethods.GetValueOrDefault(results, end, false);
    }

    // todo calculate line of sight

  }
}
