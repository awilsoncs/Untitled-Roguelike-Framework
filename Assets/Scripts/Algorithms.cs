
using System;
using System.Collections.Generic;


public static class Algorithms {
        private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

        /// <summary>
        /// The plot function delegate
        /// </summary>
        /// <param name="x">The x co-ord being plotted</param>
        /// <param name="y">The y co-ord being plotted</param>
        /// <returns>True to continue, false to stop the algorithm</returns>
        public delegate bool PlotFunction(int x, int y);

        /// <summary>
        /// Runs Bresenham's line algorithm, reverse corrected.
        /// </summary>
        public static void Line(int x0, int y0, int x1, int y1, PlotFunction plot) {
            // Explanation: in cases where the start is to the right of the end,
            // Bresenham's algorithm returns the line from end to start.
            // todo can we just fix the bresenham algorithm to not reverse?
            var line = new List<(int, int)>();
            Bresenham(
                x0, y0, x1, y1,
                (x, y) => {
                    line.Add((x, y));
                    return true;
                }
            );
            if (!(x0, y0).Equals(line[0])) {
                line.Reverse();
            }

            for(int i = 0; i < line.Count; i++) {
                bool result = plot(line[i].Item1, line[i].Item2);
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
        public static void Bresenham(int x0, int y0, int x1, int y1, PlotFunction plot)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
            if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
            int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

            for (int x = x0; x <= x1; ++x)
            {
                if (!(steep ? plot(y, x) : plot(x, y))) return;
                err = err - dY;
                if (err < 0) { y += ystep;  err += dX; }
            }
        }
}