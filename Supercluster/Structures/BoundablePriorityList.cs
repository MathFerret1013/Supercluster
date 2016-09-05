namespace Supercluster.Structures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A list of limited length that remains sorted by <typeparamref name="TPriority"/>.
    /// Useful for keeping track of items in nearest neighbor searches. Insert is O(log n). Retrieval is O(1)
    /// </summary>
    /// <typeparam name="TElement">The type of element the list maintains.</typeparam>
    /// <typeparam name="TPriority">The type the elements are prioritized by.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class BoundablePriorityList<TElement, TPriority> : IEnumerable<TElement>
        where TPriority : IComparable<TPriority>
    {
        /// <summary>
        /// Used to reverse sort order when the list is not in ascending mode.
        /// </summary>
        private readonly ReverseComparer<TPriority> descendingComparer = new ReverseComparer<TPriority>();

        /// <summary>
        /// The list holding the actual elements
        /// </summary>
        private List<TElement> elementList;

        /// <summary>
        /// The list of priorities for each element.
        /// There is a one-to-one correspondence between the
        /// priority list ad the element list.
        /// </summary>
        private List<TPriority> priorityList;

        /// <summary>
        /// Gets the element with the largest priority.
        /// </summary>
        public TElement MaxElement => this.IsAscending ? this.elementList[this.Count - 1] : this.elementList[0];

        /// <summary>
        /// Gets the largest priority. currently in the list.
        /// </summary>
        public TPriority MaxPriority => this.IsAscending ? this.priorityList[this.Count - 1] : this.priorityList[0];

        /// <summary>
        /// Gets the element with the lowest priority.
        /// </summary>
        public TElement MinElement => this.IsAscending ? this.elementList[0] : this.elementList[this.Count - 1];

        /// <summary>
        /// Gets the smallest priority.
        /// </summary>
        public TPriority MinPriority => this.IsAscending ? this.priorityList[0] : this.priorityList[this.Count - 1];

        /// <summary>
        /// Gets the maximum allowed capacity for this list.
        /// If this instance does not have a max capacity set then the result is -1.
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// Returns true if the list is at maximum capacity. This is always false if the list was not initialized with a capacity.
        /// </summary>
        public bool IsFull => this.Count == this.Capacity;

        /// <summary>
        /// Returns true if the list is sorted in ascending priority. Otherwise the return value is false
        /// and the list is sorted in descending priority.
        /// </summary>
        public bool IsAscending { get; }

        /// <summary>
        /// Returns the count of items currently in the list.
        /// </summary>
        public int Count => this.priorityList.Count;

        /// <summary>
        /// Indexer for the internal element array.
        /// </summary>
        /// <param name="index">The index in the array.</param>
        /// <returns>The element at the specified index.</returns>
        public TElement this[int index] => this.elementList[index];

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundablePriorityList{TElement, TPriority}"/> class.
        /// Note: You should not have <paramref name="allocate"/> set to true, and the capacity set to a very large number.
        /// Especially if you will be creating and destroying many <see cref="BoundablePriorityList{TElement,TPriority}"/> very rapidly.
        /// If you ignore this advice you will create lots of memory pressure. If you don't understand why this is a problem you should
        /// understand the garbage collector. Please read: https://msdn.microsoft.com/en-us/library/ee787088.aspx
        /// </summary>
        /// <param name="capacity">The maximum capacity of the list. If this number is negative then the list has infinite capacity.</param>
        /// <param name="ascending">Specifies if the list should sort (by priority) in an ascending or descending fashion.</param>
        /// <param name="allocate">If true, initializes the internal lists for the <see cref="BoundablePriorityList{TElement,TPriority}"/> with an initial capacity of <paramref name="capacity"/>.</param>
        public BoundablePriorityList(int capacity = -1, bool ascending = true, bool allocate = false)
        {
            this.IsAscending = ascending;
            this.Capacity = capacity;
            if (allocate)
            {
                this.priorityList = new List<TPriority>(capacity);
                this.elementList = new List<TElement>(capacity);
            }
            else
            {
                this.priorityList = new List<TPriority>();
                this.elementList = new List<TElement>();
            }
        }

        /// <summary>
        /// Attempts to add the provided  <paramref name="item"/>. If the list
        /// is currently at maximum capacity and the elements priority is greater
        /// than or equal to the highest priority, the <paramref name = "item"/> is not inserted. If the
        /// <paramref name = "item"/> is eligible for insertion, the upon insertion the <paramref name = "item"/> that previously
        /// had the largest priority is removed from the list.
        /// This is an O(log n) operation.
        /// </summary>
        /// <param name="item">The item to be inserted</param>
        /// <param name="priority">The priority of th given item.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TElement item, TPriority priority)
        {
            // Note that if no capacity was set this is never true
            if (this.Count >= this.Capacity && this.Capacity > 0)
            {
                // Determine if the item should be inserted
                if (this.IsAscending
                        ? this.MaxPriority.CompareTo(priority) < 0
                        : this.MinPriority.CompareTo(priority) > 0)
                {
                    return;
                }

                var index = this.IsAscending
                                ? this.priorityList.BinarySearch(priority)
                                : this.priorityList.BinarySearch(priority, this.descendingComparer);
                index = index >= 0 ? index : ~index;

                this.priorityList.Insert(index, priority);
                this.elementList.Insert(index, item);

                this.priorityList.RemoveAt(this.priorityList.Count - 1);
                this.elementList.RemoveAt(this.elementList.Count - 1);
            }
            else
            {
                var index = this.IsAscending
                                ? this.priorityList.BinarySearch(priority)
                                : this.priorityList.BinarySearch(priority, this.descendingComparer);
                index = index >= 0 ? index : ~index;

                this.priorityList.Insert(index, priority);
                this.elementList.Insert(index, item);
            }
        }

        /// <summary>
        /// Removes all elements that satisfy the given <paramref name="predicate"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAllPriorities(Func<TPriority, bool> predicate)
        {
            var indexesToKeep = this.priorityList.WhereIndex(p => !predicate(p)).ToArray();
            this.priorityList = this.priorityList.WithIndexes(indexesToKeep).ToList();
            this.elementList = this.elementList.WithIndexes(indexesToKeep).ToList();
        }

        /// <summary>
        /// Removes all elements that satisfy the given <paramref name="predicate"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAllElements(Func<TElement, bool> predicate)
        {
            var indexesToKeep = this.elementList.WhereIndex(e => !predicate(e)).ToArray();
            this.priorityList = this.priorityList.WithIndexes(indexesToKeep).ToList();
            this.elementList = this.elementList.WithIndexes(indexesToKeep).ToList();
        }


        /// <summary>
        /// Removes an element the corresponding priority at the specified index.
        /// </summary>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            this.priorityList.RemoveAt(index);
            this.elementList.RemoveAt(index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            return this.elementList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Tuple<TElement, TPriority>[] ToPairedList()
        {
            return this.elementList.Zip(this.priorityList, (e, p) => new Tuple<TElement, TPriority>(e, p)).ToArray();
        }
    }

    /// <summary>
    /// An <see cref="IComparer"/> that reverses the result of the <see cref="Compare"/> function.
    /// Used to reverse sort order.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReverseComparer<T> : IComparer<T>
    {
        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            return Comparer<T>.Default.Compare(y, x);
        }
    }
}