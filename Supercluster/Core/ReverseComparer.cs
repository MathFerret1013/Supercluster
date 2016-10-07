namespace Supercluster.Core
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// An <see cref="IComparer"/> that reverses the result of the <see cref="Compare"/> function.
    /// Used to reverse sort order.
    /// </summary>
    /// <typeparam name="T">The type of the object to be prepared.</typeparam>
    public class ReverseComparer<T> : IComparer<T>
    {
        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            return Comparer<T>.Default.Compare(y, x);
        }
    }
}