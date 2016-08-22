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
        /// <param name="source">I A set of initial values for the collections.</param>
        /// <param name="metric">The metric function which implicitly determines a metric space.</param>
        public MetricSpaceSubset(IEnumerable<T> source, Func<T, T, double> metric)
        {
            this.Source = source.ToList();
            this.Metric = metric;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricSpaceSubset{T}"/> class.
        /// </summary>
        /// <param name="metric">The metric function which implicitly determines a metric space.</param>
        public MetricSpaceSubset(Func<T, T, double> metric)
        {
            this.Source = new List<T>();
            this.Metric = metric;
        }

        /// <summary>
        /// The metric used for distance calculations. It is assumed that this metric satisfies
        /// all axioms of a metric.
        /// </summary>
        public Func<T, T, double> Metric { get; }

        /// <summary>
        /// The internal collection of
        /// </summary>
        private List<T> Source { get; }

        /// <inheritdoc />
        public void Add(T item)
        {
            this.Source.Add(item);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return this.Source.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerable<T> NearestNeighbors(T target, int k)
        {
            var boundedPriorityList = new BoundedPriorityList<T, double>(k);

            foreach (var point in this.Source)
            {
                boundedPriorityList.Add(point, this.Metric(point, target));
            }

            return boundedPriorityList;
        }

        /// <inheritdoc />
        public IEnumerable<T> RadialSearch(T center, double radius)
        {
            return this.Source.Where(point => this.Metric(point, center) <= radius).ToList();
        }
    }
}