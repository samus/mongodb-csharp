using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Configuration
{
    public class FluentlyTests
    {

        public void Test()
        {
            Fluently.Configure()
                .Mappings(m =>
                {

                })
                .Build();
        }

    }
}
