using System.Collections.Generic;

namespace Supercluster.MTree.NewDesign
{
    using System.Diagnostics;
    using System.Runtime.CompilerServices;


    public class MNode<TValue>
    {

        /// <summary>
        /// The node entry that is the parent of this node. This property is null if this is the root node;
        /// </summary>
        public MNodeEntry<TValue> ParentEntry { get; set; } = null;

        /// <summary>
        /// The entries of the node.
        /// </summary>
        public List<MNodeEntry<TValue>> Entries
        {
            get
            {
                return this.entries;
            }

            set
            {
                this.entries = value;
                foreach (var entry in this.entries)
                {
                    entry.EnclosingNode = this;
                }
            }
        } // TODO: Should probably not have setter

        /// <summary>
        /// 
        /// 
        /// Note: We assume that the <see cref="MNodeEntry{TValue}"/> DistanceFromParent
        /// and Value properties have been set.
        /// </summary>
        /// <param name="newEntry"></param>
        public void Add(MNodeEntry<TValue> newEntry)
        {
            newEntry.EnclosingNode = this;
            this.Entries.Add(newEntry);
        }

        public void AddRange(IEnumerable<MNodeEntry<TValue>> newEntries)
        {
            foreach (var mNodeEntry in newEntries)
            {
                this.Add(mNodeEntry);
            }

        }

        /// <summary>
        /// Returns true if the node is full.
        /// </summary>
        public bool IsFull => this.Entries.Count == this.Capacity;

        /// <summary>
        /// The number of entries allowed in the node.
        /// </summary>
        /// // TODO: This REALLY needs to be write only, set in a constructor.
        public int Capacity { get; set; }

        private List<MNodeEntry<TValue>> entries;

        /// <summary>
        /// Returns true if the node is not a leaf node.
        /// </summary>
        public bool IsInternalNode => !this.Entries.TrueForAll(x => x.ChildNode == null);

        public MNode()
        {
            this.entries = new List<MNodeEntry<TValue>>();
        }


        public void SetEntryAtIndex(int index, MNodeEntry<TValue> entry)
        {
            this.entries[index] = entry;
            this.entries[index].EnclosingNode = this;
        }
    }
}
