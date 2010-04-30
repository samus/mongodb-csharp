using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MongoDB.Linq.Expressions
{
    internal abstract class MongoExpression : Expression
    {
        protected MongoExpression(MongoExpressionType nodeType, Type type)
            : base((ExpressionType)nodeType, type)
        { }
    }
}