namespace URF.Algorithms {
  using System;
  using System.Collections.Generic;
  using URF.Common;

  public static class Lines {

    /// <summary>
    /// The plot function delegate
    /// </summary>
    /// <param name="p">The coordinate being plotted</param>
    /// <returns>True to continue, false to stop the algorithm</returns>
    public delegate bool PlotFunction(Position p);

    private static void Swap<T>(ref T lhs, ref T rhs) {
      (lhs, rhs) = (rhs, lhs);
    }

    /// <summary>
    /// Runs Bresenham's line algorithm, reverse corrected.
    /// </summary>
    public static void Plot(Position start, Position end, PlotFunction plotFunction) {
      // Explanation: in cases where the start is to the right of the end,
      // Bresenham's algorithm returns the line from end to start.
      // todo can we just fix the bresenham algorithm to not reverse?
      var line = new List<Position>();
      Bresenham(start, end, p => {
        line.Add(p);
        return true;
      });
      if (!start.Equals(line[0])) {
        line.Reverse();
      }

      foreach (Position t in line) {
        bool result = plotFunction(t);
        if (!result) {
          return;
        }
      }
    }

    private static void Bresenham(Position start, Position end, PlotFunction plotFunction) {
      // Clean up input before we perform the  calculation.
      (int x0, int y0) = start;
      (int x1, int y1) = end;
      bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

      if (steep) {
        Swap(ref x0, ref y0);
        Swap(ref x1, ref y1);
      }

      if (x0 > x1) {
        Swap(ref x0, ref x1);
        Swap(ref y0, ref y1);
      }

      CalculateBresenham(plotFunction, (x0, y0), (x1, y1), steep);
    }

    private static void CalculateBresenham(
      PlotFunction plotFunction,
      Position start,
      Position end,
      bool steep
    ) {

      (int x0, int y0) = start;
      (int x1, int y1) = end;

      int dX = x1 - x0;
      int dY = Math.Abs(y1 - y0);
      int err = dX / 2;
      int ystep = y0 < y1 ? 1 : -1;
      int y = y0;

      for (int x = x0; x <= x1; ++x) {
        if (!(steep ? plotFunction((y, x)) : plotFunction((x, y)))) {
          return;
        }

        err -= dY;
        if (err >= 0) {
          continue;
        }
        y += ystep;
        err += dX;
      }
    }
  }
}
