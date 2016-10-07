namespace Supercluster
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A utility class containing various extension methods to be used with the .NET collection classes.
    /// </summary>
    public static class CollectionUtilities
    {
        #region LinqToIndexes

        /// <summary>
        /// Returns the minimum value in a sequence of <typeparamref name="TSource"/> values using
        /// the default <see cref="Comparer{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the index of the minimum value.</param>
        /// <returns>The index of the minimum value in <paramref name="source"/>.</returns>
        public static int MinIndex<TSource>(this IEnumerable<TSource> source)
        {
            var comparer = Comparer<TSource>.Default;
            using (var enumerator = source.GetEnumerator())
            {
                enumerator.MoveNext();
                var minElement = enumerator.Current;
                var minIndex = 0;

                var i = 1;
                while (enumerator.MoveNext())
                {
                    if (comparer.Compare(enumerator.Current, minElement) < 0)
                    {
                        minElement = enumerator.Current;
                        minIndex = i;
                    }

                    i++;
                }

                return minIndex;
            }
        }

        /// <summary>
        /// Invokes a transform function on each element of <paramref name="source"/> and returns the index of the minimum <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result of the <paramref name="selector"/>.</typeparam>
        /// <param name="source">A sequence of values to apply the transform <see cref="selector"/> to.</param>
        /// <param name="selector">A transform to apply to each element in <paramref name="source"/>.</param>
        /// <returns>The index of the minimum <typeparamref name="TResult"/> value.</returns>
        public static int MinIndex<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            var comparer = Comparer<TResult>.Default;
            using (var enumerator = source.GetEnumerator())
            {
                enumerator.MoveNext();
                var minElement = selector(enumerator.Current);
                var minIndex = 0;

                var i = 1;
                while (enumerator.MoveNext())
                {
                    if (comparer.Compare(selector(enumerator.Current), minElement) < 0)
                    {
                        minElement = selector(enumerator.Current);
                        minIndex = i;
                    }

                    i++;
                }

                return minIndex;
            }
        }

        /// <summary>
        /// Returns the maximum value in a sequence of <typeparamref name="TSource"/> values using
        /// the default <see cref="Comparer{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the index of the maximum value.</param>
        /// <returns>The index of the maximum value in <paramref name="source"/>.</returns>
        public static int MaxIndex<TSource>(this IEnumerable<TSource> source)
        {
            var comparer = Comparer<TSource>.Default;
            using (var enumerator = source.GetEnumerator())
            {
                enumerator.MoveNext();
                var maxElement = enumerator.Current;
                var maxIndex = 0;

                var i = 1;
                while (enumerator.MoveNext())
                {
                    if (comparer.Compare(enumerator.Current, maxElement) > 0)
                    {
                        maxElement = enumerator.Current;
                        maxIndex = i;
                    }

                    i++;
                }

                return maxIndex;
            }
        }

        /// <summary>
        /// Invokes a transform function on each element of <paramref name="source"/> and returns the index of the maximum <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result of the <paramref name="selector"/>.</typeparam>
        /// <param name="source">A sequence of values to apply the transform <paramref name="selector"/> to.</param>
        /// <param name="selector">A transform to apply to each element in <paramref name="source"/>.</param>
        /// <returns>The index of the maximum <typeparamref name="TResult"/> value.</returns>
        public static int MaxIndex<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            var comparer = Comparer<TResult>.Default;
            using (var enumerator = source.GetEnumerator())
            {
                enumerator.MoveNext();
                var maxElement = selector(enumerator.Current);
                var maxIndex = 0;

                var i = 1;
                while (enumerator.MoveNext())
                {
                    if (comparer.Compare(selector(enumerator.Current), maxElement) > 0)
                    {
                        maxElement = selector(enumerator.Current);
                        maxIndex = i;
                    }

                    i++;
                }

                return maxIndex;
            }
        }

        /// <summary>
        /// Returns the indexes of the elements in <paramref name="source"/> satisfying the predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to filter based on <paramref name="predicate"/>.</param>
        /// <param name="predicate">The predicate use to filter values in <paramref name="source"/>.</param>
        /// <returns>The indexes of the elements in <paramref name="source"/> that satisfy <paramref name="predicate"/>.</returns>
        public static IEnumerable<int> WhereIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var indexes = new List<int>();
            using (var enumerator = source.GetEnumerator())
            {
                var i = 0;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        indexes.Add(i);
                    }

                    i++;
                }

                return indexes;
            }
        }

        /// <summary>
        /// Returns the index of the first element of a sequence that satisfies the given predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to find the first element satisfying the <paramref name="predicate"/>.</param>
        /// <param name="predicate">A predicate to test elements in <paramref name="source"/>.</param>
        /// <returns>The index of the first element in <paramref name="source"/> that satisfy <paramref name="predicate"/>.</returns>
        public static int FirstIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            using (var enumerator = source.GetEnumerator())
            {
                var i = 0;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        return i;
                    }

                    i++;
                }
            }

            throw new InvalidOperationException($"No element satisfies the condition in {nameof(predicate)}.");
        }

        /// <summary>
        /// Returns the index of the first element of a sequence that satisfies the given predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to find the first element satisfying the <paramref name="predicate"/>.</param>
        /// <param name="predicate">A predicate to test elements in <paramref name="source"/>.</param>
        /// <returns>The index of the first element in <paramref name="source"/> that satisfy <paramref name="predicate"/>.
        /// If no element satisfies the <paramref name="predicate"/> then -1 is returned.</returns>
        public static int FirstIndexOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            using (var enumerator = source.GetEnumerator())
            {
                var i = 0;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        return i;
                    }

                    i++;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the last element of a sequence that satisfies the given predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to find the last element satisfying the <paramref name="predicate"/>.</param>
        /// <param name="predicate">A predicate to test elements in <paramref name="source"/>.</param>
        /// <returns>The index of the last element in <paramref name="source"/> that satisfy <paramref name="predicate"/>.</returns>
        public static int LastIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var lastIndex = -1;
            using (var enumerator = source.GetEnumerator())
            {
                var i = 0;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        lastIndex = i;
                    }

                    i++;
                }
            }

            if (lastIndex == -1)
            {
                throw new InvalidOperationException($"No element satisfies the condition in {nameof(predicate)}.");
            }

            return lastIndex;
        }

        /// <summary>
        /// Returns the index of the last element of a sequence that satisfies the given predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to find the last element satisfying the <paramref name="predicate"/>.</param>
        /// <param name="predicate">A predicate to test elements in <paramref name="source"/>.</param>
        /// <returns>The index of the last element in <paramref name="source"/> that satisfy <paramref name="predicate"/>.
        /// If no element satisfies the <paramref name="predicate"/> then -1 is returned.</returns>
        public static int LastIndexOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var lastIndex = -1;
            using (var enumerator = source.GetEnumerator())
            {
                var i = 0;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        lastIndex = i;
                    }

                    i++;
                }
            }

            return lastIndex;
        }

        /// <summary>
        /// Returns all elements of a sequence whose index is in the provided sequence of indexes.
        /// Note: If an index is greater than the number of elements in <paramref name="source"/> it is ignored.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to extract elements from.</param>
        /// <param name="indexes">The indexes </param>
        /// <returns>All elements in <paramref name="source"/> whose index is in <paramref name="indexes"/>.</returns>
        public static IEnumerable<TSource> WithIndexes<TSource>(this IEnumerable<TSource> source, IEnumerable<int> indexes)
        {
            return indexes.Select(source.ElementAt);
        }

        #endregion

        /// <summary>
        /// Gets the index of the elements returned by the <c>IEnumerable.Max()</c> function. 
        /// If there is more than one maximal element then the index return is chosen at random.
        /// </summary>
        /// <param name="array">The input array</param>
        /// <returns>The index of the max element in the array.</returns>
        public static int MaxIndexRandomTies(this double[] array)
        {
            var largestElement = double.MinValue;
            var largestElementIndex = -1;

            for (var i = 0; i < array.Length; i++)
            {
                if (largestElement < array[i])
                {
                    largestElement = array[i];
                    largestElementIndex = i;
                }
            }

            // we have more than one maximal element.Choose a random index
            if (array.Count(x => x == largestElement) > 1)
            {
                var indexes = array.WhereIndex(x => x == largestElement).ToArray();
                var rand = new Random();
                var randomIndex = rand.Next(0, indexes.Length);

                return indexes[randomIndex];

            }

            return largestElementIndex;
        }

        /// <summary>
        /// Gets the index of the elements returned by the <c>IEnumerable.Min()</c> function.
        /// </summary>
        /// <param name="array">The input array</param>
        /// <returns>The index of the min element in the array.</returns>
        public static int MinIndex(this int[] array)
        {
            var smallestElement = int.MaxValue;
            var smallestElementIndex = -1;

            for (var i = 0; i < array.Length; i++)
            {
                if (smallestElement > array[i])
                {
                    smallestElement = array[i];
                    smallestElementIndex = i;
                }
            }

            return smallestElementIndex;
        }

        /// <summary>
        /// Takes a list of integers and returns its bijection with the natural numbers (includes 0)
        /// </summary>
        /// <param name="source">The input array.</param>
        /// <returns>An array that is the bijection of the <paramref name="source"/> array with the natural numbers (includes 0)</returns>
        public static int[] BijectWithNaturals(this int[] source)
        {
            var distinctValues = source.Distinct().OrderBy(x => x).ToArray();
            var dict = new Dictionary<int, int>();
            var returnArray = new int[source.Length];

            for (int i = 0; i < distinctValues.Length; i++)
            {
                dict.Add(distinctValues[i], i);
            }

            for (int i = 0; i < source.Length; i++)
            {
                returnArray[i] = dict[source[i]];
            }

            return returnArray;
        }

        /// <summary>
        /// Sets all elements of a rectangular array to a given value.
        /// </summary>
        /// <param name="source">The rectuangular array.</param>
        /// <param name="value">The value of which all elements should be set to.</param>
        /// <typeparam name="T">The type of the elements.</typeparam>
        public static void SetAllElements<T>(this T[,] source, T value)
        {
            var rows = source.GetLength(0);
            var cols = source.GetLength(1);
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    source[i, j] = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public static T[][] WithIndexes<T>(this T[,] source, int[] indexes)
        {
            var returnArray = new T[indexes.Length][];
            for (int i = 0; i < indexes.Length; i++)
            {
                returnArray[i] = source.GetRow(indexes[i]);
            }

            return returnArray;
        }


        /// <summary>
        /// Performs addition on two arrays
        /// </summary>
        /// <returns></returns>
        public static double[] ArrayAdd(this double[] array1, double[] array2)
        {
            var newArray = new double[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                newArray[i] = array1[i] + array2[i];
            }
            return newArray;
        }

        public static double[] ArraySubtract(this double[] array1, double[] array2)
        {
            var newArray = new double[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                newArray[i] = array1[i] - array2[i];
            }
            return newArray;
        }

        public static double[] ArrayDivide(this double[] array1, double[] array2)
        {
            var newArray = new double[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                // set to positive or negative infinity if the dividing number is 0;
                if (array2[i] == 0)
                {
                    newArray[i] = Math.Sign(array1[i]) >= 0 ? double.PositiveInfinity : double.NegativeInfinity;
                }

                newArray[i] = array1[i] / array2[i];
            }
            return newArray;
        }

        public static T[] GetRow<T>(this T[,] source, int rowIndex)
        {

            var columns = source.GetLength(1);
            var row = new T[columns];

            for (int i = 0; i < columns; i++)
            {
                row[i] = source[rowIndex, i];
            }

            return row;
        }

        public static T[] GetColumn<T>(this T[,] source, int colIndex)
        {

            var rows = source.GetLength(0);
            var column = new T[rows];

            for (int i = 0; i < rows; i++)
            {
                column[i] = source[i, colIndex];
            }

            return column;
        }

        /// <summary>
        ///   Stores a row vector into the given row position of the matrix.
        /// </summary>
        public static T[,] SetRow<T>(this T[,] m, int index, T[] row)
        {
            for (int i = 0; i < row.Length; i++)
                m[index, i] = row[i];

            return m;
        }

        /// <summary>
        ///   Stores a column vector into the given column position of the matrix.
        /// </summary>
        public static T[,] SetColumn<T>(this T[,] m, int index, T[] column)
        {
            for (int i = 0; i < column.Length; i++)
                m[i, index] = column[i];

            return m;
        }

        /// <summary>
        /// Returns the mode of the array
        /// </summary>
        /// <param name="array">The array to be traversed.</param>
        /// <returns>The mode of the aray</returns>
        public static int Mode(this int[] array)
        {
            // Count all the values
            var countDictionary = new Dictionary<int, int>();
            for (int i = 0; i < array.Length; i++)
            {
                if (countDictionary.ContainsKey(array[i]))
                {
                    countDictionary[array[i]]++;
                }
                else
                {
                    countDictionary.Add(array[i], 1);
                }
            }

            // Get the extry with the highest count
            int maxKey = 0;
            int maxValue = 0;
            foreach (KeyValuePair<int, int> kvp in countDictionary)
            {
                if (kvp.Value > maxValue)
                {
                    maxValue = kvp.Value;
                    maxKey = kvp.Key;
                }
            }

            return maxKey;
        }
    }
}
