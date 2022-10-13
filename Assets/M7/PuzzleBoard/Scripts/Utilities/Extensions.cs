/*
 * Extensions.cs
 * Author: Cristjan Lazar
 * Date: Oct 26, 2018
 */

using System.Collections.Generic;
using System.Linq;

using Gamelogic.Grids;
using System.Security.Cryptography;
using System;

namespace M7.Match {

    /// <summary>
    /// Utilities for MatchGrid.
    /// </summary>
    public static class Extensions {

        /// <summary>
        /// Linq Distinct comparer for comparing list of rectpoints.
        /// </summary>
        public class RectpointListComparer : IEqualityComparer<PointList<RectPoint>> {
            public bool Equals(PointList<RectPoint> pointsA, PointList<RectPoint> pointsB) {
                foreach (var point in pointsA) {
                    if (pointsA.Any(p => p.Equals(point)))
                        continue;

                    return false;
                }

                return true;
            }

            public int GetHashCode(PointList<RectPoint> obj) {
                int sum = 0;

                // Hash would be of form XXYY
                // Ex. RectPoint(02, 61) hashed is 0362.
                // Only reliable if grid is limited to 98 x 98
                // + 1 to account for X = 0 or Y = 0
                foreach (var p in obj) {
                    sum += (p.X + 1) * 100 + (p.Y + 1);
                }

                return sum;
            }
        }

        /// <summary>
        /// Find the lowest empty rectpoint in a given column starting from a given rectpoint.
        /// </summary>
        public static RectPoint FindFloor<MatchGridCell>(this RectGrid<MatchGridCell> grid, RectPoint point, Func<MatchGridCell, bool> ingnoreCondition = null, Func<MatchGridCell, bool> stopCondition = null)
        {
            var possibleFloor = new List<RectPoint>();
            possibleFloor.Add(point);
            for (var y = point.Y; y >= 0; y--)
            {
                var floor = new RectPoint(point.X, y);
                var cell = grid[floor];
                if (cell != null)
                {
                    if (stopCondition?.Invoke(cell) ?? false)
                        break;
                    else if (!(ingnoreCondition?.Invoke(cell) ?? false))
                        possibleFloor.Add(floor);
                }
            }

            return possibleFloor.FirstOrDefault();
        }

        /// <summary>
        /// Shuffle a list. Based on Fisher-Yates algortihm.
        /// </summary>
        public static IList<T> Shuffle<T> (this IList<T> list) {
            var count = list.Count;
            var last = count - 1;
            for (var i = 0; i < last; i++) {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }

            return list;
        }

        /// <summary>
        /// Shuffles a list. This is in here because observations of omni tile spawn locations showed that they were not "random" enoughh.
        /// Slower than the other shuffle function. Might be overkill for most applications. Based on Fisher-Yates algorithm.
        /// </summary>
        public static IList<T> BetterShuffle<T> (this IList<T> list) {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1) {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        private static System.Random rng = new System.Random();

        public static void ShuffleSystemRNG<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}

