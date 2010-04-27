using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDB.Linq.Expressions
{
    internal class FieldGatherer : MongoExpressionVisitor
    {
        private List<FieldExpression> _fields;
        private Expression _root;

        public ReadOnlyCollection<FieldExpression> Gather(Expression exp)
        {
            _fields = new List<FieldExpression>();
            _root = exp;
            Visit(exp);
            return _fields.AsReadOnly();
        }

        protected override Expression VisitFind(FindExpression find)
        {
            VisitFieldDeclarationList(find.Fields);
            return find;
        }

        protected override Expression VisitField(FieldExpression field)
        {
            var fields = new FieldGatherer().Gather(field.Expression);
            if (fields.Count == 0)
                _fields.Add(field);
            else
                _fields.AddRange(fields);

            return base.VisitField(field);
        }
    }
}
