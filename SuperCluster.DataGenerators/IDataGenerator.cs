namespace SuperCluster.DataGenerators
{
    interface IDataGenerator<T>
    {
        /// <summary>
        /// Generates <paramref name="n"/> data points of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="n">Number of points to be generated.</param>
        /// <returns></returns>
        T[] Generate<T>(int n);
    }
}
