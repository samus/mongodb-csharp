using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using MongoDB.Driver.Linq.Expressions;

namespace MongoDB.Driver.Linq
{
    internal class FieldProjector : MongoExpressionVisitor
    {
        private HashSet<Expression> _candidates;
        private List<string> _fields;
        private Nominator _nominator;

        public FieldProjector(Func<Expression, bool> canBeField)
        {
            _nominator = new Nominator(canBeField);
        }

        public FieldProjection ProjectFields(Expression expression)
        {
            _fields = new List<string>();
            _candidates = _nominator.Nominate(expression);
            return new FieldProjection(_fields.AsReadOnly(), Visit(expression));
        }

        protected override Expression Visit(Expression exp)
        {
            if (_candidates.Contains(exp))
            {
                if (exp.NodeType == (ExpressionType)MongoExpressionType.Field)
                    _fields.Add(((FieldExpression)exp).Name);
            }
            return base.Visit(exp);
        }

        public class FieldProjection
        {
            private readonly ReadOnlyCollection<string> _fields;
            private readonly Expression _projector;

            public ReadOnlyCollection<string> Fields
            {
                get { return _fields; }
            }

            public Expression Projector
            {
                get { return _projector; }
            }

            public FieldProjection(ReadOnlyCollection<string> fields, Expression projector)
            {
                _fields = fields;
                _projector = projector;
            }
        }
    }
}