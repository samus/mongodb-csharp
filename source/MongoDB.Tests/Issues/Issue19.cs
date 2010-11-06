using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using MongoDB.Attributes;
using MongoDB.Configuration;

namespace MongoDB.Issues
{
    [TestFixture]
    public class Issue19
    {
        private class Post
        {
            [MongoId]
            public Guid Id { get; set; }

            public DateTime LastAccess { get; set; }
            public List<string> Tags { get; set; }
        }

        [Test]
        public void Should_return_an_empty_list()
        {
            var SearchTags = new List<string> { "test" };
            using (var mongo = new Mongo())
            {
                mongo.Connect();
                var db = mongo.GetDatabase("posts");
                var posts = db.GetCollection<Post>();
                posts.Remove(x => true);
                posts.Insert(new Post() { Tags = new List<string> { "test", "funny" } });

                var result = SearchTags
                    .Aggregate(posts.Linq(), (current, tag) => current.Where(x => x.Tags.Contains(tag)))
                    .OrderByDescending(x => x.LastAccess)
                    .ToList();

                Assert.AreEqual(1, result.Count);
            }
        }

    }
}