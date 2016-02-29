using System;
using System.Linq;

namespace Supercluster_Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;

    using Supercluster;
    using Supercluster.Classification;

    [TestFixture]
    public class ExtendedNearestNeighborsTests
    {
        // References
        // [1] Bo Tang; Haibo He, "ENN: Extended Nearest Neighbor Method for Pattern Recognition [Research Frontier]," in Computational Intelligence Magazine, IEEE , vol.10, no.3, pp.52-60, Aug. 2015 doi: 10.1109/MCI.2015.2437512


        /// <summary>
        /// Tests the building of the weighted "knn map" as described in [1]
        /// Correctness of the map is compared with results of the MatLab output from the code and data sets provided by Tang and He's source code.
        /// </summary>
        [Test]
        public void WeightedMapTest()
        {
            var enn = new ExtendedNearestNeighbors<double[]>(3, 2);
            enn.Metric = (x, y) =>
                {
                    double sumOfSquaredDifference = 0;
                    for (int i = 0; i < x.Length; i++)
                    {
                        sumOfSquaredDifference += Math.Pow(x[i] - y[i], 2);
                    }

                    return Math.Sqrt(sumOfSquaredDifference);
                };

            double[][] trainingData;
            int[] trainingLabels;

            // load the training data provided by the authors of [1]
            using (var fs = new FileStream(@"..\..\data\ENN\TrainingData.dat", FileMode.Open))
            {
                trainingData = (double[][])(new BinaryFormatter().Deserialize(fs));
            }

            using (var fs = new FileStream(@"..\..\data\ENN\TrainingLabel.dat", FileMode.Open))
            {
                trainingLabels = (int[])(new BinaryFormatter().Deserialize(fs));
            }
            trainingLabels = trainingLabels.Select(l => l == 1 ? 0 : 1).ToArray();

            double[][] expectedKNNDistances;
            int[][] expectedKNNLabels;
            double[] expectedClassStatistics;

            // load the expected outputs
            using (var fs = new FileStream(@"..\..\data\ENN\weightedKNNDistance.dat", FileMode.Open))
            {
                expectedKNNDistances = (double[][])(new BinaryFormatter().Deserialize(fs));
            }

            using (var fs = new FileStream(@"..\..\data\ENN\weightedKNNLabel.dat", FileMode.Open))
            {
                expectedKNNLabels = (int[][])(new BinaryFormatter().Deserialize(fs));
            }

            using (var fs = new FileStream(@"..\..\data\ENN\TSOri.dat", FileMode.Open))
            {
                expectedClassStatistics = (double[])(new BinaryFormatter().Deserialize(fs));
            }


            // fix class labels
            for (int i = 0; i < expectedKNNLabels.Length; i++)
            {
                for (int j = 0; j < expectedKNNLabels[i].Length; j++)
                {
                    if (expectedKNNLabels[i][j] == 1)
                    {
                        expectedKNNLabels[i][j] = 0;
                    }
                    if (expectedKNNLabels[i][j] == 2)
                    {
                        expectedKNNLabels[i][j] = 1;
                    }
                }
            }


            // Use out implementation to calculate the  the distance and label maps
            enn.Train(trainingData, trainingLabels);

            // Get the data from the prvate fields

            var WeightedKNNDistance = (double[][])enn.GetType().GetField("WeightedKNNDistance", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(enn);
            var WeightedKNNLabels = (int[][])enn.GetType().GetField("WeightedKNNLabels", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(enn);
            var ClassStatistics = (double[])enn.GetType().GetField("ClassStatistics", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(enn);

            // Check equality of length
            Assert.That(WeightedKNNDistance.Length, Is.EqualTo(expectedKNNDistances.Length));
            Assert.That(WeightedKNNLabels.Length, Is.EqualTo(expectedKNNLabels.Length));
            Assert.That(ClassStatistics.Length, Is.EqualTo(expectedClassStatistics.Length));

            // Check equality of content
            for (int i = 0; i < WeightedKNNDistance.Length; i++)
            {
                Assert.That(WeightedKNNDistance[i].SequenceEqual(expectedKNNDistances[i], new DoubleEqualityComparer(1)));
            }

            // Check equality of content
            for (int i = 0; i < WeightedKNNLabels.Length; i++)
            {
                Assert.That(WeightedKNNLabels[i].SequenceEqual(expectedKNNLabels[i]));
            }

            // Check equality of content
            for (int i = 0; i < ClassStatistics.Length; i++)
            {
                Assert.That(ClassStatistics.SequenceEqual(expectedClassStatistics, new DoubleEqualityComparer(1E-3)));
            }

        }

        [Test]
        public void PredictionTest()
        {
            var enn = new ExtendedNearestNeighbors<double[]>(k: 3, clusters: 2);
            enn.Metric = (x, y) =>
            {
                double sumOfSquaredDifference = 0;
                for (int i = 0; i < x.Length; i++)
                {
                    sumOfSquaredDifference += Math.Pow(x[i] - y[i], 2);
                }

                return Math.Sqrt(sumOfSquaredDifference);
            };

            double[][] trainingData;
            double[][] testingData;
            int[] trainingLabels;
            int[] testingLabels;
            int[] PredictionLabels;
            // load the training data provided by the authors of [1]
            using (var fs = new FileStream(@"..\..\data\ENN\TrainingData.dat", FileMode.Open))
            {
                trainingData = (double[][])(new BinaryFormatter().Deserialize(fs));
            }

            using (var fs = new FileStream(@"..\..\data\ENN\TrainingLabel.dat", FileMode.Open))
            {
                trainingLabels = (int[])(new BinaryFormatter().Deserialize(fs));
            }
            trainingLabels = trainingLabels.BijectWithNaturals();
            using (var fs = new FileStream(@"..\..\data\ENN\TestingData.dat", FileMode.Open))
            {
                testingData = (double[][])(new BinaryFormatter().Deserialize(fs));
            }
            using (var fs = new FileStream(@"..\..\data\ENN\TestingLabel.dat", FileMode.Open))
            {
                testingLabels = (int[])(new BinaryFormatter().Deserialize(fs));
            }
            testingLabels = trainingLabels.BijectWithNaturals();

            using (var fs = new FileStream(@"..\..\data\ENN\PredictionLabel.dat", FileMode.Open))
            {
                PredictionLabels = (int[])(new BinaryFormatter().Deserialize(fs));
            }
            PredictionLabels = PredictionLabels.BijectWithNaturals();


            // Use out implementation to calculate the  the distance and label maps
            enn.Train(trainingData, trainingLabels);
            var predictedLabels = testingData.Select(t => enn.Compute(t)).ToArray();

            Assert.That(PredictionLabels.SequenceEqual(predictedLabels));
        }


        public class DoubleEqualityComparer : IEqualityComparer<double>
        {
            private readonly double tolerance;
            public DoubleEqualityComparer(double tolerance)
            {
                this.tolerance = tolerance;
            }

            public bool Equals(double x, double y)
            {
                return Math.Abs(x - y) <= this.tolerance;
            }

            public int GetHashCode(double obj)
            {
                return base.GetHashCode();
            }
        }

    }
}
