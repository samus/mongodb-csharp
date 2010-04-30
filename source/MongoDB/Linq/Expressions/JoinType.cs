using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Linq.Expressions
{
    internal enum JoinType
    {
        CrossJoin,
        InnerJoin,
        CrossApply,
        OuterApply,
        LeftOuter
    }
}
