namespace KDTreeTests
{
    using System.Linq;

    using NUnit.Framework;

    using Supercluster.Core;
    using Supercluster.Structures.KDTree;
    using Supercluster.Tests;


    [TestFixture]
    public class KDTreeTests
    {
        [Test]
        public void WikipediaBuildTests()
        {
            // Should generate the following tree:
            //             7,2
            //              |
            //       +------+-----+
            //      5,4          9,6
            //       |            |
            //   +---+---+     +--+
            //  2,3     4,7   8,1 


            var points = new double[][]
                             {
                                     new double[] { 7, 2 }, new double[] { 5, 4 }, new double[] { 2, 3 },
                                     new double[] { 4, 7 }, new double[] { 9, 6 }, new double[] { 8, 1 }
                             };

            var tree = new KDTree<double[]>(2, points, Metrics.L2Norm, d => d);

            Assert.That(tree.Root.Value, Is.EqualTo(points[0]));
            Assert.That(tree.Root.Left.Value, Is.EqualTo(points[1]));
            Assert.That(tree.Root.Left.Left.Value, Is.EqualTo(points[2]));
            Assert.That(tree.Root.Left.Right.Value, Is.EqualTo(points[3]));
            Assert.That(tree.Root.Right.Value, Is.EqualTo(points[4]));
            Assert.That(tree.Root.Right.Left.Value, Is.EqualTo(points[5]));
        }

        [Test]
        public void NearestNeighborTest()
        {
            var dataSize = 10000;
            var testDataSize = 100;
            var range = 1000;

            var treePoints = Utilities.GenerateDoubles(dataSize, range, 2);
            var testData = Utilities.GenerateDoubles(testDataSize, range, 2);

            var tree = new KDTree<double[]>(2, treePoints, Metrics.L2Norm, d => d);

            var neighboors = 100;
            for (var i = 0; i < testDataSize; i++)
            {
                var target = testData[i];
                var treeNearest = tree.NearestNeighbors(target, neighboors).ToArray();

                var linearResults = Utilities.LinearNearestNeighboors(
                    target,
                    neighboors,
                    treePoints,
                    Metrics.L2Norm);

                for (var j = 0; j < linearResults.Length; j++)
                {
                    Assert.That(
                        Metrics.L2Norm(target, linearResults[j]),
                        Is.EqualTo(Metrics.L2Norm(target, treeNearest[j])));
                }
            }
        }

        [Test]
        public void RadialSearchTest()
        {
            var dataSize = 10000;
            var testDataSize = 100;
            var range = 1000;

            var treePoints = Utilities.GenerateDoubles(dataSize, range, 2);
            var testData = Utilities.GenerateDoubles(testDataSize, range, 2);

            var tree = new KDTree<double[]>(2, treePoints, Metrics.L2Norm, d => d);

            var radius = 30;
            for (var i = 0; i < testDataSize; i++)
            {
                var target = testData[i];
                var treeNearest = tree.RadialSearch(target, radius).ToArray();

                var linearResults = Utilities.LinearRadialSearch(target, radius, treePoints, Metrics.L2Norm);

                for (var j = 0; j < linearResults.Length; j++)
                {
                    Assert.That(
                        Metrics.L2Norm(target, linearResults[j]),
                        Is.EqualTo(Metrics.L2Norm(target, treeNearest[j])));
                }
            }
        }


    }
}
