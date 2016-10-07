namespace Supercluster.Structures.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// An append only data structure that supports basic spatial access methods.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the collection.</typeparam>
    public interface ISpatialQueryable<T>
    {
        /// <summary>
        /// Add the items to the current collection/
        /// </summary>
        /// <param name="items">The items to be added.</param>
        /// <returns>The indexes of the items added.</returns>
        IEnumerable<int> Add(IEnumerable<T> items);

        /// <summary>
        /// Adds the item to the current collection.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <returns>The index of the item added.</returns>
        int Add(T item);

        /// <summary>
        /// Searches for the <paramref name="k"/> nearest neighbors of the specified <paramref name="target"/>.
        /// Note that the target point may or may not be in the collection.
        /// </summary>
        /// <param name="target">The point whose neighbors will be found.</param>
        /// <param name="k">The number of neighbors find.</param>
        /// <returns>The <paramref name="k"/> nearest neighbors of the <paramref name="target"/> point.</returns>
        IEnumerable<T> NearestNeighbors(T target, int k);

        /// <summary>
        /// Searches for the <paramref name="k"/> nearest neighbors of the specified <paramref name="target"/>.
        /// Note that the target point may or may not be in the collection.
        /// </summary>
        /// <param name="target">The point whose neighbors will be found.</param>
        /// <param name="k">The number of neighbors find.</param>
        /// <returns>The <paramref name="k"/> indexes nearest neighbors of the <paramref name="target"/> point.</returns>
        IEnumerable<int> NearestNeighborIndexes(T target, int k);

        /// <summary>
        /// Searches for all points within the ball of the specified <paramref name="radius"/>.
        /// </summary>
        /// <param name="center">The center of the ball to be searched.</param>
        /// <param name="radius">The radius of the ball to be searched.</param>
        /// <returns>The points that are in the ball defined by <paramref name="center"/> and <paramref name="radius"/>.</returns>
        IEnumerable<T> RadialSearch(T center, double radius);

        /// <summary>
        /// Searches for all points within the ball of the specified <paramref name="radius"/>.
        /// </summary>
        /// <param name="center">The center of the ball to be searched.</param>
        /// <param name="radius">The radius of the ball to be searched.</param>
        /// <returns>The points that are in the ball defined by <paramref name="center"/> and <paramref name="radius"/>.</returns>
        IEnumerable<int> RadialSearchIndexes(T center, double radius);

        /// <summary>
        /// Gets the instance of <typeparamref name="T"/> with the specified index. The index of an item to be retrieved should be found through the <see cref="NearestNeighborIndexes"/> or <see cref="RadialSearchIndexes"/>
        /// </summary>
        /// <param name="index">The index of the item in the internal backing array of the data-structure.</param>
        /// <returns>The instance of <typeparamref name="T"/> at the specified index.</returns>
        T this[int index] { get; }

        /// <summary>
        /// Gets the instances of <typeparamref name="T"/> with an index in <paramref name="indexes"/>. The index of an item to be retrieved should be found through the <see cref="NearestNeighborIndexes"/> or <see cref="RadialSearchIndexes"/>
        /// </summary>
        /// <param name="indexes">A collection of indexes pointing to instances of <typeparamref name="T"/> in the internal backing array of the data-structure.</param>
        /// <returns>The instances of <typeparamref name="T"/> at the specified indexes.</returns>
        IEnumerable<T> this[IEnumerable<int> indexes] { get; }
    }
}
