// Supercluster
// https://github.com/MathFerret1013/
// eregina92@gmail.com
//  

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
    ///  The green dots are new point which were classified as class 1 (meaning atleast 2 of their nearest neighbors are in class 1). The yellow dots are new point which were classified as class 2 (meaning atleast 2 of their nearest neighbors are in class 2).</para>
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
        private ISpatialQueryable<KnnPoint<T>> internalData;

        /// <summary>
        /// Initializes a new instance of the <see cref="KNearestNeighbors{T}"/> class.
        /// </summary>
        /// <param name="k">The number of neighbors during classification</param>
        /// <param name="clusters">The number of clusters (classes) that the model should have</param>
        public KNearestNeighbors(int k, int clusters, Func<T, T, double> metric)
        {
            this.Metric = metric;
            this.Clusters = clusters;
            this.K = k;

            this.internalData = new MetricSpaceSubset<KnnPoint<T>>((x, y) => this.Metric(x.Point, y.Point));
        }

        /// <summary>
        /// The metric used to calculate distance between two point.
        /// </summary>
        public Func<T, T, double> Metric { get; set; }

        /// <summary>
        /// The number of clusters the model has.
        /// </summary>
        public int Clusters { get; }

        /// <summary>
        /// The number of neighbors used during classification.
        /// </summary>
        public int K { get; }

        /// <summary>
        /// Trains the model with a single given point and an appropriate class label.
        /// </summary>
        /// <param name="label">The class label of the given <paramref name="point"/></param>
        /// <param name="point">The point to add to the model</param>
        /// <exception cref="ArgumentException">Thrown if the label of the point does not exist in the current model.</exception>
        public void Train(T point, int label)
        {
            // The label given must be within the number of clusters
            if (!(label >= 0 && label < this.Clusters))
            {
                throw new ArgumentException("You provided a label to a cluster that does not exist in the current model.");
            }

            this.internalData.Add(new KnnPoint<T>(point, label));
        }

        /// <summary>
        /// Trains the model with a set of data-points and class labels.
        /// </summary>
        /// <param name="labels">The set of class labels.</param>
        /// <param name="point">The set of point</param>
        public void TrainAll(IEnumerable<T> point, IEnumerable<int> labels)
        {
            if (labels.Count() != point.Count())
            {
                throw new ArgumentException("The number of labels and data points is not the same.");
            }

            var labelsEnumerator = labels.GetEnumerator();
            var datapointsEnumerator = point.GetEnumerator();

            while (labelsEnumerator.MoveNext() && datapointsEnumerator.MoveNext())
            {
                this.Train(datapointsEnumerator.Current, labelsEnumerator.Current);
            }
        }

        /// <summary>
        /// Determines the class a given point belongs to.
        /// </summary>
        /// <param name="datapoint"> The point to be classified</param>
        /// <returns>A class label</returns>
        public int Classify(T datapoint)
        {
            var nearestNeighbors = this.internalData.NearestNeighbors(new KnnPoint<T>(datapoint, -1), this.K);

            var labelCount = new int[this.Clusters];
            foreach (var neighbor in nearestNeighbors)
            {
                labelCount[neighbor.ClassLabel]++;
            }

            return labelCount.MaxIndex();
        }
    }
}