using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Linq.Expressions;

namespace MongoDB.Linq.Translators
{
    internal class QueryDuplicator : MongoExpressionVisitor
    {
        Dictionary<string, string> _map;

        public Expression Duplicate(Expression expression)
        {
            _map = new Dictionary<string, string>();
            return Visit(expression);
        }

        protected override Expression VisitCollection(CollectionExpression collection)
        {
            var newAlias = collection.Alias + "cpy";
            _map[collection.Alias] = newAlias;
            return new CollectionExpression(collection.Type, newAlias, collection.Database, collection.CollectionName, collection.DocumentType);
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            var newAlias = select.Alias + "cpy";
            _map[select.Alias] = newAlias;
            select = (SelectExpression)base.VisitSelect(select);
            return new SelectExpression(select.Type, newAlias, select.Fields, select.From, select.Where, select.OrderBy, select.GroupBy, select.Distinct, select.Skip, select.Take);
        }

        protected override Expression VisitField(FieldExpression field)
        {
            string newAlias;
            if (_map.TryGetValue(field.Alias, out newAlias))
                return new FieldExpression(field.Expression, newAlias, field.Name);

            return field;
        }
    }
}
