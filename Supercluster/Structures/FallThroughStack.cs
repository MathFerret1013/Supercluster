namespace Supercluster
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A stack that has a max Capacity When elements are pushed to the stack when the stack is full,
    /// elements at the bottom of the stack "fall through" and are removed from the bottom of the stack.
    /// </summary>
    public class FallThroughStack<T> : IEnumerable<T>
    {
        /// <summary>
        /// The capacity of the stack.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// The internal array storing the values.
        /// </summary>
        private T[] InternalArray { get; }



        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="capacity">The capacity of the stack.</param>
        /// <exception cref="ArgumentException">Throws exception if the given capacity is less than 1.</exception>
        public FallThroughStack(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentException("The stack must have more than one element.");
            }

            this.Capacity = capacity;
            this.InternalArray = new T[capacity];
        }

        /// <summary>
        /// Returns the element at the top of the stack without removing it.
        /// </summary>
        /// <returns>The element at the top of the stack</returns>
        public T Peek()
        {
            return this.InternalArray[0];
        }

        /// <summary>
        /// Removes and returns the element at the top of the stack.
        /// </summary>
        /// <returns>The element at the top of the stack</returns>
        public T Pop()
        {
            var returnElement = this.InternalArray[0];

            // shift elements
            for (int i = 0; i < this.Capacity - 1; i++)
            {
                this.InternalArray[i] = this.InternalArray[i + 1];
            }

            this.InternalArray[this.Capacity - 1] = default(T);
            return returnElement;
        }

        /// <summary>
        /// Adds a new element at the top of the stack. If the stack is already at maximum capacity
        /// then the element at the bottom of the stack is removed.
        /// </summary>
        /// <param name="element">Item to add to the stack.</param>
        public void Push(T element)
        {
            this.InternalArray[this.Capacity - 1] = default(T);

            for (int i = this.Capacity - 1; i > 0; i--)
            {
                this.InternalArray[i] = this.InternalArray[i - 1];
            }

            this.InternalArray[0] = element;
        }

        /// <summary>
        /// Gets the element in the stack at the specified index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The secified element in the stack.</returns>
        public T this[int index] => this.InternalArray[index];

        /// <summary>
        /// Gets an enumerator
        /// </summary>
        /// <returns>An enumerator</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.InternalArray.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator
        /// </summary>
        /// <returns>An enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.InternalArray.GetEnumerator();
        }
    }
}
