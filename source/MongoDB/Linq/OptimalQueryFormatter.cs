using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

using MongoDB.Linq.Expressions;
using MongoDB.Util;

namespace MongoDB.Linq
{
    internal class OptimalQueryFormatter : MongoExpressionVisitor
    {
        private MongoQueryObject _queryObject;

        internal MongoQueryObject Format(Expression expression)
        {
            _queryObject = new MongoQueryObject();
            Visit(expression);
            return _queryObject;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            int scopeDepth = _queryObject.ScopeDepth;
            Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.Equal:
                    break;
                case ExpressionType.GreaterThan:
                    _queryObject.PushConditionScope("$gt");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _queryObject.PushConditionScope("$gte");
                    break;
                case ExpressionType.LessThan:
                    _queryObject.PushConditionScope("$lt");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _queryObject.PushConditionScope("$lte");
                    break;
                case ExpressionType.NotEqual:
                    _queryObject.PushConditionScope("$ne");
                    break;
                case ExpressionType.Modulo:
                    throw new NotImplementedException();
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    break;
                default:
                    throw new NotSupportedException(string.Format("The operation {0} is not supported.", b.NodeType));
            }

            Visit(b.Right);

            while (_queryObject.ScopeDepth > scopeDepth)
                _queryObject.PopConditionScope();

            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            _queryObject.AddCondition(c.Value);
            return c;
        }

        protected override Expression VisitField(FieldExpression f)
        {
            _queryObject.PushConditionScope(f.Name);
            return f;
        }

        protected override Expression VisitFind(FindExpression find)
        {
            if (find.From != null)
                VisitSource(find.From);
            if (find.Where != null)
                Visit(find.Where);

            var fieldGatherer = new FieldGatherer();
            foreach (var field in find.Fields)
            {
                var expandedFields = fieldGatherer.Gather(field.Expression);
                foreach (var expandedField in expandedFields)
                    _queryObject.Fields[expandedField.Name] = 1;
            }

            if (find.OrderBy != null)
            {
                foreach (var order in find.OrderBy)
                {
                    var field = Visit(order.Expression) as FieldExpression;
                    if (field == null)
                        throw new InvalidQueryException("Could not find the field name from the order expression.");
                    _queryObject.AddOrderBy(field.Name, order.OrderType == OrderType.Ascending ? 1 : -1);
                }
            }

            if (find.Limit != null)
                _queryObject.NumberToLimit = EvaluateConstant<int>(find.Limit);

            if (find.Skip != null)
                _queryObject.NumberToSkip = EvaluateConstant<int>(find.Skip);

            return find;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(Array))
            {
                if (m.Member.Name == "Length")
                {
                    Visit(m.Expression);
                    _queryObject.PushConditionScope("$size");
                    return m;
                }
            }
            else if (typeof(ICollection).IsAssignableFrom(m.Member.DeclaringType))
            {
                if (m.Member.Name == "Count")
                {
                    Visit(m.Expression);
                    _queryObject.PushConditionScope("$size");
                    return m;
                }
            }
            else if (typeof(ICollection<>).IsOpenTypeAssignableFrom(m.Member.DeclaringType))
            {
                if (m.Member.Name == "Count")
                {
                    Visit(m.Expression);
                    _queryObject.PushConditionScope("$size");
                    return m;
                }
            }

            throw new NotSupportedException(string.Format("The member {0} is not supported.", m.Member.Name));
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            FieldExpression field;
            if (m.Method.DeclaringType == typeof(Queryable) || m.Method.DeclaringType == typeof(Enumerable))
            {
                switch (m.Method.Name)
                {
                    case "Any":
                        if(m.Arguments.Count != 2)
                            throw new NotSupportedException("Only the Any method with 2 arguments is supported.");

                        field = m.Arguments[0] as FieldExpression;
                        if (field == null)
                            throw new InvalidQueryException("A mongo field must be a part of the Contains method.");
                        Visit(field);
                        _queryObject.PushConditionScope("$elemMatch");
                        Visit(m.Arguments[1]);
                        _queryObject.PopConditionScope(); //elemMatch
                        _queryObject.PopConditionScope(); //field
                        return m;

                    case "Contains":
                        if (m.Arguments.Count != 2)
                            throw new NotSupportedException("Only the Contains method with 2 arguments is supported.");

                        field = m.Arguments[0] as FieldExpression;
                        if (field != null)
                        {
                            Visit(field);
                            _queryObject.AddCondition(EvaluateConstant<object>(m.Arguments[1]));
                            _queryObject.PopConditionScope();
                            return m;
                        }

                        field = m.Arguments[1] as FieldExpression;
                        if (field == null)
                            throw new InvalidQueryException("A mongo field must be a part of the Contains method.");
                        Visit(field);
                        _queryObject.AddCondition("$in", EvaluateConstant<IEnumerable>(m.Arguments[0]));
                        _queryObject.PopConditionScope();
                        return m;
                    case "Count":
                        if (m.Arguments.Count == 1)
                        {
                            Visit(m.Arguments[0]);
                            _queryObject.PushConditionScope("$size");
                            return m;
                        }
                        throw new NotSupportedException("The method Count with a predicate is not supported for field.");
                }
            }
            else if(typeof(ICollection<>).IsOpenTypeAssignableFrom(m.Method.DeclaringType) || typeof(IList).IsAssignableFrom(m.Method.DeclaringType))
            {
                switch(m.Method.Name)
                {
                    case "Contains":
                        field = m.Arguments[0] as FieldExpression;
                        if (field == null)
                            throw new InvalidQueryException(string.Format("The mongo field must be the argument in method {0}.", m.Method.Name));
                        Visit(field);
                        _queryObject.AddCondition("$in", EvaluateConstant<IEnumerable>(m.Object).OfType<object>().ToArray());
                        _queryObject.PopConditionScope();
                        return m;
                }
            }
            else if (m.Method.DeclaringType == typeof(string))
            {
                field = m.Object as FieldExpression;
                if (field == null)
                    throw new InvalidQueryException(string.Format("The mongo field must be the operator for a string operation of type {0}.", m.Method.Name));
                Visit(field);

                var value = EvaluateConstant<string>(m.Arguments[0]);
                if (m.Method.Name == "StartsWith")
                    _queryObject.AddCondition(new MongoRegex(string.Format("^{0}", value)));
                else if (m.Method.Name == "EndsWith")
                    _queryObject.AddCondition(new MongoRegex(string.Format("{0}$", value)));
                else if (m.Method.Name == "Contains")
                    _queryObject.AddCondition(new MongoRegex(string.Format("{0}", value)));
                else
                    throw new NotSupportedException(string.Format("The string method {0} is not supported.", m.Method.Name));

                _queryObject.PopConditionScope();
                return m;
            }
            else if (m.Method.DeclaringType == typeof(Regex))
            {
                if (m.Method.Name == "IsMatch")
                {
                    field = m.Arguments[0] as FieldExpression;
                    if (field == null)
                        throw new InvalidQueryException(string.Format("The mongo field must be the operator for a string operation of type {0}.", m.Method.Name));

                    Visit(field);
                    string value = null;
                    if (m.Object == null)
                        value = EvaluateConstant<string>(m.Arguments[1]);
                    else
                        throw new InvalidQueryException(string.Format("Only the static Regex.IsMatch is supported.", m.Method.Name));

                    _queryObject.AddCondition(new MongoRegex(value));
                    _queryObject.PopConditionScope();
                    return m;
                }
            }

            throw new NotSupportedException(string.Format("The method {0} is not supported.", m.Method.Name));
        }

        protected override Expression VisitSource(Expression source)
        {
            switch ((MongoExpressionType)source.NodeType)
            {
                case MongoExpressionType.Collection:
                    var collection = (CollectionExpression)source;
                    _queryObject.Database = collection.Database;
                    _queryObject.CollectionName = collection.CollectionName;
                    _queryObject.DocumentType = collection.DocumentType;
                    break;
                case MongoExpressionType.Select:
                    Visit(source);
                    break;
                default:
                    throw new InvalidOperationException("Select source is not valid type");
            }
            return source;
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    _queryObject.PushConditionScope("$not");
                    Visit(u.Operand);
                    _queryObject.PopConditionScope();
                    break;
                case ExpressionType.ArrayLength:
                    Visit(u.Operand);
                    _queryObject.PushConditionScope("$size");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator {0} is not supported.", u.NodeType));
            }

            return u;
        }

        private static T EvaluateConstant<T>(Expression e)
        {
            if (e.NodeType != ExpressionType.Constant)
                throw new ArgumentException("Expression must be a constant.");

            return (T)((ConstantExpression)e).Value;
        }
    }
}