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
                    return VisitSelect((SelectExpression)exp);
                default:
                    return base.Visit(exp);
            }
        }

        protected virtual Expression VisitCollection(CollectionExpression c)
        {
            return c;
        }

        protected virtual Expression VisitField(FieldExpression f)
        {
            return f;
        }

        protected virtual Expression VisitProjection(ProjectionExpression p)
        {
            var source = (SelectExpression)Visit(p.Source);
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

        protected virtual Expression VisitSelect(SelectExpression s)
        {
            var from = VisitSource(s.From);
            var where = Visit(s.Where);
            var order = VisitOrderBy(s.Order);
            var skip = Visit(s.Skip);
            var limit = Visit(s.Limit);
            if (from != s.From || where != s.Where || order != s.Order || skip != s.Skip || limit != s.Limit)
                return new SelectExpression(s.Type, s.Fields, from, where, order, s.Distinct, skip, limit);
            return s;
        }

        protected virtual Expression VisitSource(Expression source)
        {
            return Visit(source);
        }
    }
}