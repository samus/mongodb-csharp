using NUnit.Framework;

namespace MongoDB.Driver.UnitTests
{
    [TestFixture]
    public class TestMo
    {
        [Test]
        public void TestInc(){
            var mo = Mo.Inc("A", 10);

            var expected = new Document("$inc", new Document("A", 10));

            Assert.AreEqual(expected, mo);
        }

        [Test]
        public void TestSet()
        {
            var mo = Mo.Set("A", 10);

            var expected = new Document("$set", new Document("A", 10));

            Assert.AreEqual(expected, mo);
        }

        [Test]
        public void TestUnset()
        {
            var mo = Mo.Unset("A");

            var expected = new Document("$unset", new Document("A", 1));

            Assert.AreEqual(expected, mo);
        }

        [Test]
        public void TestUnsetArray(){
            var array = new[] {"A", "B"};
            var mo = Mo.Unset(array);

            var expected = new Document("$unset", new Document("A",1).Add("B",1));

            Assert.AreEqual(expected, mo);
        }

        [Test]
        public void TestPush()
        {
            var mo = Mo.Push("A",1);

            var expected = new Document("$push", new Document("A", 1));

            Assert.AreEqual(expected, mo);
        }

        [Test]
        public void TestPushAll()
        {
            var array = new object[] {1, "C"};
            var mo = Mo.PushAll("A", array);

            var expected = new Document("$pushAll", new Document("A", array));

            Assert.AreEqual(expected, mo);
        }

        [Test]
        public void TestAddToSet()
        {
            var mo = Mo.AddToSet("A", 1);

            var expected = new Document("$addToSet", new Document("A", 1));

            Assert.AreEqual(expected, mo);
        }

        [Test]
        public void TestAddToSetArray()
        {
            var array = new object[] { 1, "C" };
            var mo = Mo.AddToSet("A", array);

            var expected = new Document("$addToSet", new Document("A", new Document("$each", array)));

            Assert.AreEqual(expected, mo);
        }

        [Test]
        public void TestPopFirst()
        {
            var mo = Mo.PopFirst("A");

            var expected = new Document("$pop", new Document("A", -1));

            Assert.AreEqual(expected, mo);
        }

        [Test]
        public void TestPopLast()
        {
            var mo = Mo.PopLast("A");

            var expected = new Document("$pop", new Document("A", 1));

            Assert.AreEqual(expected, mo);
        }

        [Test]
        public void TestPull()
        {
            var mo = Mo.Pull("A",1);

            var expected = new Document("$pull", new Document("A", 1));

            Assert.AreEqual(expected, mo);
        }

        [Test]
        public void TestPullAll()
        {
            var array = new object[] { 1, "C" };
            var mo = Mo.PullAll("A", array);

            var expected = new Document("$pullAll", new Document("A", array));

            Assert.AreEqual(expected, mo);
        }

        [Test]
        public void TestMultipleModifiers()
        {
            var mo = Mo.Inc("A", 1) & Mo.Inc("B", 2) & Mo.Set("C",3);

            var expected = new Document("$inc", new Document("A", 1).Add("B", 2))
                .Add("$set",new Document("C",3));

            Assert.AreEqual(expected, mo);
        }
    }
}