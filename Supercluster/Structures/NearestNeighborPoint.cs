namespace Supercluster
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// A struct representing a neighbor in variaous nearest neighbor algorithms.
    /// </summary>
    [DebuggerDisplay("{Distance}, {ClassLabel}")]
    public struct NearestNeighborPoint<T> : IComparable<NearestNeighborPoint<T>>
    {
        /// <summary>
        /// The class label of the neighbor
        /// </summary>
        public int ClassLabel;

        /// <summary>
        /// The distance of the neighbor from the point in question.
        /// </summary>
        public double Distance;

        public T point;

        /// <summary>
        /// Initializes a new instance of the <see cref="NearestNeighborPoint"/> struct.
        /// </summary>
        /// <param name="distance">Distance from the neighbor</param>
        /// <param name="classLabel">The class that the neighbor is in</param>
        /// <param name="index">The index of the point in the data array</param>
        public NearestNeighborPoint(T point, double distance = double.MaxValue, int classLabel = -1)
        {
            this.Distance = distance;
            this.ClassLabel = classLabel;
            this.point = point;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(NearestNeighborPoint<T> other)
        {
            if (this.Distance < other.Distance)
            {
                return -1;
            }

            if (this.Distance > other.Distance)
            {
                return 1;
            }

            return 0;
        }
    }
}
