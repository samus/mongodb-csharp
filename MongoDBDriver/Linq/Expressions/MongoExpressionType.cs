using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Linq.Expressions
{
    internal enum MongoExpressionType
    {
        Collection = 1000,
        Field,
        Select,
        Projection,
        Order
    }
}