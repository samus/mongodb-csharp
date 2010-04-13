using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Collections.ObjectModel;

namespace MongoDB.Driver.Linq
{
    internal class FieldProjection
    {
        private readonly ReadOnlyCollection<FieldDeclaration> _fields;
        private readonly Expression _projector;

        public ReadOnlyCollection<FieldDeclaration> Fields
        {
            get { return _fields; }
        }

        public Expression Projector
        {
            get { return _projector; }
        }

        public FieldProjection(ReadOnlyCollection<FieldDeclaration> fields, Expression projector)
        {
            _fields = fields;
            _projector = projector;
        }

        public Document CreateDocument()
        {
            var doc = new Document();
            foreach (var field in _fields)
                doc.Add(field.Name, 1);
            return doc;
        }
    }

    internal class FieldProjector : MongoExpressionVisitor
    {
        private HashSet<Expression> _candidates;
        private List<FieldDeclaration> _fields;
        private Nominator _nominator;

        public FieldProjector(Func<Expression, bool> canBeField)
        {
            _nominator = new Nominator(canBeField);
        }

        public FieldProjection ProjectFields(Expression expression)
        {
            _fields = new List<FieldDeclaration>();
            _candidates = _nominator.Nominate(expression);
            return new FieldProjection(_fields.AsReadOnly(), Visit(expression));
        }

        //protected override Expression Visit(Expression exp)
        //{
        //    if (_candidates.Contains(exp))
        //    {
        //        if (exp.NodeType == (ExpressionType)MongoExpressionType.Field)
        //        {
        //            throw new NotSupportedException();
        //        }
        //        else
        //        {
        //            var field = (MemberExpression)exp;
        //            _fields.Add(new FieldDeclaration(field.Member.Name, field));
        //            return new FieldExpression(field.Type, field.Member.Name);
        //        }
        //    }
        //    return base.Visit(exp);
        //}

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            var members = new Stack<MemberInfo>();
            var p = m;
            while (p.Expression != null && p.Expression.NodeType == ExpressionType.MemberAccess)
            {
                members.Push(p.Member);
                p = (MemberExpression)p.Expression;
            }

            if (p.Expression != null && p.Expression.NodeType == ExpressionType.Parameter)
            {
                members.Push(p.Member);
                string fieldName = string.Join(".", members.Select(member => member.Name).ToArray());
                _fields.Add(new FieldDeclaration(fieldName, m));

                return new FieldExpression(m);
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        private class Nominator : MongoExpressionVisitor
        {
            private Func<Expression, bool> _canBeField;
            private HashSet<Expression> _candidates;
            private bool _isBlocked;

            public Nominator(Func<Expression, bool> canBeField)
            {
                _canBeField = canBeField;
            }

            public HashSet<Expression> Nominate(Expression expression)
            {
                _candidates = new HashSet<Expression>();
                _isBlocked = false;
                Visit(expression);
                return _candidates;
            }

            protected override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    var saveIsBlocked = _isBlocked;
                    _isBlocked = false;
                    base.Visit(expression);
                    if (!_isBlocked)
                    {
                        if (_canBeField(expression))
                        {
                            _candidates.Add(expression);
                        }
                        else
                        {
                            _isBlocked = true;
                        }
                    }
                    _isBlocked |= saveIsBlocked;
                }
                return expression;
            }
        }
    }
}