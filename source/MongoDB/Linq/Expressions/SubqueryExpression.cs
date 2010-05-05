using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MongoDB.Linq.Expressions
{
    internal abstract class SubqueryExpression : MongoExpression
    {
        private SelectExpression _select;

        public SelectExpression Select
        {
            get { return _select; }
        }

        protected SubqueryExpression(MongoExpressionType nodeType, Type type, SelectExpression select)
            : base(nodeType, type)
        {
            _select = select;
        }
    }
}
