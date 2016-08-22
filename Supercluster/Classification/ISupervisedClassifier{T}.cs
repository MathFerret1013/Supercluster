namespace Supercluster.Classification
{
    /// <summary>
    /// Interface for supervised classifiers.
    /// </summary>
    /// <typeparam name="T">The type of data the algorithm performs classification on.</typeparam>
    public interface ISupervisedClassifier<T>
    {
        /// <summary>
        /// Performs classification of a single Point.
        /// </summary>
        /// <param name="dataPoint">Datum to be classified.</param>
        /// <returns>The output-class index.</returns>
        int Compute(T dataPoint);

        /// <summary>
        /// Trains the model using a given set of labeled (supervised) data.
        /// </summary>
        /// <param name="trainingData">The training data set.</param>
        /// <param name="trainingOutputs">The supervised classification outputs of the training data set.</param>
        void Train(T[] trainingData, int[] trainingOutputs);
    }
}
