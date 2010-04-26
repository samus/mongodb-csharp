using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MongoDB.Driver.Configuration
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
            var configure = new MongoConfiguration();

            configure.DefaultProfile(p =>
            {
                p.AliasesAreCamelCased();
                p.CollectionNamesAreCamelCasedAndPlural();
            });

            configure.Map<Person>(m =>
            {
                m.CollectionName("people");
                m.Member("Name").Alias("name").DefaultValue("something").Ignore();
            });

            configure.BuildSerializationFactory();
        }

    }
}
