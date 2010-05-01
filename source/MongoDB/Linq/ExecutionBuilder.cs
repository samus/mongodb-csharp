using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using MongoDB.Linq.Expressions;
using MongoDB.Linq.Translators;

namespace MongoDB.Linq
{
    internal class ExecutionBuilder : MongoExpressionVisitor
    {
        private List<Expression> _initializers;
        private bool _isTop;
        private int _lookup;
        private int _numCursors;
        private Expression _provider;
        private MemberInfo _receivingMember;
        private List<ParameterExpression> _variables;


        public Expression Build(Expression expression, Expression provider)
        {
            _initializers = new List<Expression>();
            _variables = new List<ParameterExpression>();
            _isTop = true;
            _provider = provider;
            return Build(expression);
        }

        protected override MemberBinding VisitBinding(MemberBinding binding)
        {
            var save = _receivingMember;
            _receivingMember = binding.Member;
            var result = base.VisitBinding(binding);
            _receivingMember = save;
            return result;
        }

        protected override Expression VisitClientJoin(ClientJoinExpression clientJoin)
        {
            var innerKey = MakeJoinKey(clientJoin.InnerKey);
            var outerKey = MakeJoinKey(clientJoin.OuterKey);

            var pairConstructor = typeof(KeyValuePair<,>).MakeGenericType(innerKey.Type, clientJoin.Projection.Projector.Type).GetConstructor(new[] { innerKey.Type, clientJoin.Projection.Projector.Type });
            var constructPair = Expression.New(pairConstructor, innerKey, clientJoin.Projection.Projector);
            var newProjection = new ProjectionExpression(clientJoin.Projection.Source, constructPair);

            int lookupIndex = _lookup++;
            var execution = ExecuteProjection(newProjection);

            var pair = Expression.Parameter(constructPair.Type, "pair");

            if (clientJoin.Projection.Projector.NodeType == (ExpressionType)MongoExpressionType.OuterJoined)
            {
                var lambda = Expression.Lambda(
                    Expression.NotEqual(
                        Expression.PropertyOrField(pair, "Value"),
                        Expression.Constant(null, clientJoin.Projection.Projector.Type)),
                    pair);
                execution = Expression.Call(typeof(Enumerable), "Where", new[] { pair.Type }, execution, lambda);
            }

            var keySelector = Expression.Lambda(Expression.PropertyOrField(pair, "Key"), pair);
            var elementSelector = Expression.Lambda(Expression.PropertyOrField(pair, "Value"), pair);
            var toLookup = Expression.Call(typeof(Enumerable), "ToLookup", new [] { pair.Type, outerKey.Type, clientJoin.Projection.Projector.Type }, execution, keySelector, elementSelector);

            var lookup = Expression.Parameter(toLookup.Type, "lookup" + lookupIndex);
            var prop = lookup.Type.GetProperty("Item");
            Expression access = Expression.Call(lookup, prop.GetGetMethod(), Visit(outerKey));
            if(clientJoin.Projection.Aggregator != null)
                access = new ExpressionReplacer().Replace(clientJoin.Projection.Aggregator.Body, clientJoin.Projection.Aggregator.Parameters[0], access);

            _variables.Add(lookup);
            _initializers.Add(toLookup);

            return access;
        }

        protected override Expression VisitField(FieldExpression field)
        {
            return Visit(field.Expression);
        }

        protected override Expression VisitProjection(ProjectionExpression projection)
        {
            if (_isTop)
            {
                _isTop = false;
                return ExecuteProjection(projection);
            }
            else
            {
                return BuildInner(projection);
            }
        }

        private Expression AddVariables(Expression expression)
        {
            if (_variables.Count > 0)
            {
                var expressions = new List<Expression>();
                for (int i = 0, n = _variables.Count; i < n; i++)
                    expressions.Add(MakeAssign(_variables[i], _initializers[i]));

                var sequence = MakeSequence(expressions);

                var nulls = _variables.Select(v => Expression.Constant(null, v.Type)).ToArray();
                expression = Expression.Invoke(Expression.Lambda(sequence, _variables.ToArray()), nulls);
            }
            return expression;
        }

        private Expression Build(Expression expression)
        {
            expression = Visit(expression);
            expression = AddVariables(expression);
            return expression;
        }

        private Expression BuildInner(Expression expression)
        {
            var builder = new ExecutionBuilder();
            builder._receivingMember = _receivingMember;
            builder._numCursors = _numCursors;
            builder._lookup = _lookup;
            return builder.Build(expression);
        }

        private Expression ExecuteProjection(ProjectionExpression projection)
        {
            var projection = base.VisitProjection(projection);
            var queryObject = new MongoQueryObjectBuilder().Build(projection);
            queryObject.Projector = new ProjectionBuilder().Build(projection.Projector, queryObject.DocumentType, "d" + (_numCursors++), queryObject.IsMapReduce);
            queryObject.Aggregator = projection.Aggregator;

            Expression result = Expression.Call(
                _provider,
                "ExecuteQueryObject",
                Type.EmptyTypes,
                Expression.Constant(queryObject, typeof(MongoQueryObject)));

            return result;
        }

        public static T Assign<T>(ref T variable, T value)
        {
            variable = value;
            return value;
        }

        public static object Sequence(params object[] values)
        {
            return values[values.Length - 1];
        }

        private static Expression MakeAssign(ParameterExpression variable, Expression value)
        {
            return Expression.Call(typeof(ExecutionBuilder), "Assign", new[] { variable.Type }, variable, value);
        }

        private static Expression MakeJoinKey(IList<Expression> key)
        {
            if (key.Count == 1)
                return key[0];
            else
            {
                return Expression.New(
                    typeof(CompoundKey).GetConstructors()[0],
                    Expression.NewArrayInit(typeof(object), key.Select(k => (Expression)Expression.Convert(k, typeof(object)))));
            }
        }

        private static Expression MakeSequence(IList<Expression> expressions)
        {
            var last = expressions[expressions.Count - 1];
            return Expression.Convert(Expression.Call(typeof(ExecutionBuilder), "Sequence", null, Expression.NewArrayInit(typeof(object), expressions)), last.Type);
        }

        private class CompoundKey : IEquatable<CompoundKey>
        {
            private object[] _values;
            private int _hashCode;

            public CompoundKey(params object[] values)
            {
                _values = values;
                for (int i = 0, n = values.Length; i < n; i++)
                {
                    object value = values[i];
                    if (value != null)
                        _hashCode ^= (value.GetHashCode() + i);
                }
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public bool Equals(CompoundKey other)
            {
                if (other == null || other._values.Length != _values.Length)
                    return false;
                for (int i = 0, n = other._values.Length; i < n; i++)
                {
                    if (!object.Equals(_values[i], other._values[i]))
                        return false;
                }
                return true;
            }
        }
    }
}