using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Linq.Expressions
{
    internal enum MongoExpressionType
    {
        Collection = 1000,
        ClientJoin,
        Field,
        Select,
        Projection,
        Join,
        Order,
        Aggregate,
        AggregateSubquery,
        Scalar,
        OuterJoined,
        NamedValue
    }
}