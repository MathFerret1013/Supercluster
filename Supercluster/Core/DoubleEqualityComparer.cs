namespace Supercluster
{
    using System;
    using System.Collections.Generic;

    using Supercluster.Core;

    public class DoubleEqualityComparer : IEqualityComparer<double>
    {
        public bool Equals(double x, double y)
        {
            return Math.Abs(x - y) <= 1E-13;
        }

        public int GetHashCode(double obj)
        {
            return base.GetHashCode();
        }
    }

    public class DoubleArrayEqualityComparer : IEqualityComparer<double[]>
    {
        public bool Equals(double[] x, double[] y)
        {
            // check count
            if (x.Length != y.Length)
            {
                return false;
            }

            // check elementwise
            for (int i = 0; i < x.Length; i++)
            {
                if (Math.Abs(x[i] - y[i]) > SuperclusterContants.DOUBLE_TOLERANCE)
                {
                    return false;
                }
            }

            // have same count and elements match
            return true;
        }

        public int GetHashCode(double[] obj)
        {
            return base.GetHashCode();
        }
    }
}