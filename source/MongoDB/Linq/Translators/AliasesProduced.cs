using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Linq.Expressions;

namespace MongoDB.Linq.Translators
{
    /// <summary>
    ///  returns the set of all aliases produced by a query source
    /// </summary>
    internal class AliasesProduced : MongoExpressionVisitor
    {
        private HashSet<string> _aliases;

        public HashSet<string> Gather(Expression source)
        {
            _aliases = new HashSet<string>();
            Visit(source);
            return _aliases;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            _aliases.Add(select.Alias);
            return select;
        }

        protected override Expression VisitCollection(CollectionExpression collection)
        {
            _aliases.Add(collection.Alias);
            return collection;
        }
    }
}
