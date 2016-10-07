namespace Supercluster.Structures
{
    using System.Collections.Generic;
    using Supercluster.Structures.Interfaces;

    /// <summary>
    /// The class provides a facade around an <see cref="ISpatialQueryable{TValue}"/> and a <see cref="IDictionary{TKey,TValue}"/>
    /// This allows consumers of the API to easily retrieve the items in a given cluster.
    /// </summary>
    /// <typeparam name="TKey">The type of the key or label for a cluster.</typeparam>
    /// <typeparam name="TValue">The type of the elements in a cluster.</typeparam>
    public class ClusterDictionary<TKey, TValue>
    {
        /// <summary>
        /// The number of clusters in the dictionary.
        /// </summary>
        public int Count => this.clusterDictionary.Keys.Count;

        /// <summary>
        /// The reference to the elements indexes corresponding to each cluster.
        /// </summary>
        private IDictionary<TKey, List<int>> clusterDictionary { get; }

        /// <summary>
        /// The reference to the <see cref="ISpatialQueryable{TValue}"/>.
        /// </summary>
        private ISpatialQueryable<TValue> SourceDataStructure { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="source">The <see cref="ISpatialQueryable{T}"/></param>
        /// <param name="internalDictionary">The <see cref="IDictionary{TKey,TValue}"/></param>
        public ClusterDictionary(ISpatialQueryable<TValue> source, IDictionary<TKey, List<int>> internalDictionary = null)
        {
            this.SourceDataStructure = source;
            this.clusterDictionary = internalDictionary ?? new Dictionary<TKey, List<int>>();
        }

        /// <summary>
        /// Gets the elements of the cluster with the given key.
        /// </summary>
        /// <param name="clusterLabel">The label of the items in the cluster.</param>
        /// <returns>The indexes of the items in the specified cluster.</returns>
        public IEnumerable<TValue> this[TKey clusterLabel] => this.SourceDataStructure[this.clusterDictionary[clusterLabel]];

        /// <summary>
        /// Returns true if the dictionary contains a cluster with th given label.
        /// </summary>
        /// <param name="clusterLabel">The label</param>
        /// <returns></returns>
        public bool ContainsKey(TKey clusterLabel) => this.clusterDictionary.ContainsKey(clusterLabel);
    }
}
