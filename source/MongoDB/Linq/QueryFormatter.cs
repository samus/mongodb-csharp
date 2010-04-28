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
        private MongoQueryObject _queryObject;
        private QueryAttributes _queryAttributes;

        internal MongoQueryObject Format(Expression expression)
        {
            _queryObject = new MongoQueryObject();
            _queryAttributes = new QueryAttributesGatherer().Gather(expression);
            _queryObject.IsCount = _queryAttributes.IsCount;
            Visit(expression);
            return _queryObject;
        }

        protected override Expression VisitFind(FindExpression find)
        {
            if (find.From != null)
                VisitSource(find.From);
            if (find.Where != null)
            {
                if (_queryAttributes.IsFreeForm)
                    _queryObject.SetWhereClause(new JavascriptFormatter().FormatJavascript(find.Where));
                else
                    _queryObject.SetQueryDocument(new DocumentFormatter().FormatDocument(find.Where));
            }

            if (_queryAttributes.IsMapReduce)
            {
                _queryObject.IsMapReduce = true;
                var mapFunction = new MapReduceMapFunctionBuilder().Build(find.Fields, find.GroupBy);
                var reduceFunction = new MapReduceReduceFunctionBuilder().Build(find.Fields);
            }
            else if(!_queryAttributes.IsCount)
            {
                var fieldGatherer = new FieldGatherer();
                foreach (var field in find.Fields)
                {
                    var expandedFields = fieldGatherer.Gather(field.Expression);
                    foreach (var expandedField in expandedFields)
                        _queryObject.Fields[expandedField.Name] = 1;
                }
            }

            if (find.OrderBy != null)
            {
                foreach (var order in find.OrderBy)
                {
                    var field = Visit(order.Expression) as FieldExpression;
                    if (field == null)
                        throw new InvalidQueryException("Complex order by clauses are not supported.");
                    _queryObject.AddOrderBy(field.Name, order.OrderType == OrderType.Ascending ? 1 : -1);
                }
            }

            if (find.Limit != null)
                _queryObject.NumberToLimit = EvaluateConstant<int>(find.Limit);

            if (find.Skip != null)
                _queryObject.NumberToSkip = EvaluateConstant<int>(find.Skip);

            return find;
        }

        protected override Expression VisitSource(Expression source)
        {
            switch ((MongoExpressionType)source.NodeType)
            {
                case MongoExpressionType.Collection:
                    var collection = (CollectionExpression)source;
                    _queryObject.CollectionName = collection.CollectionName;
                    _queryObject.Database = collection.Database;
                    _queryObject.DocumentType = collection.DocumentType;
                    break;
                case MongoExpressionType.Find:
                    Visit(source);
                    break;
                default:
                    throw new InvalidOperationException("Select source is not valid type");
            }
            return source;
        }

        private static T EvaluateConstant<T>(Expression e)
        {
            if (e.NodeType != ExpressionType.Constant)
                throw new ArgumentException("Expression must be a constant.");

            return (T)((ConstantExpression)e).Value;
        }

        private class QueryAttributes
        {
            public bool IsCount { get; private set; }
            public bool IsFreeForm { get; private set; }
            public bool IsMapReduce { get; private set; }

            public QueryAttributes(bool isCount, bool isFreeForm, bool isMapReduce)
            {
                IsCount = isCount;
                IsFreeForm = isFreeForm;
                IsMapReduce = isMapReduce;
            }
        }

        private class QueryAttributesGatherer : MongoExpressionVisitor
        {
            public bool _isCount { get; private set; }
            public bool _isFreeForm { get; private set; }
            public bool _isMapReduce { get; private set; }

            public QueryAttributes Gather(Expression expression)
            {
                _isCount = false;
                _isFreeForm = false;
                _isMapReduce = false;
                Visit(expression);
                return new QueryAttributes(_isCount, _isFreeForm, _isMapReduce);
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
}