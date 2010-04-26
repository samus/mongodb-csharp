using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

using MongoDB.Driver.Linq.Expressions;
using MongoDB.Driver.Util;

namespace MongoDB.Driver.Linq
{
    internal class QueryFormatter : MongoExpressionVisitor
    {
        private bool _isComplex;
        private MongoQueryObject _queryObject;
        private StringBuilder _where;

        internal MongoQueryObject Format(Expression e)
        {
            _isComplex = false;
            _where = new StringBuilder();
            _queryObject = new MongoQueryObject();
            Visit(e);
            if (_isComplex)
                _queryObject.SetWhereClause(_where.ToString());

            return _queryObject;
        }

        protected override Expression VisitAggregate(AggregateExpression a)
        {
            if (a.AggregateType == AggregateType.Count)
                _queryObject.IsCount = true;

            return base.VisitAggregate(a);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            int scopeDepth = _queryObject.ScopeDepth;
            _where.Append("(");
            Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.Equal:
                    _where.Append(" === ");
                    break;
                case ExpressionType.GreaterThan:
                    _where.Append(" > ");
                    _queryObject.PushConditionScope("$gt");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _where.Append(" >= ");
                    _queryObject.PushConditionScope("$gte");
                    break;
                case ExpressionType.LessThan:
                    _where.Append(" < ");
                    _queryObject.PushConditionScope("$lt");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _where.Append(" <= ");
                    _queryObject.PushConditionScope("$lte");
                    break;
                case ExpressionType.NotEqual:
                    _where.Append(" != ");
                    _queryObject.PushConditionScope("$ne");
                    break;
                case ExpressionType.Modulo:
                    throw new NotImplementedException();
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    _where.Append(" && ");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    _where.Append(" || ");
                    _isComplex = true;
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    _where.Append(" + ");
                    _isComplex = true;
                    break;
                case ExpressionType.Coalesce:
                    _where.Append(" || ");
                    _isComplex = true;
                    break;
                case ExpressionType.Divide:
                    _where.Append(" / ");
                    _isComplex = true;
                    break;
                case ExpressionType.ExclusiveOr:
                    _where.Append(" ^ ");
                    _isComplex = true;
                    break;
                case ExpressionType.LeftShift:
                    _where.Append(" << ");
                    _isComplex = true;
                    break;
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    _where.Append(" * ");
                    _isComplex = true;
                    break;
                case ExpressionType.RightShift:
                    _where.Append(" >> ");
                    _isComplex = true;
                    break;
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    _where.Append(" - ");
                    _isComplex = true;
                    break;
                default:
                    throw new NotSupportedException(string.Format("The operation {0} is not supported.", b.NodeType));
            }

            //TODO: figure out how to test this...
            //if (b.Right.NodeType != ExpressionType.Constant)
            //    _isComplex = true;

            Visit(b.Right);

            while (_queryObject.ScopeDepth > scopeDepth)
                _queryObject.PopConditionScope();

            _where.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            _where.Append(GetJavascriptValueForConstant(c));
            _queryObject.AddCondition(c.Value);
            return c;
        }

        protected override Expression VisitField(FieldExpression f)
        {
            _queryObject.PushConditionScope(f.Name);
            _where.AppendFormat("this.{0}", f.Name);
            return f;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(Array))
            {
                if (m.Member.Name == "Length")
                {
                    Visit(m.Expression);
                    _where.Append(".length");
                    _queryObject.PushConditionScope("$size");
                    return m;
                }
            }
            else if (typeof(ICollection).IsAssignableFrom(m.Member.DeclaringType))
            {
                if (m.Member.Name == "Count")
                {
                    Visit(m.Expression);
                    _where.Append(".length");
                    _queryObject.PushConditionScope("$size");
                    return m;
                }
            }
            else if (typeof(ICollection<>).IsOpenTypeAssignableFrom(m.Member.DeclaringType))
            {
                if (m.Member.Name == "Count")
                {
                    Visit(m.Expression);
                    _where.Append(".length");
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
                        //TODO: _where
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
                        //TODO: _where
                        _queryObject.AddCondition("$in", EvaluateConstant<IEnumerable>(m.Arguments[0]));
                        _queryObject.PopConditionScope();
                        return m;
                    case "Count":
                        if (m.Arguments.Count == 1)
                        {
                            Visit(m.Arguments[0]);
                            _where.Append(".length");
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
                        //TODO: _where
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
                {
                    _where.AppendFormat("/^{0}/", value);
                    _queryObject.AddCondition(new MongoRegex(string.Format("^{0}", value)));
                }
                else if (m.Method.Name == "EndsWith")
                {
                    _where.AppendFormat("/{0}$/", value);
                    _queryObject.AddCondition(new MongoRegex(string.Format("{0}$", value)));
                }
                else if (m.Method.Name == "Contains")
                {
                    _where.AppendFormat("/{0}/", value);
                    _queryObject.AddCondition(new MongoRegex(string.Format("{0}", value)));
                }
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

                    _where.AppendFormat("/{0}/", value);
                    _queryObject.AddCondition(new MongoRegex(value));
                    _queryObject.PopConditionScope();
                    return m;
                }
            }

            throw new NotSupportedException(string.Format("The method {0} is not supported.", m.Method.Name));
        }

        protected override Expression VisitSelect(SelectExpression s)
        {
            //We couldn't reduce it to a single query...
            if (s.From.NodeType != (ExpressionType)MongoExpressionType.Collection)
                throw new NotSupportedException("The query is too complex to be processed by MongoDB. Try building a map-reduce query by hand or simplifying the query.");

            if(s.From != null)
                VisitSource(s.From);
            if (s.Where != null)
                Visit(s.Where);

            foreach (var field in s.Fields)
            {
                if (field.Expression.NodeType == (ExpressionType)MongoExpressionType.Aggregate)
                    Visit(field.Expression);
                else
                    _queryObject.Fields[field.Name] = 1;
            }

            if (s.OrderBy != null)
            {
                foreach (var order in s.OrderBy)
                {
                    var field = Visit(order.Expression) as FieldExpression;
                    if (field == null)
                        throw new InvalidQueryException("Could not find the field name from the order expression.");
                    _queryObject.AddOrderBy(field.Name, order.OrderType == OrderType.Ascending ? 1 : -1);
                }
            }

            if (s.Limit != null)
                _queryObject.NumberToLimit = EvaluateConstant<int>(s.Limit);

            if (s.Skip!= null)
                _queryObject.NumberToSkip = EvaluateConstant<int>(s.Skip);

            return s;
        }

        protected override Expression VisitSource(Expression source)
        {
            switch ((MongoExpressionType)source.NodeType)
            {
                case MongoExpressionType.Collection:
                    var collection = (CollectionExpression)source;
                    _queryObject.CollectionName = collection.CollectionName;
                    _queryObject.Database = collection.Database;
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
                    _where.Append("!(");
                    _queryObject.PushConditionScope("$not");
                    Visit(u.Operand);
                    _queryObject.PopConditionScope();
                    _where.Append(")");
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

        private static string GetJavascriptValueForConstant(ConstantExpression c)
        {
            if(c.Value == null)
                return "null";
            if (c.Type == typeof(string) || c.Type == typeof(StringBuilder))
                return string.Format(@"""{0}""", c.Value.ToString());
            
            return c.Value.ToString();
        }
    }
}