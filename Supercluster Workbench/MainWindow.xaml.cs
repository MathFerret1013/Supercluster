using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace Supercluster_Workbench
{
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using OxyPlot.Wpf;

    using Supercluster;
    using Supercluster.Algorithms;
    using Supercluster.Classification;
    using Supercluster.Clustering;

    using ScatterSeries = OxyPlot.Wpf.ScatterSeries;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ZigguratGaussianSampler ziggurat = new ZigguratGaussianSampler();
        public MainWindow()
        {
            InitializeComponent();
            this.MaximallyDistantPointsTest();

        }


        public void MaximallyDistantPointsTest()
        {
            // Set up plot
            var originalPlot = new PlotView { Model = new PlotModel() };
            this.OriginalData.Children.Add(originalPlot);
            originalPlot.Model.PlotType = PlotType.XY;
            originalPlot.Model.Background = OxyColor.FromRgb(255, 255, 255);
            originalPlot.Model.TextColor = OxyColor.FromRgb(0, 0, 0);


            Func<double[], double[], double> Metric = (x, y) =>
            {
                double sumOfSquaredDifference = 0;
                for (int i = 0; i < x.Length; i++)
                {
                    sumOfSquaredDifference += Math.Pow(x[i] - y[i], 2);
                }

                return Math.Sqrt(sumOfSquaredDifference);
            };

            var data = new List<double[]>();
            var originalSeries = GeneratePointScatterSeries(100, OxyColor.FromRgb(30, 30, 30), 4, "Cluster 1", 1, 1, 1, 1, ref data);
            var distantPoints = MostDistantPoints.MaximallyDistantPoints(data.ToArray(), 30, Metric);

            #region Setup Original Cluster Plot

            // create series for maximal points
            var maximalScatterPoints = new List<ScatterPoint>();
            for (int i = 0; i < data.Count; i++)
            {
                if (distantPoints.Contains(i))
                {
                    maximalScatterPoints.Add(new ScatterPoint(data[i][0], data[i][1]));
                }
            }
            var maximalScatterSeries = new OxyPlot.Series.ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(240, 10, 10)
            };
            maximalScatterSeries.Points.AddRange(maximalScatterPoints);

            originalPlot.Model.LegendTitle = "Legend";
            originalPlot.Model.LegendPlacement = LegendPlacement.Outside;

            originalPlot.Model.Series.Add(originalSeries);
            originalPlot.Model.Series.Add(maximalScatterSeries);

            originalPlot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Bottom, Minimum = -10, Maximum = 10 });
            originalPlot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Minimum = -10, Maximum = 10 });

            originalPlot.ActualModel.Axes[0].Reset();
            originalPlot.ActualModel.Axes[1].Reset();

            #endregion

            #region Setup Computed Cluster Plot

            // Set up plot
            var computedPlot = new PlotView { Model = new PlotModel() };
            this.ClusteredData.Children.Add(computedPlot);
            computedPlot.Model.PlotType = PlotType.XY;
            computedPlot.Model.Background = OxyColor.FromRgb(255, 255, 255);
            computedPlot.Model.TextColor = OxyColor.FromRgb(0, 0, 0);


            // scatter points used for the plot

            computedPlot.Model.LegendTitle = "Legend";
            computedPlot.Model.LegendPlacement = LegendPlacement.Outside;

            computedPlot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Bottom, Minimum = -10, Maximum = 10 });
            computedPlot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Minimum = -10, Maximum = 10 });

            computedPlot.ActualModel.Axes[0].Reset();
            computedPlot.ActualModel.Axes[1].Reset();


            #endregion
        }

        public void kMeansTest()
        {
            // Set up plot
            var originalPlot = new PlotView { Model = new PlotModel() };
            this.OriginalData.Children.Add(originalPlot);
            originalPlot.Model.PlotType = PlotType.XY;
            originalPlot.Model.Background = OxyColor.FromRgb(255, 255, 255);
            originalPlot.Model.TextColor = OxyColor.FromRgb(0, 0, 0);

            // scatter points used for the plot
            var clusterData = new List<double[]>(); // holds data for knn
            var orignalSeries = GeneratePointScatterSeries(100, OxyColor.FromRgb(50, 120, 200), 4, "Cluster 1", 1, 1, 1, 1, ref clusterData);
            var originalSeries2 = GeneratePointScatterSeries(100, OxyColor.FromRgb(200, 50, 50), 4, "Cluster 2", 3, 1, 3, 1, ref clusterData);
            var originalSeries3 = GeneratePointScatterSeries(100, OxyColor.FromRgb(96, 47, 0), 4, "Cluster 3", 5, 1, 0, 1, ref clusterData);


            var kmeans = new KMeans<double[]>(3, new DoubleArrayEqualityComparer());
            kmeans.Metric = (d, d1) => Math.Sqrt(Math.Pow(d[0] - d1[0], 2) + Math.Pow(d[1] - d1[1], 2)); // Euclidean metric
            // kmeans.Metric = (d, d1) => Math.Abs(d[0] - d1[0]) + Math.Abs(d[1] - d1[1]); // L1 Norm

            kmeans.CentralTendency = (data) => // Standard median calculation
            {
                var xValues = data.Select(x => x[0]).Sum();
                var yValues = data.Select(x => x[1]).Sum();
                var count = (double)data.Count();
                return new double[2] { xValues / count, yValues / count };
            };



            kmeans.ClusterData = clusterData;
            kmeans.ClusterLabels = Enumerable.Repeat(0, clusterData.Count).ToList();
            kmeans.Centroids = clusterData.Take(kmeans.Clusters).ToArray();
            kmeans.Compute();


            #region Setup Original Cluster Plot

            // plot both clusters

            originalPlot.Model.LegendTitle = "Legend";
            originalPlot.Model.LegendPlacement = LegendPlacement.Outside;

            originalPlot.Model.Series.Add(orignalSeries);
            originalPlot.Model.Series.Add(originalSeries2);
            originalPlot.Model.Series.Add(originalSeries3);

            originalPlot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Bottom, Minimum = -10, Maximum = 10 });
            originalPlot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Minimum = -10, Maximum = 10 });

            originalPlot.ActualModel.Axes[0].Reset();
            originalPlot.ActualModel.Axes[1].Reset();

            #endregion

            #region Setup Computed Cluster Plot

            // Set up plot
            var computedPlot = new PlotView { Model = new PlotModel() };
            this.ClusteredData.Children.Add(computedPlot);
            computedPlot.Model.PlotType = PlotType.XY;
            computedPlot.Model.Background = OxyColor.FromRgb(255, 255, 255);
            computedPlot.Model.TextColor = OxyColor.FromRgb(0, 0, 0);


            AddClusterSeriesToModel(0, kmeans, computedPlot.Model, OxyColor.FromRgb(50, 120, 200), OxyColor.FromRgb(255, 0, 255), 4, 8, "Cluster 1");
            AddClusterSeriesToModel(1, kmeans, computedPlot.Model, OxyColor.FromRgb(200, 50, 50), OxyColor.FromRgb(0, 255, 0), 4, 8, "Cluster 2");
            AddClusterSeriesToModel(2, kmeans, computedPlot.Model, OxyColor.FromRgb(96, 47, 0), OxyColor.FromRgb(0, 255, 255), 4, 8, "Cluster 3");
            //AddClusterSeriesToModel(3, kmeans, computedPlot.Model, OxyColor.FromRgb(255, 47, 255), OxyColor.FromRgb(0, 255, 255), 4, 8, "Cluster 4");

            // scatter points used for the plot

            computedPlot.Model.LegendTitle = "Legend";
            computedPlot.Model.LegendPlacement = LegendPlacement.Outside;

            computedPlot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Bottom, Minimum = -10, Maximum = 10 });
            computedPlot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Minimum = -10, Maximum = 10 });

            computedPlot.ActualModel.Axes[0].Reset();
            computedPlot.ActualModel.Axes[1].Reset();

            #endregion

        }

        public void ENNTest()
        {
            // Set up plot
            var originalPlot = new PlotView { Model = new PlotModel() };
            this.OriginalData.Children.Add(originalPlot);
            originalPlot.Model.PlotType = PlotType.XY;
            originalPlot.Model.Background = OxyColor.FromRgb(255, 255, 255);
            originalPlot.Model.TextColor = OxyColor.FromRgb(0, 0, 0);

            // scatter points used for the plot
            var clusterData0 = new List<double[]>(); // holds data for knn
            var clusterData1 = new List<double[]>(); // holds data for knn
            var orignalSeries = GeneratePointScatterSeries(100, OxyColor.FromRgb(50, 120, 200), 4, "Cluster 1", 0, 1, 0, 1, ref clusterData0);
            var originalSeries2 = GeneratePointScatterSeries(100, OxyColor.FromRgb(200, 50, 50), 4, "Cluster 2", 5, 1, 5, 1, ref clusterData1);
            // var originalSeries3 = GeneratePointScatterSeries(100, OxyColor.FromRgb(96, 47, 0), 4, "Cluster 3", 5, 1, 0, 1, ref clusterData);

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


            var labels = Enumerable.Repeat(0, clusterData0.Count).Concat(Enumerable.Repeat(1, clusterData1.Count)).ToArray();
            var trainingData = clusterData0.Concat(clusterData1).ToArray();
            enn.Train(trainingData, labels);

            var numOfTestPoints = 500;
            var xPointsTest = ziggurat.NextSamples(3, 3, numOfTestPoints);
            var yPointsTest = ziggurat.NextSamples(3, 3, numOfTestPoints);


            var tmp = new List<double[]>();
            for (int i = 0; i < xPointsTest.Length; i++)
            {
                tmp.Add(new double[] { xPointsTest[i], yPointsTest[i] });
            }

            var testingData = tmp.ToArray();
            var predictedLabels = enn.Predict(testingData);

            var correctPoints = new List<ScatterPoint>();
            var incorrectPoints = new List<ScatterPoint>();
            for (int i = 0; i < numOfTestPoints; i++)
            {
                // var result = enn.Classify(new double[] { xPointsTest[i], yPointsTest[i] });
                if (predictedLabels[i] == 0)
                {
                    correctPoints.Add(new ScatterPoint(xPointsTest[i], yPointsTest[i]));
                }
                else
                {
                    incorrectPoints.Add(new ScatterPoint(xPointsTest[i], yPointsTest[i]));
                }
            }
            this.Title = "k-NN Classification Example";

            var ssCorrect = new OxyPlot.Series.ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(0, 255, 0),
                //MarkerSize = 4
            };
            ssCorrect.Points.AddRange(correctPoints);
            var ssIncorrect = new OxyPlot.Series.ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(255, 255, 0),
                //MarkerSize = 4
            };
            ssIncorrect.Points.AddRange(incorrectPoints);


            #region Setup Original Cluster Plot

            // plot both clusters

            originalPlot.Model.LegendTitle = "Legend";
            originalPlot.Model.LegendPlacement = LegendPlacement.Outside;

            originalPlot.Model.Series.Add(orignalSeries);
            originalPlot.Model.Series.Add(originalSeries2);
            originalPlot.Model.Series.Add(ssCorrect);
            originalPlot.Model.Series.Add(ssIncorrect);
            // originalPlot.Model.Series.Add(originalSeries3);

            originalPlot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Bottom, Minimum = -10, Maximum = 10 });
            originalPlot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Minimum = -10, Maximum = 10 });

            originalPlot.ActualModel.Axes[0].Reset();
            originalPlot.ActualModel.Axes[1].Reset();

            #endregion

        }

        public void kNNTest()
        {
            // Set up plot
            var plot = new PlotView { Model = new PlotModel() };
            this.OriginalData.Children.Add(plot);
            plot.Model.PlotType = PlotType.XY;
            plot.Model.Background = OxyColor.FromRgb(255, 255, 255);
            plot.Model.TextColor = OxyColor.FromRgb(0, 0, 0);

            var ziggurat = new ZigguratGaussianSampler();
            var knn = new KNearestNeighbors<double[]>(3, 2) { Metric = (d, d1) => Math.Sqrt(Math.Pow(d[0] - d1[0], 2) + Math.Pow(d[1] - d1[1], 2)) };

            var xPoints = ziggurat.NextSamples(1, 1, 30);
            var yPoints = ziggurat.NextSamples(1, 1, 30);
            var scatterPoints = new List<ScatterPoint>();
            for (int i = 0; i < xPoints.Length; i++)
            {
                scatterPoints.Add(new ScatterPoint(xPoints[i], yPoints[i]));
            }


            var scatterSeries = new OxyPlot.Series.ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(50, 120, 200)
            };
            scatterSeries.Points.AddRange(scatterPoints);



            var xPoints2 = ziggurat.NextSamples(7, 1, 30);
            var yPoints2 = ziggurat.NextSamples(7, 1, 30);
            var scatterPoints2 = new List<ScatterPoint>();
            for (int i = 0; i < xPoints.Length; i++)
            {
                scatterPoints2.Add(new ScatterPoint(xPoints2[i], yPoints2[i]));
            }


            var scatterSeries2 = new OxyPlot.Series.ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(200, 50, 50)
            };
            scatterSeries2.Points.AddRange(scatterPoints2);


            for (int i = 0; i < 30; i++)
            {
                knn.Train(0, new double[] { xPoints[i], yPoints[i] });
            }

            for (int i = 0; i < 30; i++)
            {
                knn.Train(1, new double[] { xPoints2[i], yPoints2[i] });
            }


            var numOfTestPoints = 1000;
            var xPointsTest = ziggurat.NextSamples(3, 3, numOfTestPoints);
            var yPointsTest = ziggurat.NextSamples(3, 3, numOfTestPoints);

            var correctPoints = new List<ScatterPoint>();
            var incorrectPoints = new List<ScatterPoint>();
            for (int i = 0; i < numOfTestPoints; i++)
            {
                var result = knn.Classify(new double[] { xPointsTest[i], yPointsTest[i] });
                if (result == 0)
                {
                    correctPoints.Add(new ScatterPoint(xPointsTest[i], yPointsTest[i]));
                }
                else
                {
                    incorrectPoints.Add(new ScatterPoint(xPointsTest[i], yPointsTest[i]));
                }
            }
            this.Title = "k-NN Classification Example";

            var ssCorrect = new OxyPlot.Series.ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(0, 255, 0),
                //MarkerSize = 4
            };
            ssCorrect.Points.AddRange(correctPoints);
            var ssIncorrect = new OxyPlot.Series.ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(255, 255, 0),
                //MarkerSize = 4
            };
            ssIncorrect.Points.AddRange(incorrectPoints);

            plot.Model.LegendTitle = "Legend";
            plot.Model.LegendPlacement = LegendPlacement.Outside;
            scatterSeries.Title = "Observed Class 1";
            scatterSeries2.Title = "Observed Class 2";
            ssCorrect.Title = "Classified as Class 1";
            ssIncorrect.Title = "Classified as Class 2";

            plot.Model.Series.Add(ssCorrect);
            plot.Model.Series.Add(ssIncorrect);
            plot.Model.Series.Add(scatterSeries);
            plot.Model.Series.Add(scatterSeries2);

            plot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Bottom, Minimum = -10, Maximum = 10 });
            plot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Minimum = -10, Maximum = 10 });

            plot.ActualModel.Axes[0].Reset();
            plot.ActualModel.Axes[1].Reset();
        }

        public OxyPlot.Series.ScatterSeries GeneratePointScatterSeries(int samples, OxyColor color, int size, string title, double meanX, double stdX, double meanY, double stdY, ref List<double[]> dataPoints)
        {

            var clusterData = new List<double[]>(); // holds data for knn
            var scatterPoints = new List<ScatterPoint>();
            var xPoints = ziggurat.NextSamples(meanX, stdX, samples);
            var yPoints = ziggurat.NextSamples(meanY, stdY, samples);
            for (int i = 0; i < xPoints.Length; i++)
            {
                clusterData.Add(new double[] { xPoints[i], yPoints[i] });
                scatterPoints.Add(new ScatterPoint(xPoints[i], yPoints[i]));
            }

            var ss = new OxyPlot.Series.ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = color,
                MarkerSize = size,
                Title = title
            };

            ss.Points.AddRange(scatterPoints);
            dataPoints.AddRange(clusterData);
            return ss;
        }

        public void AddClusterSeriesToModel(int clusterLabel, KMeans<double[]> kMeans, PlotModel plot, OxyColor pointColor, OxyColor centroidColor, int pointSize, int centroidSize, string title)
        {
            var computedPoints1 = kMeans.GetClusterData(clusterLabel).Select(point => new ScatterPoint(point[0], point[1])).ToList();
            // plot both clusters
            var computerScatterSeries1 = new OxyPlot.Series.ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = pointColor,
                MarkerSize = pointSize,
                Title = title
            };

            computerScatterSeries1.Points.AddRange(computedPoints1);


            var centroidSS1 = new OxyPlot.Series.ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = centroidColor,
                MarkerSize = centroidSize
            };
            centroidSS1.Points.Add(new ScatterPoint(kMeans.Centroids[clusterLabel][0], kMeans.Centroids[clusterLabel][1]));

            plot.Series.Add(centroidSS1);
            plot.Series.Add(computerScatterSeries1);
        }

    }
}
