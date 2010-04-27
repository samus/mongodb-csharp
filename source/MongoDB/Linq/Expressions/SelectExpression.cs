using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace MongoDB.Driver.Linq.Expressions
{
    internal class SelectExpression : Expression
    {
        private readonly bool _distinct;
        private readonly ReadOnlyCollection<FieldExpression> _fields;
        private readonly Expression _from;
        private readonly Expression _limit;
        private readonly ReadOnlyCollection<OrderExpression> _order;
        private readonly Expression _skip;
        private readonly Expression _where;

        public bool Distinct
        {
            get { return _distinct; }
        }

        public ReadOnlyCollection<FieldExpression> Fields
        {
            get { return _fields; }
        }

        public Expression From
        {
            get { return _from; }
        }

        public Expression Limit
        {
            get { return _limit; }
        }

        public ReadOnlyCollection<OrderExpression> Order
        {
            get { return _order; }
        }

        public Expression Skip
        {
            get { return _skip; }
        }

        public Expression Where
        {
            get { return _where; }
        }

        public SelectExpression(Type type, IEnumerable<FieldExpression> fields, Expression from, Expression where)
            : this(type, fields, from, where, null, false, null, null)
        { }

        public SelectExpression(Type type, IEnumerable<FieldExpression> fields, Expression from, Expression where, IEnumerable<OrderExpression> order, bool distinct, Expression skip, Expression limit)
            : base((ExpressionType)MongoExpressionType.Select, type)
        {
            _fields = fields as ReadOnlyCollection<FieldExpression>;
            if (_fields == null)
                _fields = new List<FieldExpression>(fields).AsReadOnly();

            _order = order as ReadOnlyCollection<OrderExpression>;
            if (_order == null && order != null)
                _order = new List<OrderExpression>(order).AsReadOnly();

            _distinct = distinct;
            _from = from;
            _limit = limit;
            _where = where;
            _skip = skip;
        }
    }
}