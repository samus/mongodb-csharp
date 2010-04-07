using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

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
            if (source == m.Expression)
                return m;

            //var members = new Stack<MemberInfo>();
            //var p = m;
            //while (p.Expression != null && p.Expression.NodeType == ExpressionType.MemberAccess)
            //{
            //    members.Push(p.Member);
            //    p = (MemberExpression)p.Expression;
            //}

            //if (p.Expression != null && p.Expression.NodeType == ExpressionType.Parameter)
            //{
            //    members.Push(p.Member);
            //    return new FieldExpression(
            //}

            return Expression.MakeMemberAccess(source, m.Member);
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
            var fields = new List<FieldDeclaration>();
            var resultType = typeof(IEnumerable<>).MakeGenericType(collection.ElementType);
            return new ProjectionExpression(
                new SelectExpression(resultType, fields, new CollectionExpression(resultType, collection.Database, collection.CollectionName, collection.ElementType), null),
                Expression.Parameter(collection.ElementType, "document"));
        }

        private static bool CanBeField(Expression expression)
        {
            var m = expression as MemberExpression;
            if (m == null)
                return false;

            var p = m;
            while (p.Expression != null && p.NodeType == ExpressionType.MemberAccess)
                p = (MemberExpression)p.Expression;

            return p.Expression != null && p.NodeType == ExpressionType.Parameter;
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
    }
}