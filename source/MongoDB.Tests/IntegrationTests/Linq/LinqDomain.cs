using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.IntegrationTests.Linq
{
    public class Person
    {
        //[MongoAlias("fn")]
        public string FirstName { get; set; }

        //[MongoAlias("ln")]
        public string LastName { get; set; }

        //[MongoAlias("age")]
        public int Age { get; set; }

        //[MongoAlias("add")]
        public Address PrimaryAddress { get; set; }

        //[MongoAlias("otherAdds")]
        public List<Address> Addresses { get; set; }

        //[MongoAlias("emps")]
        public int[] EmployerIds { get; set; }
    }

    public class Address
    {
        //[MongoAlias("city")]
        public string City { get; set; }
    }
}
