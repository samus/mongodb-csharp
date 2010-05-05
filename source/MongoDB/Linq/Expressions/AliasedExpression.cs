using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Linq.Expressions
{
    internal abstract class AliasedExpression : MongoExpression
    {
        private Alias _alias;

        public Alias Alias
        {
            get { return _alias; }
        }

        protected AliasedExpression(MongoExpressionType nodeType, Type type, Alias alias)
            : base(nodeType, type)
        {
            _alias = alias;
        }
    }
}
