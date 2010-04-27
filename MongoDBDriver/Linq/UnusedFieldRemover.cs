using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Linq.Expressions;
using System.Linq.Expressions;

namespace MongoDB.Driver.Linq
{
    internal class UnusedFieldRemover : MongoExpressionVisitor
    {
        private Dictionary<string, HashSet<string>> _allFieldsUsed;

        public Expression Remove(Expression expression)
        {
            _allFieldsUsed = new Dictionary<string, HashSet<string>>();
            return Visit(expression);
        }

        protected override Expression VisitField(FieldExpression field)
        {
            MarkFieldAsUsed(field.Alias, field.Name);
            return field;
        }

        protected override Expression VisitSubquery(SubqueryExpression subquery)
        {
            if (subquery.NodeType == (ExpressionType)MongoExpressionType.Scalar && subquery.Find != null)
                MarkColumnAsUsed(subquery.Find.Alias, subquery.Find.Fields[0].Name);
            
            return base.VisitSubquery(subquery);
        }

        private void ClearFieldsUsed(string alias)
        {
            _allFieldsUsed[alias] = new HashSet<string>();
        }

        private bool IsFieldUsed(string alias, string name)
        {
            HashSet<string> fields;
            if (_allFieldsUsed.TryGetValue(alias, out fields))
                return fields.Contains(name);

            return false;
        }

        private void MarkFieldAsUsed(string alias, string name)
        {
            HashSet<string> fields;
            if (!_allFieldsUsed.TryGetValue(alias, out fields))
                _allFieldsUsed.Add(fields = new HashSet<string>());
            fields.Add(name);
        }
    }
}