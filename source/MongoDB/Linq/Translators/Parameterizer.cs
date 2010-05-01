using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Linq.Expressions;

namespace MongoDB.Linq.Translators
{
    internal class Parameterizer : MongoExpressionVisitor
    {
        private int _paramIndex;
        private Dictionary<object, NamedValueExpression> _map;
        private Dictionary<Expression, NamedValueExpression> _pmap;

        public Expression Parameterize(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value != null && !IsNumeric(c.Value.GetType()))
            {
                NamedValueExpression nv;
                if (!_map.TryGetValue(c.Value, out nv))
                {
                    var name = "p" + (_paramIndex++);
                    nv = new NamedValueExpression(name, c);
                    _map.Add(c.Value, nv);
                }
                return nv;
            }
            return c;
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            return GetNamedValue(p);
        }

        protected override Expression VisitProjection(ProjectionExpression projection)
        {
            SelectExpression select = (SelectExpression)Visit(projection.Source);
            if (select != projection.Source)
                return new ProjectionExpression(select, projection.Projector, projection.Aggregator);
            return projection;
        }

        private Expression GetNamedValue(Expression e)
        {
            NamedValueExpression nv;
            if (!_pmap.TryGetValue(e, out nv))
            {
                string name = "p" + (_paramIndex++);
                nv = new NamedValueExpression(name, e);
                _pmap.Add(e, nv);
            }
            return nv;
        }

        private static bool IsNumeric(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }
    }
}