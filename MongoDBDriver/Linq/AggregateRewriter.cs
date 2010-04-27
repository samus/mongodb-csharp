using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Linq.Expressions;
using System.Linq.Expressions;

namespace MongoDB.Driver.Linq
{
    internal class AggregateRewriter : MongoExpressionVisitor
    {
        ILookup<string, AggregateSubqueryExpression> _lookup;
        Dictionary<AggregateSubqueryExpression, Expression> _map;

        public AggregateRewriter()
        {
            _map = new Dictionary<AggregateSubqueryExpression, Expression>();
        }

        public Expression Rewrite(Expression expression)
        {
            _lookup = new AggregateGatherer().Gather(expression).ToLookup(x => x.GroupByAlias);
            return Visit(expression);
        }

        protected override Expression VisitAggregateSubquery(AggregateSubqueryExpression aggregate)
        {
            Expression mapped;
            if (_map.TryGetValue(aggregate, out mapped))
                return mapped;

            return Visit(aggregate.AggregateAsSubquery);
        }

        protected override Expression VisitFind(FindExpression find)
        {
            find = (FindExpression)base.VisitFind(find);
            if (_lookup.Contains(find.Alias))
            {
                var fields = new List<FieldDeclaration>(find.Fields);
                foreach (var ae in _lookup[find.Alias])
                {
                    var name = "_$agg" + fields.Count;
                    var field = new FieldDeclaration(name, ae.AggregateInGroupSelect);
                    _map.Add(ae, new FieldExpression(ae, ae.GroupByAlias, name));
                    fields.Add(field);
                }
                return new FindExpression(find.Type, find.Alias, fields, find.From, find.Where, find.OrderBy, find.GroupBy, find.Distinct, find.Skip, find.Limit);
            }
            return find;
        }

        private class AggregateGatherer : MongoExpressionVisitor
        {
            private List<AggregateSubqueryExpression> _aggregates;

            public AggregateGatherer()
            {
                _aggregates = new List<AggregateSubqueryExpression>();
            }

            public List<AggregateSubqueryExpression> Gather(Expression expression)
            {
                Visit(expression);
                return _aggregates;
            }

            protected override Expression VisitAggregateSubquery(AggregateSubqueryExpression aggregate)
            {
                _aggregates.Add(aggregate);
                return base.VisitAggregateSubquery(aggregate);
            }
        }
    }
}