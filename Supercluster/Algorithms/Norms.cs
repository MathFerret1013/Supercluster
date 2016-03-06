using System;

namespace Supercluster.Algorithms
{
    using static System.Math;

    public static class Norms
    {
        public static Func<double[], double[], double> L2Norm = (x, y) =>
            {
                double dist = 0.0;
                for (int i = 0; i < x.Length; i++)
                {
                    dist += Pow(x[i] - y[i], 2);
                }

                return Sqrt(dist);
            };

        public static Func<double[], double[], double> L2Norm_Squared = (x, y) =>
    {
        double dist = 0.0;
        for (int i = 0; i < x.Length; i++)
        {
            dist += Pow(x[i] - y[i], 2);
        }

        return dist;
    };
    }
}
