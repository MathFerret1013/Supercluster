namespace Supercluster
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a sorted array of a finite length.
    /// </summary>
    /// <typeparam name="T">The type of the array. <typeparamref name="T"/> must implement</typeparam>
    public class SortedArray<T> : IEnumerable<T>
        where T : IComparable<T>
    {
        private readonly bool ascending;
        private readonly T[] internalArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedArray{T}"/> class.
        /// </summary>
        /// <param name="length">Length of the array</param>
        /// <param name="initialValue">Specifies the starting value of the array. This is crucial to the comparison of array elements.</param>
        /// <param name="ascending">Specifies wether the array is sorted ascending or descending</param>
        public SortedArray (int length, T initialValue, bool ascending = true)
        {
            this.internalArray = Enumerable.Repeat(initialValue, length).ToArray();
            this.ascending = ascending;
        }

        /// <summary>
        /// Gets an element of the sorted array by index.
        /// </summary>
        /// <param name="index">The index of the elemen to be returned.</param>
        public T this[int index] => this.internalArray[index];

        /// <summary>
        /// Adds an element to the array. If the element is too large or too small, it may not be added. Other elements are appropriately shifted or removed.
        /// </summary>
        /// <param name="input">The input element to be potentially added to the array.</param>
        public void Add(T input)
        {
            var indexSet = -1;

            // determine if we should set the index
            for (var i = 0; i < this.internalArray.Length; i++)
            {
                if (this.ascending)
                {
                    if (input.CompareTo(this.internalArray[i]) < 0)
                    {
                        indexSet = i;
                        break;
                    }
                }
                else
                {
                    if (input.CompareTo(this.internalArray[i]) > 0)
                    {
                        indexSet = i;
                        break;
                    }
                }
            }

            // if there is not index to set then exit
            if (indexSet == -1)
            {
                return;
            }

            // shift values down
            for (int i = this.internalArray.Length - 1; indexSet < i; i--)
            {
                this.internalArray[i] = this.internalArray[i - 1];
            }

            // Set the values we need
            this.internalArray[indexSet] = input;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.internalArray.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalArray.GetEnumerator();
        }
    }
}
