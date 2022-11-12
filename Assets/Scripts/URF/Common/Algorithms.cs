using System;
using System.Collections.Generic;

namespace URF.Common {
  public static class Algorithms {

    private static void Swap<T>(ref T lhs, ref T rhs) {
      (lhs, rhs) = (rhs, lhs);
    }

    /// <summary>
    /// The plot function delegate
    /// </summary>
    /// <param name="p">The coordinate being plotted</param>
    /// <returns>True to continue, false to stop the algorithm</returns>
    public delegate bool PlotFunction(Position p);

    /// <summary>
    /// Runs Bresenham's line algorithm, reverse corrected.
    /// </summary>
    public static void Line(Position start, Position end, PlotFunction plot) {
      // Explanation: in cases where the start is to the right of the end,
      // Bresenham's algorithm returns the line from end to start.
      // todo can we just fix the bresenham algorithm to not reverse?
      var line = new List<Position>();
      Bresenham(start, end, p => {
        line.Add(p);
        return true;
      });
      if(!start.Equals(line[0])) { line.Reverse(); }

      foreach(Position t in line) {
        bool result = plot(t);
        if(!result) { break; }
      }
    }

    private static void Bresenham(Position start, Position end, PlotFunction plot) {
      (int x0, int y0) = start;
      (int x1, int y1) = end;
      bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
      if(steep) {
        Swap(ref x0, ref y0);
        Swap(ref x1, ref y1);
      }
      if(x0 > x1) {
        Swap(ref x0, ref x1);
        Swap(ref y0, ref y1);
      }
      int dX = (x1 - x0),
        dY = Math.Abs(y1 - y0),
        err = (dX / 2),
        ystep = (y0 < y1 ? 1 : -1),
        y = y0;

      for(int x = x0; x <= x1; ++x) {
        if(!(steep ? plot((y, x)) : plot((x, y)))) return;
        err = err - dY;
        if(err >= 0) { continue; }
        y += ystep;
        err += dX;
      }
    }

  }
}
