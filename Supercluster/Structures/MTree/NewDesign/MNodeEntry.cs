
namespace Supercluster.MTree.NewDesign
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a node entry. All node entries should belong to a node.
    /// </summary>
    /// <typeparam name="TValue">The </typeparam>
    public class MNodeEntry<TValue>
    {
        private MNode<TValue> childNode = null;

        /// <summary>
        /// The covering radius of the routing object. If the node entry is a leaf-node entry, then the radius is -1.
        /// </summary>
        public double CoveringRadius { get; set; } = -1;

        /// <summary>
        /// The distance from the node entry's parent routing object. If the node entry is an entry in the root
        /// node then the distance from the nodes parent is -1.
        /// </summary>
        public double DistanceFromParent { get; set; } = -1;

        /// <summary>
        /// The child node of the node entry. This property is null if we are a leaf node entry.
        /// </summary>
        public MNode<TValue> ChildNode
        {
            get
            {
                return this.childNode;
            }

            set
            {
                this.childNode = value;
                this.ChildNode.ParentEntry = this;
            }
        }

        /// <summary>
        /// The parent node containing this node entry. Should never be null. All node entries should be contained in a node.
        /// </summary>
        public MNode<TValue> EnclosingNode { get; set; } = null;

        /// <summary>
        /// The value of the current node entry.
        /// </summary>
        public TValue Value { get; set; }
    }
}
