namespace Supercluster_Tests
{
    using System.Linq;

    using NUnit.Framework;

    using Supercluster;

    /// <summary>
    /// Unit tests for the <see cref="CollectionUtilities"/> class.
    /// </summary>
    [TestFixture]
    public class CollectionUtilitiesTests
    {

        [Test]
        public void MaxIndexTest()
        {
            var intArray = new int[] { 3, 5, 8, int.MaxValue, 6, -78, 64 };
            var doubleArray = new double[] { 3.0, 5.0, double.MaxValue, 6.0, -78.0, 64.0 };

            var maxIndex = intArray.MaxIndex();
            var maxIndex2 = doubleArray.MaxIndex();

            Assert.That(maxIndex, Is.EqualTo(3));
            Assert.That(maxIndex2, Is.EqualTo(2));
        }


        [Test]
        public void MinIndexTest()
        {
            var intArray = new int[] { 3, 5, 8, int.MaxValue, 6, -78, 64 };
            var doubleArray = new double[] { 3.0, 5.0, double.MaxValue, 6.0, -78.0, 64.0 };

            var minIndex = intArray.MinIndex();
            var minIndex2 = doubleArray.MinIndex();

            Assert.That(minIndex, Is.EqualTo(5));
            Assert.That(minIndex2, Is.EqualTo(4));
        }

        [Test]
        public void ModeIndexTest()
        {
            var intArray = new int[] { 1, 2, 2, 5, 5, 5, 5, 10, 10 };

            var mode = intArray.Mode();

            Assert.That(mode, Is.EqualTo(5));
        }

        [Test]
        public void BijectWithNaturalsTest()
        {

            // -99 0
            // -1 1
            // 2 2
            // 3 3
            // 4 4
            // 90 5
            // 356 6


            var testArray = new int[] { -99, 4, 356, 3, 2, 2, 2, 2, -1, 90 };
            testArray = testArray.BijectWithNaturals();

            Assert.That(testArray.SequenceEqual(new int[] { 0, 4, 6, 3, 2, 2, 2, 2, 1, 5 }));
        }

        [Test]
        public void GetRowTest()
        {
            var matrix = new double[,]
                             {
                                 { 1, 1, 1 },
                                 { 1, -1, 1 },
                                 { 0, 1, 2 }
                             };

            var row = matrix.GetRow(1);


        }
    }
}
