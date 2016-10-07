namespace Supercluster.VisualTests
{
    using System;
    using System.Collections.Generic;

    public static class Utilities
    {
        #region Metrics

        public static Func<double[], double[], double> L2Norm = (x, y) =>
            {
                double dist = 0;
                for (int i = 0; i < x.Length; i++)
                {
                    dist += (x[i] - y[i]) * (x[i] - y[i]);
                }

                return Math.Sqrt(dist);
            };


        public static Func<double[], double[], double> Taxicab_Double = (x, y) =>
            {
                double dist = 0d;
                for (int i = 0; i < x.Length; i++)
                {
                    dist += Math.Abs(x[i] - y[i]);
                }

                return dist;
            };

        #endregion

        #region Data Generation

        public static IEnumerable<double[]> GenerateDoubles(int points, double range, int dimensions)
        {
            var data = new List<double[]>();
            var random = new Random();

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

        public static IEnumerable<double[]> GenerateDoubles(int points, double range)
        {
            var data = new List<double[]>();
            var random = new Random();

            for (int i = 0; i < points; i++)
            {
                data.Add(new double[] { (random.NextDouble() * range), (random.NextDouble() * range) });
            }

            return data.ToArray();
        }

        public static float[][] GenerateFloats(int points, double range)
        {
            var data = new List<float[]>();
            var random = new Random();

            for (int i = 0; i < points; i++)
            {
                data.Add(new float[] { (float)(random.NextDouble() * range), (float)(random.NextDouble() * range) });
            }

            return data.ToArray();
        }

        public static float[][] GenerateFloats(int points, double range, int dimensions)
        {
            var data = new List<float[]>();
            var random = new Random();

            for (var i = 0; i < points; i++)
            {
                var array = new float[dimensions];
                for (var j = 0; j < dimensions; j++)
                {
                    array[j] = (float)(random.NextDouble() * range);
                }
                data.Add(array);
            }

            return data.ToArray();
        }

        #endregion

    }
}
