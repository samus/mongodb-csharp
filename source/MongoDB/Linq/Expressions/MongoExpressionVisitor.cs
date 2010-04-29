using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDB.Linq.Expressions
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
                case MongoExpressionType.Find:
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

        protected virtual Expression VisitAggregate(AggregateExpression aggregate)
        {
            var exp = Visit(aggregate.Argument);
            if (exp != aggregate.Argument)
                return new AggregateExpression(aggregate.Type, aggregate.AggregateType, exp, aggregate.Distinct);

            return aggregate;
        }

        protected virtual Expression VisitAggregateSubquery(AggregateSubqueryExpression aggregateSubquery)
        {
            Expression e = Visit(aggregateSubquery.AggregateAsSubquery);
            ScalarExpression subquery = (ScalarExpression)e;
            if (subquery != aggregateSubquery.AggregateAsSubquery)
                return new AggregateSubqueryExpression(aggregateSubquery.GroupByAlias, aggregateSubquery.AggregateInGroupSelect, subquery);
            return aggregateSubquery;
        }

        protected virtual Expression VisitCollection(CollectionExpression collection)
        {
            return collection;
        }

        protected virtual Expression VisitField(FieldExpression field)
        {
            var e = Visit(field.Expression);
            if (field.Expression != e)
                field = new FieldExpression(e, field.Alias, field.Name);

            return field;
        }

        protected virtual Expression VisitFind(FindExpression find)
        {
            var from = VisitSource(find.From);
            var where = Visit(find.Where);
            var groupBy = Visit(find.GroupBy);
            var orderBy = VisitOrderBy(find.OrderBy);
            var skip = Visit(find.Skip);
            var limit = Visit(find.Limit);
            var fields = VisitFieldDeclarationList(find.Fields);
            if (from != find.From || where != find.Where || orderBy != find.OrderBy || groupBy != find.GroupBy || skip != find.Skip || limit != find.Limit || fields != find.Fields)
                return new FindExpression(find.Type, find.Alias, fields, from, where, orderBy, groupBy, find.Distinct, skip, limit);
            return find;
        }

        protected virtual Expression VisitProjection(ProjectionExpression projection)
        {
            var source = (FindExpression)Visit(projection.Source);
            var projector = Visit(projection.Projector);
            if (source != projection.Source || projector != projection.Projector)
                return new ProjectionExpression(source, projector, projection.Aggregator);
            return projection;
        }

        protected ReadOnlyCollection<OrderExpression> VisitOrderBy(ReadOnlyCollection<OrderExpression> orderBys)
        {
            if (orderBys != null) 
            {
                List<OrderExpression> alternate = null;
                for (int i = 0, n = orderBys.Count; i < n; i++) 
                {
                    OrderExpression expr = orderBys[i];
                    Expression e = this.Visit(expr.Expression);
                    if (alternate == null && e != expr.Expression) 
                        alternate = orderBys.Take(i).ToList();
                    if (alternate != null) 
                        alternate.Add(new OrderExpression(expr.OrderType, e));
                }
                if (alternate != null) 
                    return alternate.AsReadOnly();
            }
            return orderBys;
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

        protected virtual ReadOnlyCollection<FieldDeclaration> VisitFieldDeclarationList(ReadOnlyCollection<FieldDeclaration> fields)
        {
            if (fields == null)
                return fields;

            List<FieldDeclaration> alternate = null;
            for (int i = 0, n = fields.Count; i < n; i++)
            {
                var f = fields[i];
                var e = Visit(f.Expression);
                if (f.Expression != e && alternate == null)
                    alternate = fields.Take(i).ToList();
                if (alternate != null)
                    alternate.Add(new FieldDeclaration(f.Name, e));
            }
            if (alternate != null)
                return alternate.AsReadOnly();
            return fields;
        }
    }
}