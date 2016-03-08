namespace Workbench
{
    using Supercluster.KDTree;

    class Program
    {
        static void Main(string[] args)
        {
            var points = new double[][]
                             {
                                 new double[] { 2, 3 },
                                 new double[] { 5, 4 },
                                 new double[] { 9, 6 },
                                 new double[] { 4, 7 },
                                 new double[] { 8, 1 },
                                 new double[] { 7, 2 },
                                 new double[] { 2, 2 }, 
                             };

            var tree = new KDTree(2, points);
            var best = tree.FindNearestNeighbor(new double[] { 7, 10 });
        }
    }
}
