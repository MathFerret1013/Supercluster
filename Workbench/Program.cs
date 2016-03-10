namespace Workbench
{
    using System;
    using System.Linq;

    using Supercluster.Algorithms;
    using Supercluster.KDTree;

    class Program
    {
        static void Main(string[] args)
        {

            // Generate tree points
            var numOfPoints = 10;
            var pointsInTree = new double[numOfPoints][];
            var random = new Random();
            var range = 1000;

            for (int i = 0; i < numOfPoints; i++)
            {
                pointsInTree[i] = new double[] { random.Next(0, 1000), random.Next(0, 1000) };
            }

            // Generate test points
            var numOfTestPoints = 10000;
            var pointsToTest = new double[numOfTestPoints][];
            for (int i = 0; i < numOfTestPoints; i++)
            {
                pointsToTest[i] = new double[] { random.Next(0, 1000), random.Next(0, 1000) };
            }

            // build tree
            var tree = new KDTree(2, pointsInTree);
            var linear = pointsInTree.ToArray();

            for (int i = 0; i < numOfTestPoints; i++)
            {
                var nearestLinear = GetNearest(linear, pointsToTest[i], Norms.L2Norm);
                var nearestTree = tree.FindNearestNeighbor(pointsToTest[i]);

                if (nearestLinear[0] != nearestTree.Value[0] && nearestLinear[1] != nearestTree.Value[1])
                {
                    Console.WriteLine("Not equal! Index: " + i);
                    Console.WriteLine("Point to test: " + pointsToTest[i][0] + " " + pointsToTest[i][1]);
                    Console.WriteLine("Linear: " + nearestLinear[0] + " " + nearestLinear[1] + "\t dist: " + Norms.L2Norm(pointsToTest[i], nearestLinear));
                    Console.WriteLine("Tree: " + nearestTree.Value[0] + " " + nearestTree.Value[1] + "\t dist: " + Norms.L2Norm(pointsToTest[i], nearestTree.Value));
                    Console.ReadLine();
                }
            }
        }

        static T[] GetNearest<T>(T[][] points, T[] sample, Func<T[], T[], double> metric)
        {
            var dist = double.MaxValue;
            T[] nearestPoint = { };

            for (int i = 0; i < points.Length; i++)
            {
                var currentDist = metric(points[i], sample);
                if (currentDist < dist)
                {
                    dist = currentDist;
                    nearestPoint = points[i];
                }
            }

            return nearestPoint;
        }
    }
}
