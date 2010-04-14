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

        protected virtual Expression VisitProjection(ProjectionExpression p)
        {
            var source = (SelectExpression)Visit(p.Source);
            var projector = Visit(p.Projector);
            if (source != p.Source || projector != p.Projector)
                return new ProjectionExpression(source, projector);
            return p;
        }

        protected virtual Expression VisitSelect(SelectExpression s)
        {
            var from = VisitSource(s.From);
            var where = Visit(s.Where);
            if (from != s.From || where != s.Where)
                return new SelectExpression(s.Type, s.Fields, from, where);
            return s;
        }

        protected virtual Expression VisitSource(Expression source)
        {
            return Visit(source);
        }

        protected static string GetFieldName(MemberExpression m)
        {
            var memberNames = new Stack<string>();
            var p = m;
            while (p.Expression != null && p.Expression.NodeType == ExpressionType.MemberAccess)
            {
                memberNames.Push(p.Member.Name);
                p = (MemberExpression)p.Expression;
            }

            if (p.Expression != null && p.Expression.NodeType == ExpressionType.Parameter)
            {
                memberNames.Push(p.Member.Name);
                return string.Join(".", memberNames.ToArray());
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }
    }
}