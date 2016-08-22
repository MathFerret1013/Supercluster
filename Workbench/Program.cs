namespace Workbench
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using Supercluster.MTree;
    using Supercluster.MTree.Tests;
    using Supercluster.Structures;

    class Program
    {
        static void Main(string[] args)
        {

            var points = new double[][]
                      {
                              new double[] { 1, 3 },
                              new double[] { 2, 1 },
                              new double[] { 4, 2 },
                              new double[] { 10, 10 },
                              new double[] { 9, 9 },
                              new double[] { 8, 9 },
                              new double[] { 9.5, 9.5 },
                              new double[] { 9.75, 9.75 }
                      };

            var mtree = new MTree<double[]> { Capacity = 3, Metric = Metrics.L2Norm_Double };
            foreach (var point in points)
            {
                mtree.Add(point);
            }

            var testPoint = new double[] { 5, 5 };


            // linear search
            var linearResults = points.Select(p => new Tuple<double[], double>(p, Metrics.L2Norm_Double(p, testPoint)))
                    .OrderBy(p => p.Item2)
                    .Take(3).ToArray();

            var resultsList = mtree.NearestNeighbors(mtree.Root, testPoint, 3);
        }
    }
}
