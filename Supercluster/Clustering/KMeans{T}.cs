using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supercluster.Clustering
{
    using Microsoft.Win32.SafeHandles;

    /// <summary>
    /// <i>k</i>-Means Clustering Algorithm
    /// </summary>
    public class KMeans<T>
    {
        public List<T> ClusterData;

        public List<int> ClusterLabels;

        public T[] Centroids;

        public int Clusters { get; private set; }

        public Func<T, T, double> Metric;

        public Func<IEnumerable<T>, T> CentralTendency;

        public IEqualityComparer<T> EqualityComparer;

        public KMeans(int clusters, IEqualityComparer<T> equality)
        {
            this.Clusters = clusters;
            this.ClusterData = new List<T>();
            this.ClusterLabels = new List<int>();

            this.Centroids = new T[this.Clusters];

            this.EqualityComparer = equality;
        }


        public IList<T> GetClusterData(int clusterLabel)
        {
            var pointList = new List<T>();
            for (int pointIndex = 0; pointIndex < this.ClusterData.Count; pointIndex++)
            {
                if (this.ClusterLabels[pointIndex] == clusterLabel)
                {
                    pointList.Add(this.ClusterData[pointIndex]);
                }
            }

            return pointList;
        }

        public void Compute()
        {
            // holds the distances between centroids and current Point
            var centroidDistances = new double[this.Clusters];
            var lastCentroidValues = new T[this.Clusters];

            // keep recalculating centroid until they converge
            while (true)
            {
                Array.Copy(this.Centroids, lastCentroidValues, this.Clusters);

                // for each data Point
                for (int pointIndex = 0; pointIndex < this.ClusterData.Count; pointIndex++)
                {
                    // Calculate distance from each centroid
                    for (int centroidIndex = 0; centroidIndex < this.Centroids.Length; centroidIndex++)
                    {
                        centroidDistances[centroidIndex] = this.Metric(
                            this.ClusterData[pointIndex],
                            this.Centroids[centroidIndex]);
                    }

                    // Choose the centroid that is closest
                    var minIndex = centroidDistances.MinIndex();

                    // Set the Point as belonging to the closest centroid
                    this.ClusterLabels[pointIndex] = minIndex;
                }

                // recalculate each centroid
                // TODO: Optimize this code block
                for (int centroidIndex = 0; centroidIndex < this.Centroids.Length; centroidIndex++)
                {
                    // Get all the points for the current centroid
                    // Recalculate the centroid for this class.
                    this.Centroids[centroidIndex] = this.CentralTendency(this.GetClusterData(centroidIndex));
                }

                if (this.Centroids.SequenceEqual(lastCentroidValues, this.EqualityComparer))
                {
                    // We have coverged we are done
                    break;
                }
            }
         }
    }
}
