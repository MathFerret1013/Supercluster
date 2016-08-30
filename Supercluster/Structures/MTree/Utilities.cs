using System;
using System.Collections.Generic;

namespace Supercluster.MTree
{
    using System.Linq;

    using Supercluster.MTree.NewDesign;

    public static class Utilities
    {
        /// <summary>
        /// Gets the index of the elements returned by the <c>IEnumerable.Min()</c> function.
        /// </summary>
        /// <param name="source">The input <c>IEnumerable</c></param>
        /// <returns>The index of the min element in the <c>IEnumerable</c>.</returns>
        public static int MinIndex<T>(this IEnumerable<T> source) where T : IComparable<T>
        {

            var enumerator = source.GetEnumerator();
            enumerator.MoveNext();
            var smallestElement = enumerator.Current;
            var smallestElementIndex = 0;

            var i = 1;
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.CompareTo(smallestElement) < 0)
                {
                    smallestElement = enumerator.Current;
                    smallestElementIndex = i;
                }

                i++;
            }

            return smallestElementIndex;
        }

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

        public static T[] WithIndexes<T>(this IEnumerable<T> source, IEnumerable<int> indicies)
        {
            var len = indicies.Count();
            var result = new T[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = source.ElementAt(indicies.ElementAt(i));
            }

            return result;
        }


        [Obsolete]
        public static T[] WhereIndex<T>(this IEnumerable<T> source, IEnumerable<int> indicies)
        {

            var len = indicies.Count();
            var result = new T[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = source.ElementAt(indicies.ElementAt(i));
            }

            return result;
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
