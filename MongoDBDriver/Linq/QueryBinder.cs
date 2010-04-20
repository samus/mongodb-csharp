using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using MongoDB.Driver.Linq.Expressions;

namespace MongoDB.Driver.Linq
{
    internal class QueryBinder : MongoExpressionVisitor
    {
        private Dictionary<ParameterExpression, Expression> _map;
        private FieldProjector _projector;
        private IQueryProvider _provider;
        private Expression _root;
        private List<OrderExpression> _thenBy;
        private bool _inField;

        public QueryBinder(IQueryProvider provider, Expression root)
        {
            _projector = new FieldProjector(CanBeField);
            _provider = provider;
            _root = root;
        }

        public Expression Bind(Expression expression)
        {
            _inField = false;
            _map = new Dictionary<ParameterExpression, Expression>();
            return Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            //reverse the conditionals if the left one is a constant to make things easier in the formatter...
            if (b.Left.NodeType == ExpressionType.Constant)
                b = Expression.MakeBinary(b.NodeType, b.Right, b.Left, b.IsLiftedToNull, b.Method, b.Conversion);

            return base.VisitBinary(b);
        }

        protected override Expression VisitField(FieldExpression f)
        {
            _inField = true;
            var e = base.VisitField(f);
            _inField = false;
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) || m.Method.DeclaringType == typeof(Enumerable))
            {
                //if we are running off a field expression, things get handled in the QueryFormatter
                if (!_inField && m.Arguments[0].NodeType != (ExpressionType)MongoExpressionType.Field)
                {
                    switch (m.Method.Name)
                    {
                        case "Where":
                            return BindWhere(m.Type, m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]));
                        case "Select":
                            return BindSelect(m.Type, m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]));
                        case "OrderBy":
                            return BindOrderBy(m.Type, m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]), OrderType.Ascending);
                        case "OrderByDescending":
                            return BindOrderBy(m.Type, m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]), OrderType.Descending);
                        case "ThenBy":
                            return BindThenBy(m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]), OrderType.Ascending);
                        case "ThenByDescending":
                            return BindThenBy(m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]), OrderType.Descending);
                        case "Take":
                            if (m.Arguments.Count == 2)
                                return this.BindTake(m.Arguments[0], m.Arguments[1]);
                            break;
                        case "Skip":
                            if (m.Arguments.Count == 2)
                                return this.BindSkip(m.Arguments[0], m.Arguments[1]);
                            break;
                        case "First":
                        case "FirstOrDefault":
                        case "Single":
                        case "SingleOrDefault":
                            if (m.Arguments.Count == 1)
                                return BindFirstOrSingle(m.Arguments[0], null, m.Method.Name, m == _root);
                            else if (m.Arguments.Count == 2)
                            {
                                var predicate = (LambdaExpression)StripQuotes(m.Arguments[1]);
                                return BindFirstOrSingle(m.Arguments[0], predicate, m.Method.Name, m == _root);
                            }
                            break;
                    }
                    throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
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

            return m = Expression.MakeMemberAccess(source, m.Member);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            Expression e;
            if (_map.TryGetValue(p, out e))
                return e;
            return p;
        }

        private Expression BindDistinct(Expression source)
        {
            var projection = (ProjectionExpression)Visit(source);
            var select = projection.Source;
            var fieldProjection = _projector.ProjectFields(projection.Projector);
            return new ProjectionExpression(
                new SelectExpression(select.Type, fieldProjection.Fields, projection.Source, null, null, true, null, null),
                fieldProjection.Projector);
        }

        private Expression BindFirstOrSingle(Expression source, LambdaExpression predicate, string kind, bool isRoot)
        {
            var projection = (ProjectionExpression)Visit(source);
            Expression where = null;
            if (predicate != null)
            {
                _map[predicate.Parameters[0]] = projection.Projector;
                where = Visit(predicate.Body);
            }

            Expression limit = kind.StartsWith("First") ? Expression.Constant(1) : null;
            if (limit == null & kind.StartsWith("Single"))
                limit = Expression.Constant(2);

            if (limit != null || where != null)
            {
                var fieldProjection = _projector.ProjectFields(projection.Projector);
                projection = new ProjectionExpression(
                    new SelectExpression(source.Type, fieldProjection.Fields, projection.Source, where, null, false, null, limit),
                    fieldProjection.Projector);
            }
            if (isRoot)
            {
                var elementType = projection.Projector.Type;
                var p = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(elementType), "p");
                var lambda = Expression.Lambda(Expression.Call(typeof(Enumerable), kind, new Type[] { elementType }, p), p);
                return new ProjectionExpression(projection.Source, projection.Projector, lambda);
            }
            return projection;
        }

        private Expression BindOrderBy(Type resultType, Expression source, LambdaExpression orderSelector, OrderType orderType)
        {
            List<OrderExpression> thenBye = _thenBy;
            _thenBy = null;
            var projection = (ProjectionExpression)Visit(source);

            _map[orderSelector.Parameters[0]] = projection.Projector;
            var orderings = new List<OrderExpression>();
            orderings.Add(new OrderExpression(orderType, Visit(orderSelector.Body)));
            if (thenBye != null)
            {
                for (int i = thenBye.Count - 1; i >= 0; i--)
                {
                    var oe = thenBye[i];
                    var lambda = (LambdaExpression)oe.Expression;
                    _map[lambda.Parameters[0]] = projection.Projector;
                    orderings.Add(new OrderExpression(oe.OrderType, Visit(lambda.Body)));
                }
            }

            var fieldProjection = _projector.ProjectFields(projection.Projector);
            return new ProjectionExpression(
                new SelectExpression(resultType, fieldProjection.Fields, projection.Source, null, orderings.AsReadOnly(), false, null, null),
                fieldProjection.Projector);
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

        private Expression BindSkip(Expression source, Expression skip)
        {
            var projection = (ProjectionExpression)Visit(source);
            skip = Visit(skip);
            var select = projection.Source;
            var fieldProjection = _projector.ProjectFields(projection.Projector);
            return new ProjectionExpression(
                new SelectExpression(select.Type, fieldProjection.Fields, projection.Source, null, null, false, skip, null),
                fieldProjection.Projector);
        }

        private Expression BindTake(Expression source, Expression take)
        {
            var projection = (ProjectionExpression)Visit(source);
            take = Visit(take);
            var select = projection.Source;
            var fieldProjection = _projector.ProjectFields(projection.Projector);
            return new ProjectionExpression(
                new SelectExpression(select.Type, fieldProjection.Fields, projection.Source, null, null, false, null, take),
                fieldProjection.Projector);
        }

        private Expression BindThenBy(Expression source, LambdaExpression orderSelector, OrderType orderType)
        {
            if (_thenBy == null)
                _thenBy = new List<OrderExpression>();

            _thenBy.Add(new OrderExpression(orderType, orderSelector));
            return Visit(source);
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


    }
}