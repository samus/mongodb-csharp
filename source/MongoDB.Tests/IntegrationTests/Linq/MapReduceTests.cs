using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MongoDB.Linq;

using NUnit.Framework;

namespace MongoDB.IntegrationTests.Linq
{
    [TestFixture]
    public class MapReduceTests : LinqTestsBase
    {
        [Test]
        public void Complex()
        {
            var people = from p in collection.Linq()
                         group p by 1 into g
                         select new
                         {
                             Min = g.Min(x => x.Age),
                             Max = g.Max(x => x.Age)
                         };
            
            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.IsTrue(queryObject.IsMapReduce);
        }
    }
}