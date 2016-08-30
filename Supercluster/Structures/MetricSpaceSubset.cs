namespace Supercluster.Structures
{
    using Supercluster.Structures.Interfaces;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a subset of points which belong to a metric space implicitly defined by the <see cref="Metric"/> property.
    /// This is essentially a <see cref="List{T}"/> which implements <see cref="ISpatialQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the data in the collection.</typeparam>
    public class MetricSpaceSubset<T> : ISpatialQueryable<T>, IEnumerable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetricSpaceSubset{T}"/> class.
        /// </summary>
        /// <param name="source">A set of initial values for the collections.</param>
        /// <param name="metric">The metric function which implicitly determines a metric space.</param>
        public MetricSpaceSubset(IEnumerable<T> source, Func<T, T, double> metric)
        {
            this.source = source.ToList();
            this.Metric = metric;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricSpaceSubset{T}"/> class.
        /// </summary>
        /// <param name="metric">The metric function which implicitly determines a metric space.</param>
        public MetricSpaceSubset(Func<T, T, double> metric)
        {
            this.source = new List<T>();
            this.Metric = metric;
        }

        /// <summary>
        /// The metric used for distance calculations. It is assumed that this metric satisfies
        /// all axioms of a metric.
        /// </summary>
        public Func<T, T, double> Metric { get; }

        /// <summary>
        /// The internal collection of elements
        /// </summary>
        private List<T> source;

        /// <inheritdoc />
        public int Add(T item)
        {
            this.source.Add(item);
            return this.source.Count - 1;
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerable<T> NearestNeighbors(T target, int k)
        {
            var boundedPriorityList = new BoundablePriorityList<T, double>(k);

            foreach (var point in this.source)
            {
                boundedPriorityList.Add(point, this.Metric(point, target));
            }

            return boundedPriorityList;
        }

        /// <inheritdoc />
        public IEnumerable<int> NearestNeighborIndexes(T target, int k)
        {
            var boundedPriorityList = new BoundablePriorityList<int, double>(k);

            for (var i = 0; i < this.source.Count; i++)
            {
                boundedPriorityList.Add(i, this.Metric(this.source[i], target));
            }

            return boundedPriorityList;
        }

        /// <inheritdoc />
        public IEnumerable<T> RadialSearch(T center, double radius)
        {
            return this.source.Where(point => this.Metric(point, center) <= radius).ToList();
        }

        /// <inheritdoc />
        public IEnumerable<int> RadialSearchIndexes(T center, double radius)
        {
            return this.source.FindAllIndex(point => this.Metric(point, center) <= radius);
        }

        /// <inheritdoc />
        public T this[int index] => this.source[index];

        /// <inheritdoc />
        public IEnumerable<T> this[IEnumerable<int> indexes] => this.source.WithIndexes(indexes);
    }
}