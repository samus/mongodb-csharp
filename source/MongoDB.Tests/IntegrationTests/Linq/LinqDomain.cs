using System;
using System.Collections.Generic;

using MongoDB.Attributes;

namespace MongoDB.IntegrationTests.Linq
{
    public class Person
    {
        [MongoAlias("fn")]
        public string FirstName { get; set; }

        [MongoAlias("ln")]
        public string LastName { get; set; }

        [MongoAlias("age")]
        public int Age { get; set; }

        [MongoAlias("add")]
        public Address PrimaryAddress { get; set; }

        [MongoAlias("otherAdds")]
        public List<Address> Addresses { get; set; }

        [MongoAlias("emps")]
        public int[] EmployerIds { get; set; }

        public string MidName { get; set; }

        public Oid LinkedId { get; set; }
    }

    public class Address
    {
        [MongoAlias("city")]
        public string City { get; set; }

		public bool IsInternational { get; set; }

        public AddressType AddressType { get; set; }
    }

    public enum AddressType
    {
        Company,
        Private
    }

    public class PersonWrapper
    {
        public Person Person { get; set; }
        public string Name { get; set; }

        public PersonWrapper(Person person, string name)
        {
            Person = person;
            Name = name;
        }
    }
}
