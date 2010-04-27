using System;
using MongoDB.Configuration;
using NUnit.Framework;

namespace MongoDB.UnitTests.Configuration
{
    [TestFixture]
    public class MongoConfigurationTests
    {
        private class Person
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        [Test]
        public void Test()
        {
            var configure = new MongoConfigurationBuilder();

            configure.ConnectionString(cs =>
            {
                cs.Pooled = true;
            });

            configure.Mapping(mapping => 
            {
                mapping.DefaultProfile(p =>
                {
                    p.AliasesAreCamelCased();
                    p.CollectionNamesAreCamelCasedAndPlural();
                });

                mapping.Map<Person>(m =>
                {
                    m.CollectionName("people");
                    m.Member("Name").Alias("name").DefaultValue("something").Ignore();
                });
            });
        }

    }
}
