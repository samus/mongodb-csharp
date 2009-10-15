using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace MongoDB.Linq {
    public class MongoQuerySpec {
        public readonly Document Query;
        public readonly int Limit;
        public readonly int Skip;
        public readonly Document Fields;
        public readonly Document SortOrder;
        public readonly bool IsFirstCall;

        public MongoQuerySpec(Document query, int limit, int skip, Document fields, Document sortOrder, bool isFirstCall) {
            Query = query;
            Limit = limit;
            Skip = skip;
            Fields = fields;
            SortOrder = sortOrder;
            IsFirstCall = isFirstCall;
        }
    }

    public class MongoQueryTranslator : ExpressionVisitor {

        private Document query;
        private int limit;
        private int skip;
        private Document fields;
        private Document sortOrder;
        private bool isFirstCall = false;
        private bool inConditional = false;
        private bool foundKey = false;
        private readonly Stack valueStack = new Stack();
        private readonly Stack<string> keyStack = new Stack<string>();

        protected Document Query {
            get {
                if (query == null) {
                    query = new Document();
                }
                return query;
            }
        }

        protected Document Fields {
            get {
                if (fields == null) {
                    fields = new Document();
                }
                return fields;
            }
        }

        protected Document SortOrder {
            get {
                if (sortOrder == null) {
                    sortOrder = new Document();
                }
                return sortOrder;
            }
        }

        public MongoQuerySpec Translate(Expression expression) {
            Visit(expression);
            return new MongoQuerySpec(query, limit, skip, fields, sortOrder, isFirstCall);
        }

        private static Expression StripQuotes(Expression e) {
            while (e.NodeType == ExpressionType.Quote) {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m) {
            Debug.WriteLine(string.Format("Method call: {0}", m.Method.Name));
            if (m.Method.DeclaringType == typeof(Queryable)) {
                switch (m.Method.Name) {
                    case "Where":
                        Visit(m.Arguments[0]);
                        var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                        Visit(lambda.Body);
                        break;
                    case "Skip":
                        Visit(m.Arguments[0]);
                        Visit(m.Arguments[1]);
                        skip = (int)valueStack.Pop();
                        break;
                    case "Take":
                        Visit(m.Arguments[0]);
                        Visit(m.Arguments[1]);
                        limit = (int)valueStack.Pop();
                        break;
                    case "First":
                        isFirstCall = true;
                        limit = 1;
                        Visit(m.Arguments[0]);
                        break;
                    case "FirstOrDefault":
                        limit = 1;
                        Visit(m.Arguments[0]);
                        break;
                    case "Select":
                        break;
                    default:
                        throw new NotSupportedException(string.Format("The method '{0}' on queryable type is not supported", m.Method.Name));
                }

                return m;
            }
            if (m.Object == null && m.Method.Name == "Key") {
                Visit(m.Arguments[1]);
                keyStack.Push((string)valueStack.Pop());
                foundKey = true;
                return m;
            }
            if (m.Object != null && typeof(MongoDocumentQuery).IsAssignableFrom(m.Object.Type)) {
                Debug.WriteLine("call on MongoDocumentQuery");
                Visit(((MethodCallExpression)m.Object).Arguments[1]);
                keyStack.Push((string)valueStack.Pop());
                foundKey = true;
                switch (m.Method.Name) {
                    case "In":
                    case "NotIn":
                        var argsLambda = Expression.Lambda(m.Arguments[0]);
                        var argsValue = argsLambda.Compile().DynamicInvoke();
                        Query.Add(keyStack.Pop(), new Document().Append((m.Method.Name == "In") ? "$in" : "$nin", argsValue));
                        break;
                    case "Equals":
                        Visit(m.Arguments[0]);
                        AddEqualityQuery();
                        break;
                }
                return m;
            }
            if (m.Method.Name == "Equals") {
                if (m.Object == null) {
                    Visit(m.Arguments[0]);
                    Visit(m.Arguments[1]);
                } else {
                    Visit(m.Object);
                    Visit(m.Arguments[0]);
                }
                AddEqualityQuery();
                return m;
            }
            if (m.Object != null && typeof(Document).IsAssignableFrom(m.Object.Type) && m.Method.Name == "get_Item") {
                Debug.WriteLine("Document indexer access, divining query key");
                Visit(m.Arguments[0]);
                keyStack.Push((string)valueStack.Pop());
                foundKey = true;
                return m;
            }

            Debug.WriteLine("unrecognized method call, trying to convert to constant value");
            try {
                var methodCallLambda = Expression.Lambda(m);
                var methodConstValue = methodCallLambda.Compile().DynamicInvoke();
                valueStack.Push(methodConstValue);
            } catch (Exception e) {
                throw new NotSupportedException(string.Format("The method '{0}' could not be converted into a constant", m.Method.Name), e);
            }
            return m;
        }

        private void AddEqualityQuery() {
            var key = keyStack.Pop();
            var value = valueStack.Pop();
            Query.Append(key, value);
        }

        protected override Expression VisitUnary(UnaryExpression u) {
            Debug.WriteLine(string.Format("Unary type: {0}", u.NodeType));
            switch (u.NodeType) {
                case ExpressionType.Not:
                    break;
                case ExpressionType.Convert:
                    Visit(StripConvert(u));
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            } return u;
        }

        private Expression StripConvert(Expression expression) {
            while (expression.NodeType == ExpressionType.Convert) {
                expression = ((UnaryExpression)expression).Operand;
            }
            return expression;
        }

        protected override Expression VisitBinary(BinaryExpression b) {
            Debug.WriteLine(string.Format("Binary type: {0}", b.NodeType));
            string key;
            object value;
            switch (b.NodeType) {
                case ExpressionType.Equal:
                    Visit(b.Left);
                    Visit(b.Right);
                    AddEqualityQuery();
                    break;
                case ExpressionType.NotEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    if (inConditional) {
                        throw new NotSupportedException("cannot handle nested conditionals");
                    }
                    // Note (sdether): because of conditional ordering, left and right visits have to happen inside the
                    // inConditional = true block, which is why the visits cannot be moved to the top of the switch
                    inConditional = true;
                    Visit(b.Left);
                    bool reverseConditional = false;
                    if (!foundKey) {
                        reverseConditional = true;
                    }
                    Visit(b.Right);
                    key = keyStack.Pop();
                    value = valueStack.Pop();
                    var conditional = "$ne";
                    switch (b.NodeType) {
                        case ExpressionType.LessThan: conditional = reverseConditional ? "$gt" : "$lt"; break;
                        case ExpressionType.LessThanOrEqual: conditional = reverseConditional ? "$gte" : "$lte"; break;
                        case ExpressionType.GreaterThan: conditional = reverseConditional ? "$lt" : "$gt"; break;
                        case ExpressionType.GreaterThanOrEqual: conditional = reverseConditional ? "$lte" : "$gte"; break;
                    }
                    Query.Append(key, new Document().Append(conditional, value));
                    inConditional = false;
                    foundKey = false;
                    break;
                case ExpressionType.AndAlso:
                    Visit(b.Left);
                    Visit(b.Right);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }

            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c) {
            Debug.WriteLine(string.Format("constant: ({0}){1}", c.Type, c.Value));
            if (c.Value is MongoQuery) {
                Debug.WriteLine("constant of type MongoQuery is our terminal, ignore");
            } else {
                switch (Type.GetTypeCode(c.Value.GetType())) {
                    case TypeCode.Boolean:
                    case TypeCode.DateTime:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Single:
                    case TypeCode.String:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        valueStack.Push(c.Value);
                        break;
                    default:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
                }
            }
            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m) {
            Debug.WriteLine(string.Format("Member Access: {0}", m.Member.Name));
            if (m.Expression != null) {
                if (m.Expression.NodeType == ExpressionType.Parameter) {
                    return m;
                }
                if (m.Expression != null && m.Expression.NodeType == ExpressionType.Constant) {
                    switch (m.Member.MemberType) {
                        case MemberTypes.Property:
                            var propertyInfo = (PropertyInfo)m.Member;
                            var innerMember = (MemberExpression)m.Expression;
                            var closureFieldInfo = (FieldInfo)innerMember.Member;
                            var obj = closureFieldInfo.GetValue(((ConstantExpression)innerMember.Expression).Value);
                            valueStack.Push(propertyInfo.GetValue(obj, null));
                            break;
                        case MemberTypes.Field:
                            var fieldInfo = (FieldInfo)m.Member;
                            valueStack.Push(fieldInfo.GetValue(((ConstantExpression)m.Expression).Value));
                            break;
                        default:
                            Visit(m.Expression);
                            break;
                    }
                    return m;
                }
            }
            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }
    }
}