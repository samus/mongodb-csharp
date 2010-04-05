using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Linq
{
    public class MongoQueryObject
    {
        public Document Fields { get; set; }

        public int NumberToSkip { get; set; }

        public int NumberToLimit { get; set; }

        public Document Order { get; set; }

        public Document Query { get; set; }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}