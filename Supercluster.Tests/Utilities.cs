namespace Supercluster.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Utilities
    {
        private static readonly Random random = new Random();

        #region Data Generation

        public static double[][] GenerateDoubles(int points, double range, int dimensions)
        {
            var data = new List<double[]>();

            for (var i = 0; i < points; i++)
            {
                var array = new double[dimensions];
                for (var j = 0; j < dimensions; j++)
                {
                    array[j] = random.NextDouble() * range;
                }
                data.Add(array);
            }

            return data.ToArray();
        }

        #endregion

        #region Searches

        public static T[] LinearNearestNeighboors<T>(T target, int neighboors, IEnumerable<T> dataset, Func<T, T, double> metric)
        {
            return dataset.Select(p => new { Distance = metric(p, target), Point = p })
                    .OrderBy(p => p.Distance)
                    .Take(neighboors)
                    .Select(p => p.Point)
                    .ToArray(); // Must call .ToArray() to force an evaluation.
        }

        public static T[] LinearRadialSearch<T>(T center, double radius, IEnumerable<T> dataset, Func<T, T, double> metric)
        {
            return dataset.Select(p => new { Distance = metric(p, center), Point = p })
                    .Where(t => t.Distance <= radius)
                    .OrderBy(p => p.Distance)
                    .Select(p => p.Point)
                    .ToArray(); // Must call .ToArray() to force an evaluation.
        }

        #endregion
    }
}
