// Supercluster
// https://github.com/MathFerret1013/
// eregina92@gmail.com
//  
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//     
//        http://www.apache.org/licenses/LICENSE-2.0
//     
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
namespace Supercluster.Classification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// kNN Learning Algorithm.
    /// </summary>
    /// <remarks>
    /// 
    /// <h2>Introduction</h2>
    /// <para> The <i>k</i> nearest neighbors learning algorithm (kNN) is a classification algorithm. To determine class membership of a unseen input the algorithm computes the <i>k</i> nearest neighbors in the learned feature space 
    /// and then chooses the class which is most common among the <i>k</i> neighbors.</para>
    /// 
    /// <para> The picture below is of the output of the kNN algorithm k=3 in a 2-dimensional feature space with 2 classes. The blue and red dots are datapoints belonging to class 1 and class 2 which were learned during model training.
    ///  The green dots are new datapoints which were classified as class 1 (meaning atleast 2 of their nearest neighbors are in class 1). The yellow dots are new datapoints which were classified as class 2 (meaning atleast 2 of their nearest neighbors are in class 2).</para>
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
        private readonly List<List<T>> internalData;

        /// <summary>
        /// Initializes a new instance of the <see cref="KNearestNeighbors{T}"/> class.
        /// </summary>
        /// <param name="k">The number of neighbors during classification</param>
        /// <param name="clusters">The number of clusters (classes) that the model should have</param>
        public KNearestNeighbors(int k, int clusters)
        {
            this.internalData = new List<List<T>>();
            for (int i = 0; i < clusters; i++)
            {
                this.internalData.Add(new List<T>());
            }

            this.Clusters = clusters;
            this.K = k;
        }

        /// <summary>
        /// The metric used to calculate distance between two datapoints.
        /// </summary>
        public Func<T, T, double> Metric { get; set; }

        /// <summary>
        /// The number of clusters the model has.
        /// </summary>
        public int Clusters { get; private set; }

        /// <summary>
        /// The number of neighbors used during classification.
        /// </summary>
        public int K { get; private set; }

        /// <summary>
        /// Trains the model with a single given datapoint and an appropriate class label.
        /// </summary>
        /// <param name="label">The class label of the given <paramref name="datapoint"/></param>
        /// <param name="datapoint">The datapoint to add to the model</param>
        /// <exception cref="ArgumentException">Thrown if the label of the datapoint does not exist in the current model.</exception>
        public void Train(int label, T datapoint)
        {
            // The label given must be within the number of clusters
            if (!(label >= 0 && label < this.Clusters))
            {
                throw new ArgumentException("You provided a label that does not exist in the current model.");
            }

            this.internalData[label].Add(datapoint);
        }

        /// <summary>
        /// Trains the model with a set of data-points and class labels.
        /// </summary>
        /// <param name="labels">The set of class labels.</param>
        /// <param name="datapoints">The set of datapoints</param>
        public void TrainAll(IEnumerable<int> labels, IEnumerable<T> datapoints)
        {
            if (labels.Count() != datapoints.Count())
            {
                throw new ArgumentException("The number of labels and data points is not the same.");
            }

            var labelsEnumerator = labels.GetEnumerator();
            var datapointsEnumerator = datapoints.GetEnumerator();

            while (labelsEnumerator.MoveNext() && datapointsEnumerator.MoveNext())
            {
                this.Train(labelsEnumerator.Current, datapointsEnumerator.Current);
            }
        }

        /// <summary>
        /// Determines the class a given point belongs to.
        /// </summary>
        /// <param name="datapoint"> The point to be classified</param>
        /// <returns>A class label</returns>
        public int Classify(T datapoint)
        {
            var nearestNeighbors = new SortedArray<NearestNeighborPoint<T>>(this.K, new NearestNeighborPoint<T>(datapoint, double.PositiveInfinity));

            for (int classIndex = 0; classIndex < this.Clusters; classIndex++)
            {
                var currentClass = this.internalData[classIndex];

                // Get the k nearest neighbors
                for (int index = 0; index < currentClass.Count; index++)
                {
                    var point = currentClass[index];
                    var dist = this.Metric(datapoint, point);

                    // update our list of closest points if necessary
                    nearestNeighbors.Add(new NearestNeighborPoint<T>(
                                         point: point,
                                         distance: dist,
                                         classLabel: classIndex));
                }
            }

            var labelCount = new int[this.Clusters];
            foreach (var neighbor in nearestNeighbors)
            {
                labelCount[neighbor.ClassLabel]++;
            }

            return labelCount.MaxIndex();
        }
    }
}