using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MongoDB.Driver.Linq.Expressions
{
    internal abstract class SubqueryExpression : Expression
    {
        private FindExpression _find;

        public FindExpression Find
        {
            get { return _find; }
        }

        protected SubqueryExpression(MongoExpressionType nodeType, Type type, FindExpression find)
            : base((ExpressionType)nodeType, type)
        {
            _find = find;
        }
    }
}
