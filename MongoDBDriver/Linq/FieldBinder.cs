using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Driver.Linq.Expressions;
using System.Collections;

namespace MongoDB.Driver.Linq
{
    internal class FieldBinder : ExpressionVisitor
    {
        private static HashSet<Type> _collectionTypes = new HashSet<Type>()
        {
            typeof(ICollection), typeof(ICollection<>)
        };

        private bool _canBeField;
        private Stack<string> _fieldParts;
        private FieldFinder _finder;

        public Expression Bind(Expression expression)
        {
            _finder = new FieldFinder();
            return Visit(expression);
        }

        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
                return exp;

            var fieldName = _finder.Find(exp);
            if (fieldName != null)
                return new FieldExpression(fieldName, exp);

            return base.Visit(exp);
        }

        private class FieldFinder : ExpressionVisitor
        {
            private Stack<string> _fieldParts;
            private bool _isBlocked;

            public string Find(Expression expression)
            {
                _fieldParts = new Stack<string>();
                _isBlocked = false;
                Visit(expression);
                var fieldName = string.Join(".", _fieldParts.ToArray());
                if (_isBlocked)
                    fieldName = null;

                return fieldName;
            }

            protected override Expression Visit(Expression exp)
            {
                if (exp == null)
                    return null;

                switch (exp.NodeType)
                {
                    case ExpressionType.Call:
                    case ExpressionType.MemberAccess:
                    case ExpressionType.Parameter:
                        return base.Visit(exp);
                    default:
                        _isBlocked = true;
                        return exp;
                }
            }

            protected override Expression VisitMemberAccess(System.Linq.Expressions.MemberExpression m)
            {
                var declaringType = m.Member.DeclaringType;
                if (!IsNativeToMongo(declaringType) && !IsCollection(declaringType))
                {
                    _fieldParts.Push(m.Member.Name);
                    Visit(m.Expression);
                    return m;
                }

                _isBlocked = true;
                return m;
            }

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                if (m.Method.DeclaringType == typeof(MongoQueryable))
                {
                    if (m.Method.Name == "Key")
                    {
                        _fieldParts.Push((string)((ConstantExpression)m.Arguments[1]).Value);
                        Visit(m.Arguments[0]);
                        return m;
                    }
                }
                else if (typeof(Document).IsAssignableFrom(m.Method.DeclaringType))
                {
                    if (m.Method.Name == "get_Item") //TODO: does this work for VB?
                    {
                        _fieldParts.Push((string)((ConstantExpression)m.Arguments[0]).Value);
                        Visit(m.Object);
                        return m;
                    }
                }

                _isBlocked = true;
                return m;
            }

            private static bool IsCollection(Type type)
            {
                //HACK: this is going to generally subvert custom objects that implement ICollection or ICollection<T>, 
                //but are not collections
                if (type.IsGenericType)
                    type = type.GetGenericTypeDefinition();

                return _collectionTypes.Any(x => x.IsAssignableFrom(type));
            }

            private static bool IsNativeToMongo(Type type)
            {
                //TODO: this code exists here and in BsonClassMapDescriptor.  Should probably be centralized...
                var typeCode = Type.GetTypeCode(type);

                if (typeCode != TypeCode.Object)
                    return true;

                if (type == typeof(Guid))
                    return true;

                if (type == typeof(Oid))
                    return true;

                if (type == typeof(byte[]))
                    return true;

                return false;
            }
        }
    }
}