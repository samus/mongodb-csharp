using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Linq.Expressions;
using System.Collections.ObjectModel;

namespace MongoDB.Linq
{
    internal class ProjectionBuilder : MongoExpressionVisitor
    {
        private MongoQueryObject _queryObject;
        private ParameterExpression _document;

        public LambdaExpression Build(MongoQueryObject queryObject, Expression projector)
        {
            _queryObject = queryObject;
            if (_queryObject.IsMapReduce)
            {
                _document = Expression.Parameter(typeof(Document), "document");
            }
            else
                _document = Expression.Parameter(queryObject.DocumentType, "document");

            var body = Visit(projector);
            return Expression.Lambda(body, _document);
        }

        protected override Expression VisitField(FieldExpression field)
        {
            if (_queryObject.IsMapReduce)
            {
                var value = Expression.Call(
                    _document,
                    "Get",
                    new [] { typeof(Document) },
                    Expression.Constant("value"));

                return Expression.Convert(
                    Expression.Call(
                        value,
                        "Get",
                        new [] { field.Type },
                        Expression.Constant(field.Name)),
                    field.Type);
            }
            else
                return Visit(field.Expression);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (p.Type == _document.Type)
                return _document;

            return p;
        }

        //private class AggregateNameMapGatherer : MongoExpressionVisitor
        //{
        //    private Dictionary<string, string> _map;

        //    public Dictionary<string, string> Gather(Expression expression)
        //    {
        //        _map = new Dictionary<string, string>();
        //        Visit(expression);
        //        return _map;
        //    }

        //    protected override Expression VisitAggregate(AggregateExpression aggregate)
        //    {
        //        return base.VisitAggregate(aggregate);
        //    }

        //    protected override NewExpression VisitNew(NewExpression nex)
        //    {

        //        return base.VisitNew(nex);
        //    }

        //    protected override ReadOnlyCollection<FieldDeclaration> VisitFieldDeclarationList(ReadOnlyCollection<FieldDeclaration> fields)
        //    {
        //        for (int i = 0, n = fields.Count; i < n; i++)
        //        {
        //            Visit(fields[i].Expression);
        //        }

        //        return fields;
        //    }
        //}
    }
}
