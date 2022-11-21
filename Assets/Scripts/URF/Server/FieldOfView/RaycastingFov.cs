namespace URF.Server.FieldOfView {
  using System.Collections.Generic;
  using URF.Common;

  /// <summary>
  /// Field of view implementation using the strategy of raycasting from a given point to the edge
  /// of the map to mark visible cells.
  /// </summary>
  public class RaycastingFov : IFieldOfView {

    // Marks results true or false based on game state position transparency.
    private static Algorithms.PlotFunction GetPlotter(
      bool[,] transparency,
      IDictionary<Position, bool> results
    ) {
      if (transparency == null) {
        return p => false;
      }
      return p => {
        results[p] = true;
        return transparency[p.X, p.Y];
      };
    }

    /// <inheritdoc />
    public IFieldOfViewQueryResult CalculateFov(bool[,] transparency, Position pos) {
      var results = new Dictionary<Position, bool>();
      if (transparency == null) {
        return new FieldOfViewQueryResult(results);
      }

      Algorithms.PlotFunction pf = GetPlotter(transparency, results);
      int width = transparency.GetLength(0);
      int height = transparency.GetLength(1);

      for (int column = 0; column < width; column++) {
        Algorithms.Line(pos, (column, height - 1), pf);
        Algorithms.Line(pos, (column, 0), pf);
      }

      for (int row = 1; row < height - 1; row++) {
        Algorithms.Line(pos, (0, row), pf);
        Algorithms.Line(pos, (width - 1, row), pf);
      }

      return new FieldOfViewQueryResult(results);
    }

    /// <inheritdoc />
    public bool IsVisible(bool[,] transparency, Position start, Position end) {
      var results = new Dictionary<Position, bool>();
      Algorithms.PlotFunction pf = GetPlotter(transparency, results);
      Algorithms.Line(start, end, pf);
      return results.GetValueOrDefault(end);
    }

  }
}
