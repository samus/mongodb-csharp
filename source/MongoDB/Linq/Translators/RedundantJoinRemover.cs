using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Linq.Expressions;
using MongoDB.Util;

namespace MongoDB.Linq.Translators
{
    internal class RedundantJoinRemover : MongoExpressionVisitor
    {
        private Dictionary<Alias, Alias> _map;

        public Expression Remove(Expression expression)
        {
            _map = new Dictionary<Alias, Alias>();
            return Visit(expression);
        }

        protected override Expression VisitField(FieldExpression field)
        {
            Alias mapped;
            if (_map.TryGetValue(field.Alias, out mapped))
                return new FieldExpression(field.Expression, mapped, field.Name);
            return field;
        }

        protected override Expression VisitJoin(JoinExpression join)
        {
            var result = base.VisitJoin(join);
            join = result as JoinExpression;
            if (join != null)
            {
                var right = join.Right as AliasedExpression;
                if (right != null)
                {
                    var similarRight = (AliasedExpression)FindSimiliarRight(join.Left as JoinExpression, join);
                    if (similarRight != null)
                    {
                        _map.Add(right.Alias, similarRight.Alias);
                        return join.Left;
                    }
                }
            }
            return result;
        }

        private Expression FindSimiliarRight(JoinExpression join, JoinExpression compareTo)
        {
            if (join == null)
                return null;
            if (join.Join == compareTo.Join)
            {
                if (join.Right.NodeType == compareTo.Right.NodeType && MongoExpressionComparer.AreEqual(join.Right, compareTo.Right))
                {
                    if (join.Condition == compareTo.Condition)
                        return join.Right;
                    var scope = new ScopedDictionary<Alias, Alias>(null);
                    scope.Add(((AliasedExpression)join.Right).Alias, ((AliasedExpression)compareTo.Right).Alias);
                    if (MongoExpressionComparer.AreEqual(null, scope, join.Condition, compareTo.Condition))
                        return join.Right;
                }
            }
            var result = FindSimiliarRight(join.Left as JoinExpression, compareTo);
            if (result == null)
                result = FindSimiliarRight(join.Right as JoinExpression, compareTo);
            return result;
        }
    }
}