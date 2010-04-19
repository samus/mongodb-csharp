using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MongoDB.Driver.Configuration
{
    [TestFixture]
    public class FluentlyTests
    {
        private class Person
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        [Test]
        public void Test()
        {
            Fluently.Configure()
                .Mappings(m =>
                {
                    m.For<Person>(c =>
                    {
                        c.CollectionNameIs("people");
                        c.Member("Name").Alias("name").DefaultValueIs("something").PersistNull();
                    });

                })
                .BuildSerializationFactory();
        }

    }
}
