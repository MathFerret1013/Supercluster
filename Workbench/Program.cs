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

            #region ERROR TEST

            // Generate test points
            
            var numOfTestPoints = 1000;
            var testData = new double[numOfTestPoints * numOfTestPoints][];
            var index = 0;
            for (int i = 0; i < numOfTestPoints; i++)
            {
                for (int j = 0; j < numOfTestPoints; j++)
                {
                    testData[index] = new double[] { i, j };
                    index++;
                }

            }


            #endregion


            /*
            var treeData = new double[][]
                               {
                                   new double[] { 493, 878 },
                                   new double[] { 918, 809 },
                                   new double[] { 838, 454 },
                                   new double[] { 984, 939 },
                                   new double[] { 616, 482 },
                                   new double[] { 150, 307 },
                                   new double[] { 387, 144 },
                                   new double[] { 280, 262 },
                                   new double[] { 455, 870 },
                                   new double[] { 379, 429 }
                               };

            
            var testData = new double[][]
                               {
                                   new double[] { 456, 289 }
                               };*/
            
            // build tree

            for (int k = 0; k < 100; k++)
            {
                var data = GenerateRandomPoint(100);
                var tree = new KDTree(2, data);
                var linear = data.ToArray();

                for (int i = 0; i < testData.Length; i++)
                {
                    var nearestLinear = GetNearest(linear, testData[i], Norms.L2Norm);
                    var nearestTree = tree.FindNearestNeighbor(testData[i]);

                    if (Norms.L2Norm(testData[i], nearestLinear) != Norms.L2Norm(testData[i], nearestTree.Value))
                    {
                        Console.WriteLine("Not equal! Index: " + i);
                        Console.WriteLine("Point to test: " + testData[i][0] + " " + testData[i][1]);
                        Console.WriteLine(
                            "Linear: " + nearestLinear[0] + " " + nearestLinear[1] + "\t dist: "
                            + Norms.L2Norm(testData[i], nearestLinear));
                        Console.WriteLine(
                            "Tree: " + nearestTree.Value[0] + " " + nearestTree.Value[1] + "\t dist: "
                            + Norms.L2Norm(testData[i], nearestTree.Value));
                        Console.ReadLine();

                    }
                }

                Console.WriteLine("Finished run: " + k);
            }




            Console.WriteLine("Done!");
            Console.ReadLine();

        }




        static double[][] GenerateRandomPoint(int points)
        {
            // Generate tree points
            var numOfPoints = 10;

            var random = new Random();
            var range = 1000;


            var treeData = new double[numOfPoints][];
            for (int i = 0; i < numOfPoints; i++)
            {
                treeData[i] = new double[] { random.Next(0, 1000), random.Next(0, 1000) };
            }

            return treeData;

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
