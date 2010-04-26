using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MongoDB.Driver.Linq.Expressions
{
    internal abstract class SubqueryExpression : Expression
    {
        private SelectExpression _select;

        public SelectExpression Select
        {
            get { return _select; }
        }

        protected SubqueryExpression(MongoExpressionType nodeType, Type type, SelectExpression select)
            : base((ExpressionType)nodeType, type)
        {
            _select = select;
        }
    }
}
