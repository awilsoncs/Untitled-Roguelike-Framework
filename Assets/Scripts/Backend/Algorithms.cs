
using System;
using System.Collections.Generic;


namespace URFCommon {
    public static class Algorithms {
            private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

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
                Bresenham(
                    start, end,
                    p => {
                        line.Add(p);
                        return true;
                    }
                );
                if (!start.Equals(line[0])) {
                    line.Reverse();
                }

                for(int i = 0; i < line.Count; i++) {
                    bool result = plot(line[i]);
                    if (!result) {break;}
                }
            }

            /// <summary>
            /// Plot the line from (x0, y0) to (x1, y1)
            /// </summary>
            /// <param name="x0">The start x</param>
            /// <param name="y0">The start y</param>
            /// <param name="x1">The end x</param>
            /// <param name="y1">The end y</param>
            /// <param name="plot">The plotting function (if this returns false, the algorithm stops early)</param>
            public static void Bresenham(Position start, Position end, PlotFunction plot)
            {
            (int x0, int y0) = start;
            (int x1, int y1) = end;
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
            if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
            int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

                for (int x = x0; x <= x1; ++x)
                {
                    if (!(steep ? plot((y, x)) : plot((x, y)))) return;
                    err = err - dY;
                    if (err < 0) { y += ystep;  err += dX; }
                }
            }
    }

}
