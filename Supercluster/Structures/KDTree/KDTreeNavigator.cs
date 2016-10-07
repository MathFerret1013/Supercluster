using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supercluster.Structures.KDTree
{
    using System.Runtime.CompilerServices;

    public class KDTreeNavigator<TNode>
    {
        private readonly KDTree<TNode> tree;

        private readonly KDNode node;

        public TNode Value => this.tree[this.node.ElementIndex];

        public KDTreeNavigator<TNode> Right => new KDTreeNavigator<TNode>(this.tree, this.node.Right);

        public KDTreeNavigator<TNode> Left => new KDTreeNavigator<TNode>(this.tree, this.node.Left);

        public KDTreeNavigator<TNode> Parrent => new KDTreeNavigator<TNode>(this.tree, this.node.Parent);

        public KDTreeNavigator(KDTree<TNode> tree, KDNode node)
        {
            this.tree = tree;
            this.node = node;
        }
    }
}
