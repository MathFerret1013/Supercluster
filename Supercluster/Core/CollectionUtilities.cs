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
        /// <summary>
        /// Gets the index of the elements returned by the <c>IEnumerable.Max()</c> function.
        /// </summary>
        /// <param name="array">The input array</param>
        /// <returns>The index of the max element in the array.</returns>
        public static int MaxIndex(this int[] array)
        {
            var largestElement = int.MinValue;
            var largestElementIndex = -1;

            for (var i = 0; i < array.Length; i++)
            {
                if (largestElement < array[i])
                {
                    largestElement = array[i];
                    largestElementIndex = i;
                }
            }

            return largestElementIndex;
        }

        /// <summary>
        /// Gets the index of the elements returned by the <c>IEnumerable.Max()</c> function.
        /// </summary>
        /// <param name="array">The input array</param>
        /// <returns>The index of the max element in the array.</returns>
        public static int MaxIndex(this double[] array)
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

            return largestElementIndex;
        }


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
                var indexes = array.FindAllIndex(x => x == largestElement).ToArray();
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
        /// Gets the index of the elements returned by the <c>IEnumerable.Min()</c> function.
        /// </summary>
        /// <param name="array">The input array</param>
        /// <returns>The index of the min element in the array.</returns>
        public static int MinIndex(this double[] array)
        {
            var smallestElement = double.MaxValue;
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
        /// Returns the indexes of all elements in an array that satisfy a given predicate.
        /// </summary>
        /// <typeparam name="T">The data type of the array.</typeparam>
        /// <param name="source">The collection to enumerate through and test the predicate.</param>
        /// <param name="predicate">The predicate to be test.</param>
        /// <returns>The indexes of all elements in the collection that satisfy the given predicate.</returns>
        public static IEnumerable<int> FindAllIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            int i = 0;
            var indexes = new List<int>();
            var enumerator = source.GetEnumerator();
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
        public static T[] SubsetByIndex<T>(this IList<T> source, int[] indexes)
        {
            var returnArray = new T[indexes.Length];
            for (int i = 0; i < indexes.Length; i++)
            {
                returnArray[i] = source[indexes[i]];
            }

            return returnArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public static T[][] SubsetByIndex<T>(this T[,] source, int[] indexes)
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
