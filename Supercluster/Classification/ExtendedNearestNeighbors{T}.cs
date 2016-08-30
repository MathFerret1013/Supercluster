namespace Supercluster.Classification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The Extended Nearest Neighbors learning algorithm is a variation of the kNN learning algorithm which classification decisions are made in such a way that they maximize intra-class coherence.
    /// Bo Tang; Haibo He, "ENN: Extended Nearest Neighbor Method for Pattern Recognition [Research Frontier]," in Computational Intelligence Magazine, IEEE , vol.10, no.3, pp.52-60, Aug. 2015 doi: 10.1109/MCI.2015.2437512
    /// </summary>
    /// <typeparam name="T">The data-type of the data points to be classified.</typeparam>
    public class ExtendedNearestNeighbors<T> : ISupervisedClassifier<T>
    {
        /// <summary>
        /// An array containing the original training data.
        /// </summary>
        private T[] trainingData;

        /// <summary>
        /// An array containg the original training outputs.
        /// </summary>
        private int[] trainingOutputs;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedNearestNeighbors{T}"/> class.
        /// </summary>
        /// <param name="k">The number of neighbors during classification</param>
        /// <param name="clusters">The number of clusters (classes) that the model should have</param>
        public ExtendedNearestNeighbors(int k, int clusters)
        {
            this.Clusters = clusters;
            this.K = k;
        }

        /// <summary>
        /// The metric used to calculate distance between two datapoints.
        /// </summary>
        public Func<T, T, double> Metric { get; set; }

        private double[][] WeightedKNNDistance;
        private int[][] WeightedKNNLabels;
        private double[] ClassStatistics;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testingData"></param>
        /// <returns></returns>
        public int[] Predict(T[] testingData)
        {
            var predictedLabels = Enumerable.Repeat(-1, testingData.Length).ToArray();
            var numClass = this.trainingOutputs.Distinct().Count();
            var numTrainingEachClass = new int[numClass];

            // calculate the num training each class
            for (int i = 0; i < numClass; i++)
            {
                numTrainingEachClass[i] = this.trainingOutputs.Count(l => l == i);
            }

            // classify each Point
            for (int i = 0; i < testingData.Length; i++)
            {

                // get all distances for the Point
                var distances = new List<double[]>();
                for (int j = 0; j < this.trainingData.Length; j++)
                {
                    // TODO: Here we have the index, distance, then class label, make this more elegant
                    distances.Add(new double[] { j, this.Metric(testingData[i], this.trainingData[j]), this.trainingOutputs[j] });
                }

                // index 0: index of trainingData
                // index 1: distance between testingData and trainingData
                distances = distances.OrderBy(d => d[1]).ToList();

                // count how many training labels per class
                var hitNumKNN = new int[numClass];
                for (int j = 0; j < numClass; j++)
                {
                    hitNumKNN[j] = distances.Take(this.K).Count(d => (int)d[2] == j);
                }

                // variables defined in matlab program
                var tsEnn = new double[numClass];
                var numTrainingNN = new double[numClass];
                var numSameTraining = new double[numClass];

                // for each class we count the change of class membership among the k-neighbors
                for (int j = 0; j < numClass; j++)
                {
                    // TODO: Optimize the shit out of this

                    // Get all the indexes of the current class
                    var indicies = this.trainingOutputs.FindAllIndex(l => l == j).ToArray();
                    distances = distances.OrderBy(d => d[0]).ToList();

                    // calculates the distances for each Point 
                    var testingMuDistances = distances.WithIndexes(indicies).Select(x => x[1]).ToArray();

                    // only get the k-th nearest neighbor
                    var trainingMuDis = this.WeightedKNNDistance.WithIndexes(indicies).Select(x => x[this.K - 1]).ToArray();
                    var trainingMuClass = this.WeightedKNNLabels.WithIndexes(indicies).Select(x => x[this.K - 1]).ToArray();

                    var diffDistance = testingMuDistances.ArraySubtract(trainingMuDis);
                    var indexesLessThanZero = diffDistance.FindAllIndex(d => d <= 0);

                    numTrainingNN[j] = indexesLessThanZero.Count();
                    if (numTrainingNN[j] > 0)
                    {
                        numSameTraining[j] =
                            trainingMuClass.WithIndexes(indexesLessThanZero.ToArray()).Count(m => m == j);
                    }
                }

                // now we calculate the class statistics
                for (int j = 0; j < numClass; j++) // we assume the testing data to be in class j
                {
                    var deltaNumSame = numTrainingNN[j] - numSameTraining[j];
                    var difTmp = numSameTraining.ArrayDivide(numTrainingEachClass.Select(x => (double)(x * this.K)).ToArray());
                    var deltaNumDif = difTmp.Sum() - numSameTraining[j] / ((double)(numTrainingEachClass[j] * this.K));

                    tsEnn[j] = ((deltaNumSame + hitNumKNN[j] - (this.ClassStatistics[j] * this.K)) / ((numTrainingEachClass[j] + 1) * this.K)) - deltaNumDif;

                }

                predictedLabels[i] = tsEnn.MaxIndexRandomTies();

            }

            return predictedLabels;
        }

        /// <summary>
        /// The number of clusters the model has.
        /// </summary>
        public int Clusters { get; private set; }

        /// <summary>
        /// The number of neighbors used during classification.
        /// </summary>
        public int K { get; private set; }

        /// <summary>
        /// Performs classification of a single Point.
        /// </summary>
        /// <param name="dataPoint">Datum to be classified.</param>
        /// <returns>The output-class index.</returns>
        public int Compute(T dataPoint)
        {
            var numClass = this.trainingOutputs.Distinct().Count();
            var numTrainingEachClass = new int[numClass];

            // calculate the num training each class
            for (int i = 0; i < numClass; i++)
            {
                numTrainingEachClass[i] = this.trainingOutputs.Count(l => l == i);
            }

            // get all distances for the Point
            var distances = new List<double[]>();
            for (int j = 0; j < this.trainingData.Length; j++)
            {
                // TODO: Here we have the index, distance, then class label, make this more elegant
                distances.Add(new double[] { j, this.Metric(dataPoint, this.trainingData[j]), this.trainingOutputs[j] });
            }

            // index 0: index of trainingData
            // index 1: distance between testingData and trainingData
            distances = distances.OrderBy(d => d[1]).ToList();

            // count how many training labels per class
            var hitNumKNN = new int[numClass];
            for (int j = 0; j < numClass; j++)
            {
                hitNumKNN[j] = distances.Take(this.K).Count(d => (int)d[2] == j);
            }

            // variables defined in matlab program
            var tsEnn = new double[numClass];
            var numTrainingNN = new double[numClass];
            var numSameTraining = new double[numClass];

            // for each class we count the change of class membership among the k-neighbors
            for (int j = 0; j < numClass; j++)
            {
                // TODO: Optimize the shit out of this

                // Get all the indexes of the current class
                var indicies = this.trainingOutputs.FindAllIndex(l => l == j).ToArray();
                distances = distances.OrderBy(d => d[0]).ToList();

                // calculates the distances for each Point 
                var testingMuDistances = distances.WithIndexes(indicies).Select(x => x[1]).ToArray();

                // only get the k-th nearest neighbor
                var trainingMuDis = this.WeightedKNNDistance.WithIndexes(indicies).Select(x => x[this.K - 1]).ToArray();
                var trainingMuClass = this.WeightedKNNLabels.WithIndexes(indicies).Select(x => x[this.K - 1]).ToArray();

                var diffDistance = testingMuDistances.ArraySubtract(trainingMuDis);
                var indexesLessThanZero = diffDistance.FindAllIndex(d => d <= 0);

                numTrainingNN[j] = indexesLessThanZero.Count();
                if (numTrainingNN[j] > 0)
                {
                    numSameTraining[j] =
                        trainingMuClass.WithIndexes(indexesLessThanZero.ToArray()).Count(m => m == j);
                }
            }

            // now we calculate the class statistics
            for (int j = 0; j < numClass; j++) // we assume the testing data to be in class j
            {
                var deltaNumSame = numTrainingNN[j] - numSameTraining[j];
                var difTmp = numSameTraining.ArrayDivide(numTrainingEachClass.Select(x => (double)(x * this.K)).ToArray());
                var deltaNumDif = difTmp.Sum() - (numSameTraining[j] / ((double)(numTrainingEachClass[j] * this.K)));

                tsEnn[j] = ((deltaNumSame + hitNumKNN[j] - (this.ClassStatistics[j] * this.K)) / ((numTrainingEachClass[j] + 1) * this.K)) - deltaNumDif;
            }

            return tsEnn.MaxIndexRandomTies();
        }

        /// <summary>
        /// We train the ENN algorithm by computing a "kNN map". See [1] and the source code for a explanation of a weighted kNN map.
        /// </summary>
        /// <param name="trainingData">The training data set.</param>
        /// <param name="trainingOutputs">The supervised classification outputs of the training data set.</param>
        public void Train(T[] trainingData, int[] trainingOutputs)
        {
            this.trainingData = trainingData;
            this.trainingOutputs = trainingOutputs.BijectWithNaturals();

            // holds the k-nearest distances
            var weightedKNNDistance = new double[trainingData.Length][];

            // holds the k-nearest labels
            var weightedKNNLabels = new int[trainingData.Length][];

            Parallel.For(
                0,
                trainingData.Length,
                index =>
                {
                    var currentPoint = trainingData[index];
                    var distances = new List<double[]>();

                    // calculate all the distances
                    for (int i = 0; i < trainingData.Length; i++)
                    {
                        distances.Add(
                            new double[] { trainingOutputs[i], this.Metric(currentPoint, trainingData[i]) });
                    }

                    // exclude self
                    distances[index][1] = double.PositiveInfinity;

                    // get k nearest distance
                    var topK = distances.OrderBy(d => d[1]).Take(this.K).ToArray();

                    // set the distance matrix and labels matrix
                    weightedKNNDistance[index] = new double[this.K];
                    weightedKNNLabels[index] = new int[this.K];

                    for (int i = 0; i < this.K; i++)
                    {
                        weightedKNNLabels[index][i] = (int)topK[i][0];
                        weightedKNNDistance[index][i] = topK[i][1];
                    }
                });

            // TODO: Make method
            // calculate class statistics
            var classStatistics = new double[this.Clusters];
            // Parallel.For(0, classes, currentClassLabel =>
            for (int currentClassLabel = 0; currentClassLabel < this.Clusters; currentClassLabel++)
            {
                var currentClassIndexes = trainingOutputs.FindAllIndex(t => t == currentClassLabel).ToArray();
                var numTraining = currentClassIndexes.Length;

                var count = 0;

                // for each Point in the current class
                for (int j = 0; j < numTraining; j++)
                {
                    // count the number of k neighbours that are the current class
                    var neighboorLabels = weightedKNNLabels[currentClassIndexes[j]];
                    var countofCurrentClass = neighboorLabels.Count(l => l == currentClassLabel);
                    count += countofCurrentClass;
                }

                classStatistics[currentClassLabel] = (double)count / (double)(numTraining * this.K);
            }

            this.WeightedKNNDistance = weightedKNNDistance;
            this.WeightedKNNLabels = weightedKNNLabels;
            this.ClassStatistics = classStatistics;
        }
    }
}
