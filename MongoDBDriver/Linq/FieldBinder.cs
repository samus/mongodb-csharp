using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Driver.Linq.Expressions;

namespace MongoDB.Driver.Linq
{
    internal class FieldBinder : ExpressionVisitor
    {
        public Expression Bind(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitMemberAccess(System.Linq.Expressions.MemberExpression m)
        {
            if (CanBeField(m.Member.DeclaringType))
            {
                var fieldName = GetFieldName(m);
                if (fieldName != null)
                    return new FieldExpression(fieldName, m);
            }

            return base.VisitMemberAccess(m);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(MongoQueryable))
            {
                if (m.Method.Name == "Key")
                {
                    return new FieldExpression((string)((ConstantExpression)m.Arguments[1]).Value, m);
                }
            }
            else if (typeof(Document).IsAssignableFrom(m.Method.DeclaringType))
            {
                if (m.Method.Name == "get_Item") //TODO: does this work for VB?
                {
                    return new FieldExpression((string)((ConstantExpression)m.Arguments[0]).Value, m);
                }
            }

            return base.VisitMethodCall(m);
        }

        private static bool CanBeField(Type type)
        {
            return true;
        }

        private static string GetFieldName(MemberExpression m)
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

            return null;
        }
    }
}
