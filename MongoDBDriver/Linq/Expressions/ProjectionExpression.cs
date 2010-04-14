using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MongoDB.Driver.Linq.Expressions
{
    internal class ProjectionExpression : Expression
    {
        private readonly SelectExpression _source;
        private readonly Expression _projector;

        public SelectExpression Source
        {
            get { return _source; }
        }

        public Expression Projector
        {
            get { return _projector; }
        }

        public ProjectionExpression(SelectExpression source, Expression projector)
            : base((ExpressionType)MongoExpressionType.Projection, source.Type)
        {
            _source = source;
            _projector = projector;
        }
    }
}
