namespace Supercluster_Tests
{
    using System;

    using NUnit.Framework;

    using Supercluster.Algorithms;

    [TestFixture]
    public class MostDistantPointsTests
    {
        [Test]
        public void Test()
        {
            Func<double[], double[], double> Metric = (x, y) =>
            {
                double sumOfSquaredDifference = 0;
                for (int i = 0; i < x.Length; i++)
                {
                    sumOfSquaredDifference += Math.Pow(x[i] - y[i], 2);
                }

                return Math.Sqrt(sumOfSquaredDifference);
            };

            var data = new double[][]
                           {
                               new[] { 2.0, 3.0 }, new[] { 9.0, 3.0 }, new[] { 4.0, 4.0 }, new[] { 1.0, 1.0 },
                               new[] { 2.0, 2.0 }, new[] { 7.0, 0.0 }, new[] { 4.0, 3.0 }, new[] { 10.0, 5.0 },
                           };

            MostDistantPoints.MaximallyDistantPoints(data, 3, Metric);

        }
    }
}
