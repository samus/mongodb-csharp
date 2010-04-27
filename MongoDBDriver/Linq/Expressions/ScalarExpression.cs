using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Linq.Expressions
{
    internal class ScalarExpression : SubqueryExpression
    {
        public ScalarExpression(Type type, FindExpression select)
            : base(MongoExpressionType.Scalar, type, select)
        { }
    }
}
