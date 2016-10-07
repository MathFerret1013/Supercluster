using System;
using System.Collections.Generic;

namespace Supercluster.MTree
{
    using System.Linq;

    using Supercluster.MTree.NewDesign;

    public static class Utilities
    {

        /// <summary>
        /// Gets <paramref name="n"/> choose 2 combinations of positive integers less than n, where each element
        /// of the pair is distinct. For n, there are (n * (n-1))/2 such pairs. The pairs are ordered by first index, then by second index.
        /// </summary>
        /// <param name="n">The exclusive upper bound of each element of the tuples.</param>
        /// <returns>The distinct pairs.</returns>
        public static Tuple<int, int>[] UniquePairs(int n)
        {
            var tupleList = new List<Tuple<int, int>>();
            for (var i = 0; i < n; i++)
            {
                for (var j = i + 1; j < n; j++)
                {
                    tupleList.Add(new Tuple<int, int>(i, j));
                }
            }

            return tupleList.ToArray();
        }

        public static double MaxDistanceFromFirst<T>(this IEnumerable<int> source, DistanceMatrix<T> distanceMatrix)
        {
            var len = source.Count();
            var maxDist = double.MinValue;
            var promotionObject = source.First();
            foreach (var element in source)
            {
                maxDist = Math.Max(maxDist, distanceMatrix[promotionObject, element]);
            }
            return maxDist;
        }
    }
}
