namespace Supercluster_Tests
{
    using NUnit.Framework;

    using Supercluster;

    [TestFixture]
    public class NearestNeighborsListTest
    {
        [Test]
        public void Add()
        {
            // Ascending
            var list = new SortedArray<double>(5, double.PositiveInfinity);
            list.Add(10);
            list.Add(5);
            list.Add(.2);
            list.Add(12);
            list.Add(.001);
            list.Add(500);
            list.Add(11);
            Assert.That(list[0], Is.EqualTo(.001));
            Assert.That(list[1], Is.EqualTo(.2));
            Assert.That(list[2], Is.EqualTo(5));
            Assert.That(list[3], Is.EqualTo(10));
            Assert.That(list[4], Is.EqualTo(11));

            // Descending
            var list2 = new SortedArray<double>(5, double.NegativeInfinity, false);
            list2.Add(10);
            list2.Add(5);
            list2.Add(.2);
            list2.Add(12);
            list2.Add(.001);
            list2.Add(500);
            list2.Add(11);
            Assert.That(list2[0], Is.EqualTo(500));
            Assert.That(list2[1], Is.EqualTo(12));
            Assert.That(list2[2], Is.EqualTo(11));
            Assert.That(list2[3], Is.EqualTo(10));
            Assert.That(list2[4], Is.EqualTo(5));
        }
    }
}
