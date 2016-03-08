using System;

namespace Supercluster.KDTree
{
    using System.Collections.Generic;
    using System.Linq;

    using Supercluster.Algorithms;

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

        /// <summary>
        /// The parent of the current node. If the current node is the root node, then its parent is null.
        /// </summary>
        public KDNode Parent = null;

        public KDNode(double[] value = null, KDNode parent = null, KDNode left = null, KDNode right = null)
        {
            this.Value = value;
            this.Left = left;
            this.Right = right;
            this.Parent = parent;
        }
    }


    public class KDTree
    {
        public KDNode Root = new KDNode(null, null);

        public int Count;

        private int test = -1;

        public KDTree(int k, IEnumerable<double[]> points)
        {
            if (k < 1)
            {
                throw new ArgumentException("You must have at least 2 dim.");
            }

            this.K = k;
            this.GrowTree(ref this.Root, null, points.ToArray(), 0);
            this.Count = points.Count();
        }


        public FallThroughStack<KDNode> NearestPoints = new FallThroughStack<KDNode>(3);

        public KDNode FindNearestNeighbor(double[] point)
        {
            // see where point would be inserted
            var parentAfterInsertWithDim = this.FalseInsert(this.Root, point, 0);
            var currentBest = parentAfterInsertWithDim.Item1;
            var dim = parentAfterInsertWithDim.Item2;

            // set the point as the curent "best"
            var bestDist = Norms.L2Norm_Squared(point, currentBest.Value);

            var visitedNodes = new List<KDNode>();

            // start unwinding the recursion
            var previousNode = currentBest;
            var currentNode = currentBest.Parent;
            dim = (dim + 1) % this.K;
            visitedNodes.Add(previousNode);

            while (currentNode != null)
            {
                // go up and test
                var currentDist = Norms.L2Norm_Squared(point, currentNode.Value);

                if (currentDist < bestDist) // we found a better node
                {
                    bestDist = currentDist;
                    currentBest = currentNode;
                }


                // check if there could be points on the other side of the hyper plane
                var hyperPlaneDist = Math.Pow(point[dim] - currentNode.Value[dim], 2);
                if (hyperPlaneDist < bestDist)
                {
                    // The hyper plane intersects the hyper sphere so we check the OTHER side of the branch.
                    // We check which side the previous node was on.
                    // If greater than 0 we were a left child, other wise, we were a right child
                    bool leftSide = currentNode.Value[dim] - previousNode.Value[dim] > 0;
                    var nextNode = leftSide ? currentNode.Right : currentNode.Left;

                    if (nextNode != null && // We can only go down if the brach exists
                        !visitedNodes.Contains(nextNode)) // Don't go down to the node if we visited it before
                    {
                        // Now we go down the opposite side of the branch
                        var nextInsertNode = this.FalseInsert(nextNode, point, (dim + 1) % this.K);
                        dim = nextInsertNode.Item2;
                        currentNode = nextInsertNode.Item1;
                    }
                    else
                    {
                        previousNode = currentNode;
                        currentNode = currentNode.Parent;
                        dim = (dim + 1) % this.K;
                        visitedNodes.Add(previousNode);
                    }
                }
                else // The hyperplane does not intersect, move up the branch
                {
                    previousNode = currentNode;
                    currentNode = currentNode.Parent;
                    dim = (dim + 1) % this.K;
                    visitedNodes.Add(previousNode);
                }
            }

            return currentBest;
        }

        /// <summary>
        /// Grows a KD tree recursively
        /// </summary>
        /// <param name="currentNode">The current node in the recursion.</param>
        /// <param name="previousNode">The node in the previous level of the recursion. This is als the parent of the cirrent node.</param>
        /// <param name="points">The set of points remaining to be added to the kd-tree</param>
        /// <param name="dim">The current splitting dimension of the kd-tree. This is the depth mod K, where K is the dimensionality of the data set and depth is the depth of the tree.</param>
        private void GrowTree(ref KDNode currentNode, KDNode previousNode, double[][] points, int dim)
        {
            // See wikipedia for a good explanation kd-tree creation.
            // https://en.wikipedia.org/wiki/K-d_tree

            // sort the points along the current dimension
            var sortedPoints = points.OrderBy(p => p[dim]).ToArray();

            // get the point which has the median value of the current dimension.
            var medianPoint = sortedPoints[points.Length / 2];
            var medianPointIdx = sortedPoints.Length / 2;

            // The point with the median value all the current dimension now becomes the value of the current tree node
            // The previous node becomes the parents of the current node.
            currentNode = new KDNode(value: medianPoint, parent: previousNode);
            previousNode = currentNode; // Set the previous node to the current for the next level of recursion.

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
                    currentNode.Left = new KDNode { Value = leftPoints[0], Parent = previousNode };
                }
            }
            else
            {
                this.GrowTree(ref currentNode.Left, currentNode, leftPoints, nextDim);
            }

            // Do the same for the right points
            if (rightPoints.Length <= 1)
            {
                if (rightPoints.Length == 1)
                {
                    currentNode.Right = new KDNode { Value = rightPoints[0], Parent = previousNode };
                }
            }
            else
            {
                this.GrowTree(ref currentNode.Right, currentNode, rightPoints, nextDim);
            }
        }

        /// <summary>
        /// Attempts to insert a node to the given subtree, but doesn't actually insert it.
        /// </summary>
        /// <param name="root">The root node of the sub-tree</param>
        /// <param name="point">The point that would be inserted.</param>
        /// <param name="dim">The splitting dimension of the <paramref name="root"/> parameter.</param>
        /// <returns>The node that would be the parent of the inserted node.</returns>
        private Tuple<KDNode, int> FalseInsert(KDNode root, double[] point, int dim)
        {
            var node = root;

            // Keeps track of the nodes
            var lastNode = node;

            while (true)
            {
                node = node.Value[dim] < point[dim] ? node.Right : node.Left;

                if (node == null)
                {
                    break;
                }

                lastNode = node;
                dim = (dim + 1) % this.K;
            }

            return new Tuple<KDNode, int>(lastNode, dim);
        }

        /// <summary>
        /// The dimenonality of the tree.
        /// </summary>
        public int K { get; }

    }
}
