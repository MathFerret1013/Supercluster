namespace Workbench
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime;
    using System.Runtime.CompilerServices;
    using System.Text;

    using Supercluster.Classification;
    using Supercluster.MTree;
    using Supercluster.MTree.Tests;
    using Supercluster.Structures.MTree;
    using Supercluster.Tests.Structures.MTree.Supercluster.MTree;

    class Program
    {
        static void Main(string[] args)
        {
            var mtreeStopwatch = new Stopwatch();
            var fastMtreeStopWatch = new Stopwatch();

            var originalTimes = new List<long>();
            var fastTimes = new List<long>();

            var dataSize = 100000;
            var testDataSize = 50;
            var range = 1000000;
            var neighbors = 10;
            var dimensions = 1000;

            Console.WriteLine($"{nameof(dataSize)}: {dataSize}");
            Console.WriteLine($"{nameof(dimensions)}: {dimensions}");
            Console.WriteLine($"{nameof(neighbors)}: {neighbors}");
            Console.WriteLine();

            var testData = Supercluster.Tests.Utilities.GenerateDoubles(testDataSize, range, dimensions);
            for (int index = 0; index < testData.Length; index++)
            {

                var treeData = Supercluster.Tests.Utilities.GenerateDoubles(dataSize, range, dimensions);
                var target = testData[index];


                // 1. Build Trees
                var fastMtree = new FastMTree<double[]> { Capacity = 3, Metric = Metrics.L2Norm_Double };
                foreach (var point in treeData)
                {
                    fastMtree.Add(point);
                }


                var mtree = new MTree<double[]> { Capacity = 3, Metric = Metrics.L2Norm_Double };
                foreach (var point in treeData)
                {
                    mtree.Add(point);
                }

                GC.Collect();
                GCLatencyMode oldMode = GCSettings.LatencyMode;

                // Make sure we can always go to the catch block, 
                // so we can set the latency mode back to `oldMode`
                RuntimeHelpers.PrepareConstrainedRegions();

                try
                {
                    GCSettings.LatencyMode = GCLatencyMode.LowLatency;

                    // Measure Trees
                    mtreeStopwatch.Start();
                    var resultsList = mtree.NearestNeighbors(target, neighbors).ToArray();
                    mtreeStopwatch.Stop();

                    fastMtreeStopWatch.Start();
                    var fastResults = fastMtree.NearestNeighbors(target, neighbors).ToArray();
                    fastMtreeStopWatch.Stop();

                    // Generation 2 garbage collection is now
                    // deferred, except in extremely low-memory situations
                }
                finally
                {
                    // ALWAYS set the latency mode back
                    GCSettings.LatencyMode = oldMode;
                }


                // Print times
                if (index != 0) // We skip first run because of jitting
                {

                    // Record Times
                    originalTimes.Add(mtreeStopwatch.ElapsedTicks);
                    fastTimes.Add(fastMtreeStopWatch.ElapsedTicks);
                    Console.WriteLine(mtreeStopwatch.ElapsedTicks + " " + fastMtreeStopWatch.ElapsedTicks);

                }

                // reset stopwatches
                mtreeStopwatch.Reset();
                fastMtreeStopWatch.Reset();


            }

            Console.WriteLine("Average: " + originalTimes.Average() + " " + fastTimes.Average());
            Console.Read();
        }


        static void ParamterTest(string[] args)
        {
            /* We vary 2 parameters
            1. Node Capacity
            2. Data Size

                The aim is to find a relationship between node capacity, data size and performance (ticks) for kNN queries.
            */

            var range = 1000000; // Range for the random data set
            var neighbors = 10;
            var dimensions = 1000;
            var stopwatch = new Stopwatch();

            // node capacity parameter range
            var maxNodeCapacity = 5;
            var minNodeCapacity = 3;

            // data size parameter range
            var minDataSize = 10000;
            var maxDataSize = 1010000;
            var dataSizeStep = 50000;


            // Keeps track of tick recorded 
            // key: node capacity, value elapsed ticks for each test
            // Note that a data set is generated and then all node capacities are tests on the same data set
            var recordedTicks = new Dictionary<int, List<long>>();
            var testDataSize = 31;

            // initialize dictionaries
            for (int i = minNodeCapacity; i <= maxNodeCapacity; i++)
            {
                recordedTicks.Add(i, new List<long>());
            }


            for (int dataSize = minDataSize; dataSize <= maxDataSize; dataSize += dataSizeStep)
            {
                var testDataset = Supercluster.Tests.Utilities.GenerateDoubles(testDataSize, range, dimensions);
                foreach (var testDataum in testDataset)
                {
                    var dataset = Supercluster.Tests.Utilities.GenerateDoubles(dataSize, range, dimensions);

                    // test the dataset on each node capacity
                    for (int nodeCapacity = minNodeCapacity; nodeCapacity <= maxNodeCapacity; nodeCapacity++)
                    {
                        // 1. Build MTree
                        var mtree = new FastMTree<double[]>(Metrics.L2Norm_Double, nodeCapacity, dataset);

                        // 2. Run and time query
                        stopwatch.Start();
                        var results = mtree.NearestNeighbors(testDataum, neighbors);
                        stopwatch.Stop();

                        // 3. record ticks and reset stopwatch
                        recordedTicks[nodeCapacity].Add(stopwatch.ElapsedTicks);

                        Console.WriteLine($"{nameof(nodeCapacity)}: {nodeCapacity}, {nameof(stopwatch.ElapsedTicks)}: {stopwatch.ElapsedTicks}");
                        stopwatch.Reset();
                    }
                }
            }

            var outputStringBuilder = new StringBuilder();

            // Output data
            foreach (var record in recordedTicks)
            {
                outputStringBuilder.Append($"Node Capacity: {record.Key}, Ticks: ");

                foreach (var tickCount in record.Value)
                {
                    outputStringBuilder.Append(tickCount + ", ");
                }

                outputStringBuilder.Append("\n");
            }

            File.WriteAllText(@"C:\Users\Eric\Desktop\nodeCapacity.txt", outputStringBuilder.ToString());
        }

    }
}


// load tree
/*
linearStopwatch.Start();
// linear search
var linearResults =
    treeData.Select(p => new Tuple<double[], double>(p, Metrics.L2Norm_Double(p, target)))
        .OrderBy(p => p.Item2)
        .Take(neighboors)
        .Select(p => p.Item1)
        .ToArray();
linearStopwatch.Stop();
Console.WriteLine("Linear: " + linearStopwatch.ElapsedTicks);
times.Add(linearStopwatch.ElapsedTicks);
linearStopwatch.Reset();
*/

// kd tree
/*
var kdTree = new KDTree<double, double[]>(
    dimensions,
    treeData,
    treeData,
    Metrics.L2Norm_Double,
    double.MinValue,
    double.MaxValue);

kdtreeStopWatch.Start();
var kdtreeResults = kdTree.NearestNeighbors(target, neighboors).ToArray();
kdtreeStopWatch.Stop();
times.Add(kdtreeStopWatch.ElapsedTicks);
Console.WriteLine("KDTree: " + kdtreeStopWatch.ElapsedTicks);
kdtreeStopWatch.Reset();
*/
