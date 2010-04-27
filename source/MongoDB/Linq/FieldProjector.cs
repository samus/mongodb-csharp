using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using MongoDB.Linq.Expressions;

namespace MongoDB.Linq
{
    internal class FieldProjector : MongoExpressionVisitor
    {
        private HashSet<Expression> _candidates;
        private string[] _existingAliases;
        private HashSet<string> _fieldNames;
        private List<FieldDeclaration> _fields;
        private Dictionary<FieldExpression, FieldExpression> _map;
        private string _newAlias;
        private Nominator _nominator;
        private int columnIndex;

        public FieldProjector(Func<Expression, bool> canBeField)
        {
            _nominator = new Nominator(canBeField);
        }

        public FieldProjection ProjectFields(Expression expression, string newAlias, params string[] existingAliases)
        {
            _newAlias = newAlias;
            _existingAliases = existingAliases;
            _fields = new List<FieldDeclaration>();
            _fieldNames = new HashSet<string>();
            _candidates = _nominator.Nominate(expression);
            _map = new Dictionary<FieldExpression, FieldExpression>();
            return new FieldProjection(_fields.AsReadOnly(), Visit(expression));
        }

        protected override Expression Visit(Expression exp)
        {
            if (_candidates.Contains(exp))
            {
                if (exp.NodeType == (ExpressionType)MongoExpressionType.Field)
                {
                    var field = (FieldExpression)exp;
                    FieldExpression mapped;
                    if (_map.TryGetValue(field, out mapped))
                        return mapped;

                    string alias = _existingAliases.Contains(field.Alias) ? field.Alias : _newAlias;
                    var ordinal = _fields.Count;
                    var fieldName = GetUniqueFieldName(field.Name);
                    _fields.Add(new FieldDeclaration(fieldName, field));
                    return new FieldExpression(exp, alias, field.Name);
                }
                else
                {
                    var fieldName = GetNextFieldName();
                    _fields.Add(new FieldDeclaration(fieldName, exp));
                    return new FieldExpression(exp, _newAlias, fieldName);
                }
            }
            return base.Visit(exp);
        }

        private bool IsFieldNameInUse(string name)
        {
            return _fieldNames.Contains(name);
        }

        private string GetUniqueFieldName(string name)
        {
            string baseName = name;
            int suffix = 1;
            while(IsFieldNameInUse(name))
                name = baseName + (suffix++);
            return name;
        }

        private string GetNextFieldName()
        {
            return GetUniqueFieldName("_$f" + (columnIndex++));
        }

        public class FieldProjection
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
        }
    }
}