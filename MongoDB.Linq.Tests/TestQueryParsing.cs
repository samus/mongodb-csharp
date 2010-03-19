using System;
using System.Diagnostics;
using System.Linq;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;

namespace MongoDB.Linq.Tests {
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestQueryParsing {

        private IMongoQuery queryable;
        private Mock<IMongoCollection<Document>> collectionMock;
        private Mock<ICursor<Document>> cursorMock;

        [SetUp]
        public void Setup() {
            Debug.WriteLine("initializing queryable");
            collectionMock = new Mock<IMongoCollection<Document>>();
            cursorMock = new Mock<ICursor<Document>>();
            collectionMock.Setup(c => c.Find(It.IsAny<Document>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Document>())).Returns(cursorMock.Object);
            queryable = new MongoQuery(new MongoQueryProvider(collectionMock.Object));
        }

        [Test]
        public void No_where_produces_empty_Query() {
            var q = (IMongoQuery)(from d in queryable select d);
            Assert.IsNull(q.Query);
        }

        [Test]
        public void Can_call_ToList_on_query() {
            (from d in queryable select d).ToList();
            collectionMock.Verify(c => c.Find(null, 0, 0, null));
        }

        [Test]
        public void Can_call_AsEnumerable_on_query() {
            var q = (from d in queryable select d).AsEnumerable();
            var enumerator = q.GetEnumerator();
            var first = enumerator.Current;
            collectionMock.Verify(c => c.Find(null, 0, 0, null));
        }

        [Test]
        public void No_skip_produces_zero_skip() {
            var q = (IMongoQuery)(from d in queryable select d);
            Assert.AreEqual(0, q.Skip);
        }

        [Test]
        public void Skip_5_produces_skip_5() {
            var q = (IMongoQuery)(from d in queryable select d).Skip(5);
            Assert.AreEqual(5, q.Skip);
        }

        [Test]
        public void No_take_produces_zero_limit() {
            var q = (IMongoQuery)(from d in queryable select d);
            Assert.AreEqual(0, q.Limit);
        }

        [Test]
        public void Take_5_produces_limit_5() {
            var q = (IMongoQuery)(from d in queryable select d).Take(5);
            Assert.AreEqual(5, q.Limit);
        }

        [Test]
        public void Can_chain_Take_and_Skip() {
            var q = (IMongoQuery)(from d in queryable select d).Take(5).Skip(10);
            Assert.AreEqual(5, q.Limit);
            Assert.AreEqual(10, q.Skip);
        }

        [Test]
        public void FirstOrDefault_produces_limit_1() {
            (from d in queryable select d).FirstOrDefault();
            collectionMock.Verify(c => c.Find(null, 1, 0, null));
        }

        [Test]
        public void First_on_empty_sequence_throws() {
            try {
                (from d in queryable select d).First();
                Assert.Fail("First didn't throw");
            } catch (InvalidOperationException e) {
                Assert.AreEqual("Sequence contains no elements", e.Message);
            }
            collectionMock.Verify(c => c.Find(null, 1, 0, null));
        }

        [Test]
        public void Can_use_equality_where_clause_with_equals() {
            var q = (IMongoQuery)(from d in queryable where Equals(d["foo"], "bar") select d);
            Assert.AreEqual(new Document().Add("foo","bar").ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_equality_where_clause_with_equals_method_on_Document_indexer() {
            var q = (IMongoQuery)(from d in queryable where d["foo"].Equals("bar") select d);
            Assert.AreEqual(new Document().Add("foo", "bar").ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_equality_where_clause_with_equals_method_on_value() {
            var q = (IMongoQuery)(from d in queryable where "bar".Equals(d["foo"]) select d);
            Assert.AreEqual(new Document().Add("foo", "bar").ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_equality_where_clause_with_left_cast_to_string() {
            var q = (IMongoQuery)(from d in queryable where (string)d["foo"] == "bar" select d);
            Assert.AreEqual(new Document().Add("foo", "bar").ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_equality_where_clause_with_right_cast_to_object() {
            var q = (IMongoQuery)(from d in queryable where (string)d["foo"] == "bar" select d);
            Assert.AreEqual(new Document().Add("foo", "bar").ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_equality_where_clause_with_left_and_right_reversed() {
            var q = (IMongoQuery)(from d in queryable where "bar" == (string)d["foo"] select d);
            Assert.AreEqual(new Document().Add("foo", "bar").ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_equality_where_clause_with_key_as_variable() {
            string key = "foo";
            var q = (IMongoQuery)(from d in queryable where (string)d[key] == "bar" select d);
            Assert.AreEqual(new Document().Add("foo", "bar").ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_equality_where_clause_with_value_as_variable() {
            string value = "bar";
            var q = (IMongoQuery)(from d in queryable where (string)d["foo"] == value select d);
            Assert.AreEqual(new Document().Add("foo","bar").ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_inequality() {
            var q = (IMongoQuery)(from d in queryable where (string)d["foo"] != "bar" select d);
            Assert.AreEqual(new Document().Add("foo",new Document().Add("$ne","bar")).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_inequality_reversed() {
            var q = (IMongoQuery)(from d in queryable where "bar" != (string)d["foo"] select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$ne", "bar")).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than() {
            var q = (IMongoQuery)(from d in queryable where (int)d["foo"] > 10 select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$gt", 10)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than_reversed() {
            var q = (IMongoQuery)(from d in queryable where 10 > (int)d["foo"] select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lt", 10)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than_or_equal() {
            var q = (IMongoQuery)(from d in queryable where (int)d["foo"] >= 10 select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$gte", 10)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_less_than() {
            var q = (IMongoQuery)(from d in queryable where (int)d["foo"] < 10 select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lt", 10)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_less_than_or_equal() {
            var q = (IMongoQuery)(from d in queryable where (int)d["foo"] <= 10 select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lte", 10)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_do_and_queries() {
            var q = (IMongoQuery)(from d in queryable where (int)d["foo"] <= 10 && (string)d["bar"] == "zoop" select d);
            Assert.AreEqual(
                new Document()
                    .Add("foo", new Document().Add("$lte", 10))
                    .Add("bar","zoop"),
                q.Query);
        }
		
		[Test]
		public void Can_do_and_queries_on_same_key(){
            var q = (IMongoQuery)(from d in queryable where (int)d["foo"] < 10 && (int)d["foo"] > 5 select d);
            Assert.AreEqual(
                new Document().Add("foo", new Document().Add("$lt", 10).Add("$gt",5)),
                q.Query);			
		}
		
        [Test]
        public void Can_compose_queries() {
            // Note (sdether): this passes without explicit AND support, which is a bit scary
            var q1 = from d in queryable where (int)d["foo"] <= 10 select d;
            var q2 = (IMongoQuery)(from d in q1 where (string)d["bar"] == "zoop" select d);
            Assert.AreEqual(
                new Document()
                    .Add("foo", new Document().Add("$lte", 10))
                    .Add("bar", "zoop"),
                q2.Query);
        }

        [Test]
        public void Can_use_dot_notation_for_queries() {
            // Note (sdether): dot.notation in document is a bit of a perversion, since it's not legal
            // in a document to be saved. So this syntax may break, if Document becomes more strict
            var q = (IMongoQuery)(from d in queryable where (int)d["foo.bar"] == 10 select d);
            Assert.AreEqual(new Document().Add("foo.bar",10).ToString(), q.Query.ToString());
        }
    }
    // ReSharper restore InconsistentNaming
}
