namespace Supercluster.Classification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Supercluster.Structures;
    using Supercluster.Structures.Interfaces;

    /// <summary>
    /// kNN Learning Algorithm.
    /// </summary>
    /// <remarks>
    /// 
    /// <h2>Introduction</h2>
    /// <para> The <i>k</i> nearest neighbors learning algorithm (kNN) is a classification algorithm. To determine class membership of a unseen input the algorithm computes the <i>k</i> nearest neighbors in the learned feature space 
    /// and then chooses the class which is most common among the <i>k</i> neighbors.</para>
    /// 
    /// <para> The picture below is of the output of the kNN algorithm k=3 in a 2-dimensional feature space with 2 classes. The blue and red dots are point belonging to class 1 and class 2 which were learned during model training.
    ///  The green dots are new point which were classified as class 1 (meaning at least 2 of their nearest neighbors are in class 1). The yellow dots are new point which were classified as class 2 (meaning atleast 2 of their nearest neighbors are in class 2).</para>
    ///  <img src="..\..\media\knn_example.png" />
    /// 
    /// <h3>Advantages</h3>
    /// 
    /// <ul style="list-style-type:square">
    ///  <li>The kNN learning algorithm is perhaps the simplest learning algorithm. It is extremely easy to implement, and very easy to understand even for one who is a complete novice to machine learning.</li>
    ///  <li>When the kNN is used with a metric that is fast to compute (such as the <a href="https://en.wikipedia.org/wiki/Euclidean_distance">Euclidean Metric</a>) the model may usually be trained very quickly compared to more complex learning algorithms.</li>
    ///</ul>
    ///     
    /// <h3>Considerations</h3>
    /// <ul style="list-style-type:square">
    /// <li>To prevent ties during the classification step one should use an odd value of <i>k</i> if there is an even number of classes.</li>
    /// <li>The number of possible classification outputs must be known when using a kNN as it is a <a href="https://en.wikipedia.org/wiki/Supervised_learning">supervised learning algorithm</a>.</li>
    /// <li>The kNN learning algorithm essentially <i>memorizes</i> all supervised examples, thus very large example datasets may require much of memory.</li>
    /// <li>kNN performs very poorly when the training data has noisy or irrelevant features.</li>
    ///</ul>
    /// 
    /// </remarks>
    /// <example>
    /// 
    /// <h2>This is a test.</h2>
    /// 
    /// <code>
    ///  public class TestCode()
    ///  {
    ///     var ferret = 3;
    ///  }
    /// </code>
    /// </example>
    /// 
    /// <typeparam name="T">The type of the training data.</typeparam>
    [Serializable]
    public class KNearestNeighbors<T>
    {
        /// <summary>
        /// The internal dataset of the points observed.
        /// </summary>
        private ISpatialQueryable<T> internalDataStructure;

        /// <summary>
        /// Initializes a new instance of the <see cref="KNearestNeighbors{T}"/> class.
        /// </summary>
        /// <param name="k">The number of neighbors during classification</param>
        /// <param name="dataStructure">The backing data structure</param>
        public KNearestNeighbors(int k, Func<T, T, double> metric, ISpatialQueryable<T> dataStructure = null)
        {
            this.Metric = metric;

            this.K = k;

            this.internalDataStructure = dataStructure ?? new MetricSpaceSubset<T>(metric);

            this.clusterIndexDictionary = new Dictionary<int, List<int>>();
            this.Clusters = new ClusterDictionary<int, T>(this.internalDataStructure, this.clusterIndexDictionary);
        }

        /// <summary>
        /// The metric used to calculate distance between two point.
        /// </summary>
        public Func<T, T, double> Metric { get; set; }

        /// <summary>
        /// The number of clusters the model has.
        /// </summary>
        public ClusterDictionary<int, T> Clusters { get; }

        private Dictionary<int, List<int>> clusterIndexDictionary;

        /// <summary>
        /// The number of neighbors used during classification.
        /// </summary>
        public int K { get; }

        /// <summary>
        /// Trains the model with a single given point and an appropriate class label.
        /// </summary>
        /// <param name="point">The point to add to the model</param>
        /// <param name="label">The class label of the given <paramref name="point"/></param>
        /// <exception cref="ArgumentException">Thrown if the label of the point does not exist in the current model.</exception>
        public void Train(T point, int label)
        {
            var pointIndex = this.internalDataStructure.Add(point);
            if (this.clusterIndexDictionary.ContainsKey(label))
            {
                this.clusterIndexDictionary[label].Add(pointIndex);
            }
            else
            {
                this.clusterIndexDictionary.Add(label, new List<int> { pointIndex });
            }
        }

        /// <summary>
        /// Trains the model with a set of data-points and class labels.
        /// </summary>
        /// <param name="points">The set of point</param>
        /// <param name="labels">The set of class labels.</param>
        public void TrainAll(IEnumerable<T> points, IEnumerable<int> labels)
        {
            if (labels.Count() != points.Count())
            {
                throw new ArgumentException("The number of labels and data points is not the same.");
            }

            var pointIndexes = this.internalDataStructure.Add(points).ToArray();
            var labelsArray = labels.ToArray();

            for (int i = 0; i < pointIndexes.Length; i++)
            {
                var label = labelsArray[i];
                var pointIndex = pointIndexes[i];

                if (this.clusterIndexDictionary.ContainsKey(label))
                {
                    this.clusterIndexDictionary[label].Add(pointIndex);
                }
                else
                {
                    this.clusterIndexDictionary.Add(label, new List<int> { pointIndex });
                }
            }
        }

        /// <summary>
        /// Determines the class a given point belongs to.
        /// </summary>
        /// <param name="datapoint"> The point to be classified</param>
        /// <returns>A class label</returns>
        public int Classify(T datapoint)
        {
            var nearestNeighborIndexes = this.internalDataStructure.NearestNeighborIndexes(datapoint, this.K);

            // NOTE: We assume that a point belongs to only one cluster
            var keys = this.clusterIndexDictionary.Keys.ToArray();
            var labelCount = new int[keys.Length];
            foreach (var neighborIndex in nearestNeighborIndexes)
            {
                for (var i = 0; i < keys.Length; i++)
                {
                    if (this.clusterIndexDictionary[keys[i]].Contains(neighborIndex))
                    {
                        // we found the cluster we belong to. Don't check the other clusters.
                        labelCount[i]++;
                        break;
                    }
                }
            }

            return keys[labelCount.MaxIndex()];
        }
    }
}