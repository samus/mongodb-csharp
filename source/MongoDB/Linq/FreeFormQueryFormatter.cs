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
    internal class FreeFormQueryFormatter : MongoExpressionVisitor
    {
        private MongoQueryObject _queryObject;
        private StringBuilder _where;

        internal MongoQueryObject Format(Expression expression)
        {
            _where = new StringBuilder();
            _queryObject = new MongoQueryObject();
            Visit(expression);
            _queryObject.SetWhereClause(_where.ToString());
            return _queryObject;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            _where.Append("(");
            Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.Equal:
                    _where.Append(" === ");
                    break;
                case ExpressionType.GreaterThan:
                    _where.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _where.Append(" >= ");
                    break;
                case ExpressionType.LessThan:
                    _where.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _where.Append(" <= ");
                    break;
                case ExpressionType.NotEqual:
                    _where.Append(" != ");
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
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    _where.Append(" + ");
                    break;
                case ExpressionType.Coalesce:
                    _where.Append(" || ");
                    break;
                case ExpressionType.Divide:
                    _where.Append(" / ");
                    break;
                case ExpressionType.ExclusiveOr:
                    _where.Append(" ^ ");
                    break;
                case ExpressionType.LeftShift:
                    _where.Append(" << ");
                    break;
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    _where.Append(" * ");
                    break;
                case ExpressionType.RightShift:
                    _where.Append(" >> ");
                    break;
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    _where.Append(" - ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The operation {0} is not supported.", b.NodeType));
            }

            Visit(b.Right);

            _where.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            _where.Append(GetJavascriptValueForConstant(c));
            return c;
        }

        protected override Expression VisitField(FieldExpression f)
        {
            _where.AppendFormat("this.{0}", f.Name);
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
                    _where.Append(".length");
                    return m;
                }
            }
            else if (typeof(ICollection).IsAssignableFrom(m.Member.DeclaringType))
            {
                if (m.Member.Name == "Count")
                {
                    Visit(m.Expression);
                    _where.Append(".length");
                    return m;
                }
            }
            else if (typeof(ICollection<>).IsOpenTypeAssignableFrom(m.Member.DeclaringType))
            {
                if (m.Member.Name == "Count")
                {
                    Visit(m.Expression);
                    _where.Append(".length");
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
                    case "Count":
                        if (m.Arguments.Count == 1)
                        {
                            Visit(m.Arguments[0]);
                            _where.Append(".length");
                            return m;
                        }
                        throw new NotSupportedException("The method Count with a predicate is not supported for field.");
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
                    _where.AppendFormat("/^{0}/", value);
                else if (m.Method.Name == "EndsWith")
                    _where.AppendFormat("/{0}$/", value);
                else if (m.Method.Name == "Contains")
                    _where.AppendFormat("/{0}/", value);
                else
                    throw new NotSupportedException(string.Format("The string method {0} is not supported.", m.Method.Name));

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
                    Visit(u.Operand);
                    _where.Append(")");
                    break;
                case ExpressionType.ArrayLength:
                    Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator {0} is not supported.", u.NodeType));
            }

            return u;
        }

        private void BuildMapReduce(FindExpression find)
        {

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