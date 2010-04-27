using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDB.Driver.Linq.Expressions
{
    internal class MongoExpressionVisitor : ExpressionVisitor
    {
        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
                return null;
            switch ((MongoExpressionType)exp.NodeType)
            {
                case MongoExpressionType.Collection:
                    return VisitCollection((CollectionExpression)exp);
                case MongoExpressionType.Field:
                    return VisitField((FieldExpression)exp);
                case MongoExpressionType.Projection:
                    return VisitProjection((ProjectionExpression)exp);
                case MongoExpressionType.Select:
                    return VisitFind((FindExpression)exp);
                case MongoExpressionType.Aggregate:
                    return VisitAggregate((AggregateExpression)exp);
                case MongoExpressionType.AggregateSubquery:
                    return VisitAggregateSubquery((AggregateSubqueryExpression)exp);
                case MongoExpressionType.Scalar:
                    return VisitScalar((ScalarExpression)exp);
                default:
                    return base.Visit(exp);
            }
        }

        protected virtual Expression VisitAggregate(AggregateExpression a)
        {
            var exp = Visit(a.Argument);
            if (exp != a.Argument)
                return new AggregateExpression(a.Type, a.AggregateType, exp, a.Distinct);

            return a;
        }

        protected virtual Expression VisitAggregateSubquery(AggregateSubqueryExpression aggregate)
        {
            Expression e = Visit(aggregate.AggregateAsSubquery);
            ScalarExpression subquery = (ScalarExpression)e;
            if (subquery != aggregate.AggregateAsSubquery)
                return new AggregateSubqueryExpression(aggregate.AggregateInGroupSelect, subquery);
            return aggregate;
        }

        protected virtual Expression VisitCollection(CollectionExpression c)
        {
            return c;
        }

        protected virtual Expression VisitField(FieldExpression f)
        {
            var exp = Visit(f.Expression);
            if (exp != f.Expression)
                f = new FieldExpression(f.Name, exp);

            return f;
        }

        protected virtual Expression VisitFind(FindExpression f)
        {
            var from = VisitSource(f.From);
            var where = Visit(f.Where);
            var groupBy = VisitExpressionList(f.GroupBy);
            var orderBy = VisitOrderBy(f.OrderBy);
            var skip = Visit(f.Skip);
            var limit = Visit(f.Limit);
            if (from != f.From || where != f.Where || orderBy != f.OrderBy || groupBy != f.GroupBy || skip != f.Skip || limit != f.Limit)
                return new FindExpression(f.Type, f.Fields, from, where, orderBy, groupBy, f.Distinct, skip, limit);
            return f;
        }

        protected virtual Expression VisitProjection(ProjectionExpression p)
        {
            var source = (FindExpression)Visit(p.Source);
            var projector = Visit(p.Projector);
            if (source != p.Source || projector != p.Projector)
                return new ProjectionExpression(source, projector, p.Aggregator);
            return p;
        }

        protected ReadOnlyCollection<OrderExpression> VisitOrderBy(ReadOnlyCollection<OrderExpression> expressions)
        {
            if (expressions != null) 
            {
                List<OrderExpression> alternate = null;
                for (int i = 0, n = expressions.Count; i < n; i++) 
                {
                    OrderExpression expr = expressions[i];
                    Expression e = this.Visit(expr.Expression);
                    if (alternate == null && e != expr.Expression) 
                        alternate = expressions.Take(i).ToList();
                    if (alternate != null) 
                        alternate.Add(new OrderExpression(expr.OrderType, e));
                }
                if (alternate != null) 
                    return alternate.AsReadOnly();
            }
            return expressions;
        }

        protected virtual Expression VisitScalar(ScalarExpression scalar)
        {
            FindExpression find = (FindExpression)Visit(scalar.Find);
            if (find != scalar.Find)
                return new ScalarExpression(scalar.Type, find);
            return scalar;
        }

        protected virtual Expression VisitSource(Expression source)
        {
            return Visit(source);
        }

        protected virtual Expression VisitSubquery(SubqueryExpression subquery)
        {
            switch ((MongoExpressionType)subquery.NodeType)
            {
                case MongoExpressionType.Scalar:
                    return VisitScalar((ScalarExpression)subquery);
            }
            return subquery;
        }
    }
}