using System.Collections.Generic;

namespace Supercluster.Structures
{
    using System.Linq;

    using Supercluster.Structures.Interfaces;

    public class ClusterDictionary<TKey, TValue>
    {
        public int Count => this.clusterDictionary.Keys.Count;

        private IDictionary<TKey, List<int>> clusterDictionary { get; }

        private ISpatialQueryable<TValue> SourceDataStructure { get; }

        public ClusterDictionary(ISpatialQueryable<TValue> source, IDictionary<TKey, List<int>> internalDictionary)
        {
            this.SourceDataStructure = source;
            this.clusterDictionary = internalDictionary;
        }

        public IEnumerable<TValue> this[TKey index] => this.SourceDataStructure[this.clusterDictionary[index]];

        internal void AddCluster(TKey clusterLabel, IEnumerable<int> indexesForCluster)
        {
            this.clusterDictionary.Add(clusterLabel, indexesForCluster.ToList());
        }

        internal void AddCluster(TKey clusterLabel)
        {
            this.clusterDictionary.Add(clusterLabel, new List<int>());
        }

        public bool ContainsKey(TKey clusterLabel) => this.clusterDictionary.ContainsKey(clusterLabel);

        internal bool RemoveCluster(TKey clusterLabel)
        {
            return this.clusterDictionary.Remove(clusterLabel);
        }


    }
}
