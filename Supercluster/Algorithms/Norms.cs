using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
namespace Supercluster.Algorithms
{
    public static class Norms
    {

        public static Func<double[], double[], double> L2Norm = (double[] x, double[] y) =>
            {
                var dist = 0.0;
                for (int i = 0; i < x.Length; i++)
                {
                    dist += Pow(x[i] - y[i], 2);
                }

                return Sqrt(dist);
            };

        public static Func<double[], double[], double> L2Norm_Squared = (double[] x, double[] y) =>
        {
            var dist = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                dist += Pow(x[i] - y[i], 2);
            }

            return dist;
        };


    }
}
