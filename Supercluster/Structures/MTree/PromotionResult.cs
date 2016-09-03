namespace Supercluster.MTree.NewDesign
{
    using System.Collections.Generic;

    /// <summary>
    /// Result from the promotion function
    /// </summary>
    public class PromotionResult<T>
    {
        public MNodeEntry<T> FirstPromotionObject;
        public MNodeEntry<T> SecondPromotionObject;
        public List<MNodeEntry<T>> FirstPartition;
        public List<MNodeEntry<T>> SecondPartition;

    }
}
