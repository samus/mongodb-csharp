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
            _lookup = new AggregateGatherer().Gather(expression).ToLookup(x => x.Alias);
            return Visit(expression);
        }

        protected override Expression VisitAggregateSubquery(AggregateSubqueryExpression aggregate)
        {
            Expression mapped;
            if (_map.TryGetValue(aggregate, out mapped))
                return mapped;

            return Visit(aggregate.AggregateAsSubquery);
        }

        protected override Expression VisitFind(FindExpression f)
        {
            f = (FindExpression)base.VisitFind(f);
            if (_lookup.Contains(f.Alias))
            {
                var fields = new List<FieldExpression>(f.Fields);
                foreach (var ae in _lookup[f.Alias])
                {
                    var field = new FieldExpression("", ae.AggregateInGroupSelect);
                    _map.Add(ae, field);
                    fields.Add(field);
                }
                return new FindExpression(f.Type, f.Alias, fields, f.From, f.Where, f.OrderBy, f.GroupBy, f.Distinct, f.Skip, f.Limit);
            }
            return f;
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