using System;
using System.Linq;
using NUnit.Framework;

namespace MongoDB.IntegrationTests
{
    [TestFixture]
    public class TestCursor : MongoTestBase
    {
        public override string TestCollections
        {
            get { return "sorts,hintindex,smallreads,reads,largereads"; }
        }

        public override void OnInit()
        {
            //smallreads
            var smallreads = DB["smallreads"];
            for(var j = 1; j < 5; j++)
                smallreads.Insert(new Document {{"x", 4}, {"j", j}});
            smallreads.Insert(new Document {{"x", 4}, {"j", 5}, {"n", 1}});

            var reads = DB["reads"];
            for(var j = 1; j < 10000; j++)
                reads.Insert(new Document {{"x", 4}, {"h", "hi"}, {"j", j}});

            var properties = Enumerable.Range(1, 500).ToDictionary(x => x.ToString(), x => (object)x).ToArray();
            var largereads = DB["largereads"];
            largereads.Insert(Enumerable.Range(1, 3000).Select(i => new Document(properties)));
        }

        [Test]
        public void TestCanLimit()
        {
            var c = DB["reads"].FindAll().Limit(5);

            Assert.IsNotNull(c, "Cursor shouldn't be null");
            var reads = c.Documents.Count();
            Assert.IsTrue(reads > 0, "No documents were returned.");
            Assert.AreEqual(5, reads);
        }

        [Test]
        public void TestCanLimitWithLargeResultSet()
        {
            var count = DB["largereads"].FindAll().Limit(2000).Documents.Count();

            Assert.AreEqual(2000,count);
        }

        [Test]
        [ExpectedException(typeof(MongoException))]
        public void TestThrowsAnExceptionIfServerReturnsAQueryFailure()
        {
            DB["largereads"].FindAll().Sort("j").Documents.Any();
        }

        [Test]
        public void TestCanReadAndKillCursor()
        {
            var c = (Cursor)DB["reads"].FindAll();

            Assert.IsNotNull(c, "Cursor shouldn't be null");
            c.Documents.Any();
            c.Dispose();
            Assert.AreEqual(0, c.Id);
        }

        [Test]
        public void TestCanReadMore()
        {
            var c = (Cursor)DB["reads"].FindAll();

            Assert.IsNotNull(c, "Cursor shouldn't be null");
            var reads = 0;
            var idchanges = 0;
            long id = 0;
            foreach(var doc in c.Documents)
            {
                reads++;
                if(c.Id == id)
                    continue;
                idchanges++;
                id = c.Id;
            }
            Assert.IsTrue(reads > 0, "No documents were returned.");
            Assert.IsTrue(idchanges > 0, String.Format("ReadMore message never sent. {0} changes seen", idchanges));
            Assert.AreEqual(9999, reads, "Not all documents returned.");
            Console.Out.Write(String.Format("{0} records read", reads));
        }

        [Test]
        public void TestCanReuseCursor()
        {
            var c = (Cursor)DB["reads"].FindAll();

            Assert.IsNotNull(c, "Cursor shouldn't be null");

            var firstCount = c.Documents.Count();
            var secondCount = c.Documents.Count();

            Assert.AreEqual(firstCount,secondCount);
        }

        [Test]
        public void TestCanReadSmall()
        {
            var c = DB["smallreads"].FindAll();

            Assert.IsNotNull(c, "Cursor shouldn't be null");
            var reads = c.Documents.Count();
            Assert.IsTrue(reads > 0, "No documents were returned.");
            Assert.AreEqual(5, reads, "More than 5 documents in the small reads dataset");
        }

        [Test]
        public void TestExplainWithSort()
        {
            var exp = DB["reads"].FindAll().Limit(5).Skip(5).Sort("x").Explain();
            Assert.IsTrue(exp.ContainsKey("cursor"));
            Assert.AreEqual(9999, exp.Get("nscanned"));
            Assert.AreEqual(10,exp.Get("n"));
        }

        [Test]
        public void TestExplain()
        {
            var exp = DB["reads"].FindAll().Limit(5).Skip(5).Explain();
            Assert.IsTrue(exp.ContainsKey("cursor"));
            Assert.AreEqual(10, exp.Get("nscanned"));
            Assert.AreEqual(5, exp.Get("n"));
        }

        [Test]
        public void TestHint()
        {
            var reads = DB["reads"];
            var hint = new Document().Add("x", IndexOrder.Ascending);

            var exp = reads.FindAll().Hint(hint).Explain();
            Assert.IsTrue(exp.ContainsKey("$err"), "No error found");

            reads.Metadata.CreateIndex("hintindex", hint, false);
            exp = reads.FindAll().Hint(hint).Explain();

            Assert.IsTrue(exp.ContainsKey("cursor"));
            Assert.IsTrue(exp.ContainsKey("n"));
            Assert.IsTrue(exp.ContainsKey("nscanned"));
        }

        [Test]
        public void TestSort()
        {
            var sorts = DB["sorts"];
            var randoms = new[] {4, 6, 8, 9, 1, 3, 2, 5, 7, 0};
            foreach(var x in randoms)
                sorts.Insert(new Document().Add("x", randoms[x]));
            Assert.AreEqual(randoms.Length, sorts.Count());

            var exp = 0;
            foreach(var doc in sorts.FindAll().Sort("x", IndexOrder.Ascending).Documents)
            {
                Assert.AreEqual(exp, Convert.ToInt32(doc["x"]));
                exp++;
            }
            Assert.AreEqual(randoms.Length, exp);

            exp = 9;
            foreach(var doc in sorts.FindAll().Sort("x", IndexOrder.Descending).Documents)
            {
                Assert.AreEqual(exp, Convert.ToInt32(doc["x"]));
                exp--;
            }
            Assert.AreEqual(-1, exp);
        }
    }
}