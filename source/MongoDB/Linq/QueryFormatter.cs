using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Linq.Expressions;
using System.Linq.Expressions;

namespace MongoDB.Linq
{
    internal class QueryFormatter : MongoExpressionVisitor
    {
        private bool _isCount;
        private bool _isFreeForm;
        private bool _isMapReduce;

        public MongoQueryObject Format(Expression expression)
        {
            _isCount = false;
            _isFreeForm = false;
            _isMapReduce = false;
            Visit(expression);

            MongoQueryObject queryObject;
            if (_isMapReduce)
                queryObject = null;
            else if (_isFreeForm)
                queryObject = new FreeFormQueryFormatter().Format(expression);
            else
                queryObject = new OptimalQueryFormatter().Format(expression);

            if (_isCount)
                queryObject.IsCount = true;

            return queryObject;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.NotEqual:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                    break;
                default:
                    _isFreeForm = true;
                    break;
            }

            //TODO: figure out how to test this...
            //if (b.Right.NodeType != ExpressionType.Constant)
            //    _isComplex = true;

            Visit(b.Right);

            return b;
        }

        protected override Expression VisitFind(FindExpression find)
        {
            if (find.From.NodeType != (ExpressionType)MongoExpressionType.Collection)
                throw new InvalidQueryException("The query is too complex to be processed by MongoDB. Try building a map-reduce query by hand or simplifying the query and using Linq-to-Objects.");

            bool hasAggregates = new AggregateChecker().HasAggregates(find);

            if (find.GroupBy != null && find.GroupBy.Count > 0)
                _isMapReduce = true;
            else if (hasAggregates)
            {
                if (find.Fields.Count == 1 && find.Fields[0].Expression.NodeType == (ExpressionType)MongoExpressionType.Aggregate)
                {
                    var aggregateExpression = (AggregateExpression)find.Fields[0].Expression;
                    if (aggregateExpression.AggregateType == AggregateType.Count)
                        _isCount = true;
                }

                if (!_isCount)
                    _isMapReduce = true;
            }

            Visit(find.Where);
            return find;
        }
    }
}