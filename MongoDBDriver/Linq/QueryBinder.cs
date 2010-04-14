using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using MongoDB.Driver.Linq.Expressions;

namespace MongoDB.Driver.Linq
{
    internal class QueryBinder : ExpressionVisitor
    {
        private FieldProjector _projector;
        private Dictionary<ParameterExpression, Expression> _map;

        public QueryBinder()
        {
            _projector = new FieldProjector(CanBeField);
        }

        public Expression Bind(Expression expression)
        {
            _map = new Dictionary<ParameterExpression, Expression>();
            return Visit(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) || m.Method.DeclaringType == typeof(Enumerable))
            {
                switch (m.Method.Name)
                {
                    case "Where":
                        return BindWhere(m.Type, m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]));
                    case "Select":
                        return BindSelect(m.Type, m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]));
                }
                throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
            }
            else if (m.Method.DeclaringType == typeof(MongoQueryable))
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

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (IsCollection(c.Value))
                return GetCollectionProjection(c.Value);
            return base.VisitConstant(c);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            var fieldName = GetFieldName(m);

            var source = Visit(m.Expression);
            switch (source.NodeType)
            {
                case ExpressionType.MemberInit:
                    var init = (MemberInitExpression)source;
                    for (int i = 0, n = init.Bindings.Count; i < n; i++)
                    {
                        var ma = init.Bindings[i] as MemberAssignment;
                        if (ma != null && MembersMatch(ma.Member, m.Member))
                            return ma.Expression;
                    }
                    break;
                case ExpressionType.New:
                    var nex = (NewExpression)source;
                    if (nex.Members != null)
                    {
                        for (int i = 0, n = nex.Members.Count; i < n; i++)
                        {
                            if (MembersMatch(nex.Members[i], m.Member))
                                return nex.Arguments[i];
                        }
                    }
                    break;
            }

            Expression ret;
            if (source == m.Expression)
                ret = m;
            else
                ret = Expression.MakeMemberAccess(source, m.Member);

            if (fieldName != null)
                ret = new FieldExpression(fieldName, ret);

            return ret;
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            Expression e;
            if (_map.TryGetValue(p, out e))
                return e;
            return p;
        }

        private Expression BindSelect(Type resultType, Expression source, LambdaExpression selector)
        {
            var projection = (ProjectionExpression)Visit(source);
            _map[selector.Parameters[0]] = projection.Projector;
            var expression = Visit(selector.Body);
            var fieldProjection = _projector.ProjectFields(expression);
            return new ProjectionExpression(
                new SelectExpression(resultType, fieldProjection.Fields, projection.Source, null),
                fieldProjection.Projector);
        }

        private Expression BindWhere(Type resultType, Expression source, LambdaExpression predicate)
        {
            var projection = (ProjectionExpression)Visit(source);
            _map[predicate.Parameters[0]] = projection.Projector;
            var where = Visit(predicate.Body);
            var fieldProjection = _projector.ProjectFields(projection.Projector);
            return new ProjectionExpression(
                new SelectExpression(resultType, fieldProjection.Fields, projection.Source, where),
                fieldProjection.Projector);
        }

        private ProjectionExpression GetCollectionProjection(object value)
        {
            var collection = (IMongoQueryable)value;
            var bindings = new List<MemberBinding>();
            var fields = new List<string>();
            var resultType = typeof(IEnumerable<>).MakeGenericType(collection.ElementType);
            return new ProjectionExpression(
                new SelectExpression(resultType, fields, new CollectionExpression(resultType, collection.Database, collection.CollectionName, collection.ElementType), null),
                Expression.Parameter(collection.ElementType, "document"));
        }

        private static bool CanBeField(Expression expression)
        {
            return expression.NodeType == (ExpressionType)MongoExpressionType.Field;
        }

        private static bool IsCollection(object value)
        {
            var q = value as IMongoQueryable;
            return q != null && q.Expression.NodeType == ExpressionType.Constant;
        }

        private static bool MembersMatch(MemberInfo a, MemberInfo b)
        {
            if (a == b)
                return true;
            if (a is MethodInfo && b is PropertyInfo)
                return a == ((PropertyInfo)b).GetGetMethod();
            else if (a is PropertyInfo && b is MethodInfo)
                return ((PropertyInfo)a).GetGetMethod() == b;
            return false;
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
                e = ((UnaryExpression)e).Operand;
            return e;
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