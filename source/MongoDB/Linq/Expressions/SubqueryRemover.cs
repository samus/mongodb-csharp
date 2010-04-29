using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MongoDB.Linq.Expressions
{
    internal class SubqueryRemover : MongoExpressionVisitor
    {
        private HashSet<SelectExpression> _selectsToRemove;

        public Expression Remove(SelectExpression outerSelect, IEnumerable<SelectExpression> selectsToRemove)
        {
            _selectsToRemove = new HashSet<SelectExpression>(selectsToRemove);
            return Visit(outerSelect);
        }

        protected override Expression VisitSelect(SelectExpression s)
        {
            if (_selectsToRemove.Contains(s))
                return Visit(s.From);

            return base.VisitSelect(s);
        }
    }
}
