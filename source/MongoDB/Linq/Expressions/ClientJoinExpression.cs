using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDB.Linq.Expressions
{
    internal class ClientJoinExpression : MongoExpression
    {
        private readonly ReadOnlyCollection<Expression> _outerKey;
        private readonly ReadOnlyCollection<Expression> _innerKey;
        private readonly ProjectionExpression _projection;

        public ReadOnlyCollection<Expression> InnerKey
        {
            get { return _innerKey; }
        }

        public ReadOnlyCollection<Expression> OuterKey
        {
            get { return _outerKey; }
        }

        public ProjectionExpression Projection
        {
            get { return _projection; }
        }

        public ClientJoinExpression(ProjectionExpression projection, IEnumerable<Expression> outerKey, IEnumerable<Expression> innerKey)
            : base(MongoExpressionType.ClientJoin, projection.Type)
        {
            _outerKey = outerKey as ReadOnlyCollection<Expression>;
            if (_outerKey == null)
                _outerKey = new List<Expression>(outerKey).AsReadOnly();
            
            _innerKey = innerKey as ReadOnlyCollection<Expression>;
            if (_innerKey == null)
                _innerKey = new List<Expression>(innerKey).AsReadOnly();
            
            _projection = projection;
        }
    }
}