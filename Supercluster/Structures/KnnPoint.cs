namespace Supercluster
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a point and its label in a K Nearest Neighbor model.
    /// </summary>
    /// <typeparam name="T">The type of the data points in the KNN model.</typeparam>
    [DebuggerDisplay("{Point}, {ClassLabel}")]
    public class KnnPoint<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KnnPoint{T}"/> class.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="classLabel">The label of the point.</param>
        public KnnPoint(T point, int classLabel)
        {
            this.Point = point;
            this.ClassLabel = classLabel;
        }

        /// <summary>
        /// The class label of the neighbor
        /// </summary>
        public int ClassLabel { get; }

        /// <summary>
        /// The data point that is labeled.
        /// </summary>
        public T Point { get; }
    }
}
