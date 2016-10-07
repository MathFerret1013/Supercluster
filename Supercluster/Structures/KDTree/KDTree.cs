// <copyright file="KDTree.cs" company="Eric Regina">
// Copyright (c) Eric Regina. All rights reserved.
// </copyright>

namespace Supercluster.Structures.KDTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Supercluster.Core;
    using Supercluster.DataStructures.KDTree;
    using Supercluster.Structures;
    using Supercluster.Structures.Interfaces;

    public class CoordinateWithIndex
    {
        public double[] Coordinate { get; }

        public int Index { get; }

        public CoordinateWithIndex(int index, double[] Coordinate)
        {
            this.Index = index;
            this.Coordinate = Coordinate;
        }
    }

    /// <summary>
    /// The node of a KDTree.
    /// </summary>
    public class KDNode
    {
        /// <summary>
        /// The index of the element in the internal array.
        /// </summary>
        public int ElementIndex { get; set; }

        /// <summary>
        /// The left child of the node.
        /// </summary>
        public KDNode Left;

        /// <summary>
        /// The right child of of the node;
        /// </summary>
        public KDNode Right;

        /// <summary>
        /// The parent of the current node. If the current node is the root node, then its parent is null.
        /// </summary>
        public KDNode Parent;

        public KDNode(int index, KDNode parent = null, KDNode left = null, KDNode right = null)
        {
            this.ElementIndex = index;
            this.Left = left;
            this.Right = right;
            this.Parent = parent;
        }
    }

    /// <summary>
    /// Represents a KD-Tree. KD-Trees are used for fast spatial searches. Searching in a
    /// balanced KD-Tree is O(log n) where linear search is O(n). Points in the KD-Tree are
    /// equi-length arrays of type <typeparamref name="double"/>. The node objects associated
    /// with the points is an array of type <typeparamref name="TNode"/>.
    /// </summary>
    /// <remarks>
    /// KDTrees can be fairly difficult to understand at first. The following references helped me
    /// understand what exactly a KDTree is doing and the contain the best descriptions of searches in a KDTree.
    /// Samet's book is the best reference of multidimensional data structures I have ever seen. Wikipedia is also a good starting place.
    /// References:
    /// <ul style="list-style-type:none">
    /// <li> <a href="http://store.elsevier.com/product.jsp?isbn=9780123694461">Foundations of Multidimensional and Metric Data Structures, 1st Edition, by Hanan Samet. ISBN: 9780123694461</a> </li>
    /// <li> <a href="https://en.wikipedia.org/wiki/K-d_tree"> https://en.wikipedia.org/wiki/K-d_tree</a> </li>
    /// </ul>
    /// </remarks>
    [Serializable]
    public class KDTree<TNode> : ISpatialQueryable<TNode>
    {
        /// <summary>
        /// The number of points in the KDTree
        /// </summary>
        public int Count => this.InternalList.Count;

        /// <summary>
        /// The numbers of dimensions that the tree has.
        /// </summary>
        public int Dimensions { get; }

        /// <summary>
        /// The metric function used to calculate distance between points.
        /// </summary>
        public Func<double[], double[], double> Metric { get; set; }

        private List<TNode> InternalList { get; }

        public KDTreeNavigator<TNode> Root => new KDTreeNavigator<TNode>(this, this.rootNode);

        private KDNode rootNode = new KDNode(-1);

        /// <summary>
        /// Initializes a new instance of the <see cref="KDTree{TNode}"/> class.
        /// </summary>
        /// <param name="dimensions">The number of dimensions in the data set.</param>
        /// <param name="nodes">The nodes associated with each point.</param>
        /// <param name="metric">The metric function which implicitly defines the metric space in which the KDTree operates in. This should satisfy the triangle inequality.</param>
        public KDTree(
            int dimensions,
            Func<double[], double[], double> metric,
            Func<TNode, double[]> selector = null)
        {
            this.PointSelector = selector ?? CoordinateSelector.CreateDefaultCoordinateSelector<TNode>();
            this.InternalList = new List<TNode>();
            this.Dimensions = dimensions;
            this.Metric = metric;
        }

        public KDTree(int dimensions,
            IEnumerable<TNode> nodes,
            Func<double[], double[], double> metric,
            Func<TNode, double[]> selector = null)
        {
            this.PointSelector = selector ?? CoordinateSelector.CreateDefaultCoordinateSelector<TNode>();
            this.InternalList = new List<TNode>(nodes);
            this.Dimensions = dimensions;
            this.Metric = metric;

            var points = this.InternalList.Select((p, i) => new CoordinateWithIndex(i, this.PointSelector(p))).ToArray();
            this.GenerateTree(ref this.rootNode, null, 0, points);
        }

        private Func<TNode, double[]> PointSelector { get; }

        /// <inheritdoc />
        public IEnumerable<int> Add(IEnumerable<TNode> items)
        {
            var originalMaxIndex = this.InternalList.Count - 1;
            this.InternalList.AddRange(items);
            var afterAddMaxIndex = this.InternalList.Count - 1;
            var coordinatesWithIndex = this.InternalList.Select((p, i) => new CoordinateWithIndex(i, this.PointSelector(p))).ToArray();
            this.GenerateTree(ref this.rootNode, null, 0, coordinatesWithIndex);
            return Enumerable.Range(originalMaxIndex + 1, afterAddMaxIndex - originalMaxIndex);
        }

        /// <inheritdoc />
        public int Add(TNode item)
        {
            this.InternalList.Add(item);
            var coordinatesWithIndex = this.InternalList.Select((p, i) => new CoordinateWithIndex(i, this.PointSelector(p))).ToArray();
            this.GenerateTree(ref this.rootNode, null, 0, coordinatesWithIndex);
            return this.InternalList.Count - 1;
        }

        /// <inheritdoc />
        public IEnumerable<TNode> NearestNeighbors(TNode target, int k) => this[this.NearestNeighborIndexes(target, k)];

        /// <inheritdoc />
        public IEnumerable<int> NearestNeighborIndexes(TNode target, int k)
        {
            var point = this.PointSelector(target);
            var nearestNeighborList = new BoundablePriorityList<int, double>(k, true);
            var rect = HyperRect<double>.Infinite(this.Dimensions, double.MaxValue, double.MinValue);
            this.SearchForNearestNeighbors(this.rootNode, point, rect, 0, nearestNeighborList, double.MaxValue);
            return nearestNeighborList;
        }

        /// <inheritdoc />
        public IEnumerable<TNode> RadialSearch(TNode center, double radius)
            => this.InternalList.WithIndexes(this.RadialSearchIndexes(center, radius));

        /// <inheritdoc />
        public IEnumerable<int> RadialSearchIndexes(TNode center, double radius)
        {
            var nearestNeighbors = new BoundablePriorityList<int, double>();
            var centerPoint = this.PointSelector(center);
            this.SearchForNearestNeighbors(
                this.rootNode,
                centerPoint,
                HyperRect<double>.Infinite(this.Dimensions, double.MaxValue, double.MinValue),
                0,
                nearestNeighbors,
                radius);

            return nearestNeighbors;
        }

        /// <inheritdoc />
        public TNode this[int index] => this.InternalList[index];

        /// <inheritdoc />
        public IEnumerable<TNode> this[IEnumerable<int> indexes] => this.InternalList.WithIndexes(indexes);

        /// <summary>
        /// Grows a KD tree recursively via median splitting. We find the median by doing a full sort.
        /// </summary>
        /// <param name="currentNode">The array index for the current node.</param>
        /// <param name="dim">The current splitting dimension.</param>
        /// <param name="points">The set of points remaining to be added to the KDTree.</param>
        /// <param name="nodes">The set of nodes RE</param>
        private void GenerateTree(
            ref KDNode currentNode,
            KDNode previousNode,
            int dim,
            IReadOnlyCollection<CoordinateWithIndex> points)
        {
            // See wikipedia for a good explanation KDTree construction.
            // https://en.wikipedia.org/wiki/K-d_tree

            // sort the points along the current dimension
            var sortedPoints = points.OrderBy(z => z.Coordinate[dim]).ToArray();

            // get the point which has the median value of the current dimension.
            var medianPoint = sortedPoints[points.Count / 2];
            var medianPointIdx = sortedPoints.Length / 2;

            // The point with the median value all the current dimension now becomes the value of the current tree node
            // The previous node becomes the parents of the current node.
            currentNode = new KDNode(medianPoint.Index, previousNode);
            previousNode = currentNode;

            // We now split the sorted points into 2 groups
            // 1st group: points before the median
            var leftPoints = new CoordinateWithIndex[medianPointIdx];
            Array.Copy(sortedPoints, leftPoints, leftPoints.Length);

            // 2nd group: Points after the median
            var rightPoints = new CoordinateWithIndex[sortedPoints.Length - (medianPointIdx + 1)];
            Array.Copy(
                sortedPoints,
                medianPointIdx + 1,
                rightPoints,
                0,
                rightPoints.Length);

            // We new recurse, passing the left and right arrays for arguments.
            // The current node's left and right values become the "roots" for
            // each recursion call. We also forward cycle to the next dimension.
            var nextDim = (dim + 1) % this.Dimensions; // select next dimension

            // We only need to recurse if the point array contains more than one point
            // If the array has no points then the node stay a null value
            if (leftPoints.Length <= 1)
            {
                if (leftPoints.Length == 1)
                {
                    currentNode.Left = new KDNode(leftPoints[0].Index, previousNode);
                }
            }
            else
            {
                this.GenerateTree(ref currentNode.Left, currentNode, nextDim, leftPoints);
            }

            // Do the same for the right points
            if (rightPoints.Length <= 1)
            {
                if (rightPoints.Length == 1)
                {
                    currentNode.Right = new KDNode(rightPoints[0].Index, previousNode);
                }
            }
            else
            {
                this.GenerateTree(ref currentNode.Right, currentNode, nextDim, rightPoints);
            }
        }

        /// <summary>
        /// A top-down recursive method to find the nearest neighbors of a given point.
        /// </summary>
        /// <param name="node">The index of the node for the current recursion branch.</param>
        /// <param name="target">The point whose neighbors we are trying to find.</param>
        /// <param name="rect">The <see cref="HyperRect{T}"/> containing the possible nearest neighbors.</param>
        /// <param name="dimension">The current splitting dimension for this recursion branch.</param>
        /// <param name="nearestNeighbors">The <see cref="BoundedPriorityList{TElement,TPriority}"/> containing the nearest neighbors already discovered.</param>
        /// <param name="maxSearchRadius">The radius of the current largest distance to search from the <paramref name="target"/></param>
        private void SearchForNearestNeighbors(
            KDNode node,
            double[] target,
            HyperRect<double> rect,
            int dimension,
            BoundablePriorityList<int, double> nearestNeighbors,
            double maxSearchRadius)
        {
            if (node == null)
            {
                return;
            }

            // Work out the current dimension
            var dim = dimension % this.Dimensions;

            // Get the coordinate of the current node.
            var coordinate = this.PointSelector(this.InternalList[node.ElementIndex]);

            // Split our hyper-rectangle into 2 sub rectangles along the current
            // node's point on the current dimension
            var leftRect = rect.Clone();
            leftRect.MaxPoint[dim] = coordinate[dim];

            var rightRect = rect.Clone();
            rightRect.MinPoint[dim] = coordinate[dim];

            // Determine which side the target resides in
            var comparison = target[dim] <= coordinate[dim];

            var nearerRect = comparison ? leftRect : rightRect;
            var furtherRect = comparison ? rightRect : leftRect;

            var nearerNode = comparison ? node.Left : node.Right;
            var furtherNode = comparison ? node.Right : node.Left;

            // Move down into the nearer branch
            this.SearchForNearestNeighbors(
                nearerNode,
                target,
                nearerRect,
                dimension + 1,
                nearestNeighbors,
                maxSearchRadius);

            // Walk down into the further branch but only if our capacity hasn't been reached
            // OR if there's a region in the further rectangle that's closer to the target than our
            // current furtherest nearest neighbor
            var closestPointInFurtherRect = furtherRect.GetClosestPoint(target);
            var distanceToTarget = this.Metric(closestPointInFurtherRect, target);

            if (distanceToTarget.CompareTo(maxSearchRadius) <= 0)
            {
                if (nearestNeighbors.IsFull)
                {
                    if (distanceToTarget.CompareTo(nearestNeighbors.MaxPriority) < 0)
                    {
                        this.SearchForNearestNeighbors(
                            furtherNode,
                            target,
                            furtherRect,
                            dimension + 1,
                            nearestNeighbors,
                            maxSearchRadius);
                    }
                }
                else
                {
                    this.SearchForNearestNeighbors(
                        furtherNode,
                        target,
                        furtherRect,
                        dimension + 1,
                        nearestNeighbors,
                        maxSearchRadius);
                }
            }

            // Try to add the current node to our nearest neighbors list
            distanceToTarget = this.Metric(coordinate, target);
            if (distanceToTarget.CompareTo(maxSearchRadius) <= 0)
            {
                nearestNeighbors.Add(node.ElementIndex, distanceToTarget);
            }
        }
    }

}
