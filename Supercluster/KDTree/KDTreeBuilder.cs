using System;

namespace Supercluster.KDTree
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// The node of a KDTree.
    /// </summary>
    public class KDNode
    {
        /// <summary>
        /// The value of the tree node.
        /// </summary>
        public double[] Value;

        /// <summary>
        /// The left child of the node.
        /// </summary>
        public KDNode Left = null;

        /// <summary>
        /// The right child of of the node;
        /// </summary>
        public KDNode Right = null;

        public KDNode(double[] value = null, KDNode left = null, KDNode right = null)
        {
            this.Value = value;
            this.Left = left;
            this.Right = right;
        }
    }


    public class KDTree
    {
        public KDNode Root = new KDNode();

        private int test = -1;

        public KDTree(int k, IEnumerable<double[]> points)
        {
            if (k < 1)
            {
                throw new ArgumentException("You must have at least 2 dim.");
            }

            this.K = k;
            this.GrowTree(ref this.Root, points.ToArray(), 0);
        }



        private void GrowTree(ref KDNode localRoot, double[][] points, int dim)
        {
            // See wikipedia for a good explanation kd-tree creation.
            // https://en.wikipedia.org/wiki/K-d_tree

            // sort the points along the current dimension
            var sortedPoints = points.OrderBy(p => p[dim]).ToArray();

            // get the point which has the median value of the current dimension.
            var medianPoint = sortedPoints[points.Length / 2];
            var medianPointIdx = sortedPoints.Length / 2;

            // The point with the median value all the current dimension now becomes the value of the current tree node
            localRoot = new KDNode(value: medianPoint);

            // We now split the sorted points into 2 groups
            // 1st group: points before the median
            var leftPoints = new double[medianPointIdx][];
            Array.Copy(sortedPoints, leftPoints, leftPoints.Length);

            // 2nd group: Points after the median
            var rightPoints = new double[sortedPoints.Length - (medianPointIdx + 1)][];
            Array.Copy(sortedPoints, medianPointIdx + 1, rightPoints, 0, rightPoints.Length);

            // We new recurse, passing the left and right arrays for arguments.
            // The current node's left and right values become the "roots" for
            // each recursion call. We also forward cycle to the next dimension.

            var nextDim = (dim + 1) % this.K; // select next dimension

            // We only need to recurse if the point array contains more than one point
            // If the array has no points then the node stay a null value

            if (leftPoints.Length <= 1)
            {
                if (leftPoints.Length == 1)
                {
                    localRoot.Left = new KDNode { Value = leftPoints[0] };
                }
            }
            else
            {
                this.GrowTree(ref localRoot.Left, leftPoints, nextDim);
            }

            // Do the same for the right points
            if (rightPoints.Length <= 1)
            {
                if (rightPoints.Length == 1)
                {
                    localRoot.Right = new KDNode { Value = rightPoints[0] };
                }
            }
            else
            {
                this.GrowTree(ref localRoot.Right, rightPoints, nextDim);
            }
        }

        public int K { get; }

    }
}
