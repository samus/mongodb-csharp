using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace MongoDB.Linq.Expressions
{
    internal class SelectExpression : AliasedExpression
    {
        private readonly bool _distinct;
        private readonly ReadOnlyCollection<FieldDeclaration> _fields;
        private readonly Expression _from;
        private readonly Expression _groupBy;
        private readonly Expression _take;
        private readonly ReadOnlyCollection<OrderExpression> _orderBy;
        private readonly Expression _skip;
        private readonly Expression _where;

        public bool Distinct
        {
            get { return _distinct; }
        }

        public ReadOnlyCollection<FieldDeclaration> Fields
        {
            get { return _fields; }
        }

        public Expression From
        {
            get { return _from; }
        }

        public Expression GroupBy
        {
            get { return _groupBy; }
        }

        public Expression Take
        {
            get { return _take; }
        }

        public ReadOnlyCollection<OrderExpression> OrderBy
        {
            get { return _orderBy; }
        }

        public Expression Skip
        {
            get { return _skip; }
        }

        public Expression Where
        {
            get { return _where; }
        }

        public SelectExpression(Alias alias, IEnumerable<FieldDeclaration> fields, Expression from, Expression where)
            : this(alias, fields, from, where, null, null)
        { }

        public SelectExpression(Alias alias, IEnumerable<FieldDeclaration> fields, Expression from, Expression where, IEnumerable<OrderExpression> orderBy, Expression groupBy)
            : this(alias, fields, from, where, orderBy, groupBy, false, null, null)
        { }

        public SelectExpression(Alias alias, IEnumerable<FieldDeclaration> fields, Expression from, Expression where, IEnumerable<OrderExpression> orderBy, Expression groupBy, bool distinct, Expression skip, Expression take)
            : base(MongoExpressionType.Select, typeof(void), alias)
        {
            _fields = fields as ReadOnlyCollection<FieldDeclaration>;
            if (_fields == null)
                _fields = new List<FieldDeclaration>(fields).AsReadOnly();

            _orderBy = orderBy as ReadOnlyCollection<OrderExpression>;
            if (_orderBy == null && orderBy != null)
                _orderBy = new List<OrderExpression>(orderBy).AsReadOnly();

            _distinct = distinct;
            _from = from;
            _groupBy = groupBy;
            _take = take;
            _where = where;
            _skip = skip;
        }
    }
}