namespace Supercluster.MTree.NewDesign
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A class that encapsulates an <see cref="Array"/> as a distance-matrix. The class only stores unique
    /// distances between points and is accessed like a 2-Dimensional rectangular.
    /// </summary>
    /// <remarks>
    /// We calculate all possible unique pair-wise distances
    /// between the points to avoid any further distance calculations.
    /// There is a one-to-one correspondence between uniquePairs and uniqueDistances
    /// The uniqueDistance array holds the calculated distances between pairs of objects.
    /// As an example, consider the distance matrix below created by a list of 4 entries.
    /// The top and left columns are an item's index in entry list. So the Point at (2,1)
    /// would be the distance between the 3rd and 2nd item in the index list.
    ///
    /// | - 0 1 2 3 |
    /// | 0 X 5 4 3 |
    /// | 1 X X 2 1 |
    /// | 2 X X X 0 |
    /// | 3 X X X X |
    ///
    /// All entries in the matrix with X are duplicate or self-distances (which are always 0).
    /// The non X entries represent the index in the uniqueDistances array in which the distance is stored.
    ///
    /// Calculating the index for the given pair(p, q):
    /// Note that dist(p, q) = dist(q, p).
    /// The number of valid entries in each row always increases by 1 as we move up. Also, the
    /// bottom row is always empty. Thus we use the formula for calculating the sum of the first n natural numbers.
    ///
    /// The largest index of the row containing (p, q) is
    ///             var x = (Entries.Length - 1) - Min(p, q);
    ///             var largestIndex = x(x + 1) / 2;
    ///
    /// To locate the index of the Point(p, q) we now just subtract the offset of how many positions
    /// in the current row (p, q) is from largestIndex.
    ///
    /// var offset = Max(p, q) - Min(p, q);
    ///
    /// The final index is just the difference between the two values
    /// var index = largestIndex - offset;
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class DistanceMatrix<T>
    {
        /// <summary>
        /// Calculates all pair-wise distances of the elements in <paramref name="source"/> array
        /// with the given <paramref name="metric"/>.
        /// </summary>
        /// <param name="source">The elements for which the pair-wise distances will be computed.</param>
        /// <param name="metric">A metric function used to calculate distances.</param>
        public DistanceMatrix(IReadOnlyList<T> source, Func<T, T, double> metric)
        {
            var indexPairs = Utilities.UniquePairs(source.Count);
            this.DistanceBetweenUniquePairs = indexPairs.Select(p => metric(source[p.Item1], source[p.Item2])).Reverse().ToArray();
            this.LengthDecremented = source.Count - 1;
        }

        /// <summary>
        /// Computes the index of the element in the <see cref="DistanceBetweenUniquePairs"/> array.
        /// </summary>
        /// <param name="x">The index of a Point in the original source array.</param>
        /// <param name="y">The index of a Point in the original source array.</param>
        /// <returns>The index of the distance between the two points stored in the <see cref="DistanceBetweenUniquePairs"/> array.</returns>
        private int ComputeIndex(int x, int y)
        {
            var max = Math.Max(x, y);
            var min = Math.Min(x, y);
            var n = this.LengthDecremented - min;
            return (n * (n + 1) / 2) - (max - min);
        }

        /// <summary>
        /// Get the distance between the two points and the provided indexes.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>The distance between the points in the source array at the indexes 
        /// provided by the <paramref name="x"/> and <paramref name="y"/> parameters.</returns>
        public double this[int x, int y] => x == y ? 0 : this.DistanceBetweenUniquePairs[this.ComputeIndex(x, y)];

        /// <summary>
        /// Stored at construction and used during the <see cref="ComputeIndex"/> method.
        /// </summary>
        private readonly int LengthDecremented;

        /// <summary>
        /// The array storing the distances between the unique pairs.
        /// </summary>
        private readonly double[] DistanceBetweenUniquePairs;

        /// <summary>
        /// Produces a rectangular array that contains the values in the distance matrix.
        /// Useful if you cannot use the <see cref="DistanceMatrix{T}"/>.
        /// </summary>
        /// <returns>A rectangular array that contains the values in the distance matrix.</returns>
        public double[,] ToRectangularArray()
        {
            var len = this.LengthDecremented + 1;
            var matrix = new double[len, len];
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    matrix[i, j] = this[i, j];
                }
            }

            return matrix;
        }
    }
}
