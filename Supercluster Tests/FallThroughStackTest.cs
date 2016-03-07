namespace Supercluster_Tests
{
    using NUnit.Framework;

    using Supercluster;

    /// <summary>
    /// Tests for the FallThroughStack
    /// </summary>
    [TestFixture]
    public class FallThroughStackTest
    {
        [Test]
        public void PushTest()
        {
            var data = new int[] { 4, 8, 7, 2 };
            var fstack = new FallThroughStack<int>(3);
            foreach (var i in data)
            {
                fstack.Push(i);
            }

            Assert.That(fstack[0], Is.EqualTo(2));
            Assert.That(fstack[1], Is.EqualTo(7));
            Assert.That(fstack[2], Is.EqualTo(8));
        }


        [Test]
        public void PopTest()
        {
            var data = new int[] { 4, 8, 7, 2 };
            var fstack = new FallThroughStack<int>(3);
            foreach (var i in data)
            {
                fstack.Push(i);
            }

            Assert.That(fstack.Pop(), Is.EqualTo(2));
            Assert.That(fstack.Pop(), Is.EqualTo(7));
            Assert.That(fstack.Pop(), Is.EqualTo(8));
        }
    }
}
