using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDB.Driver.Linq
{
    internal class MongoQueryTranslator : ExpressionVisitor
    {
        private MongoQueryObject _queryObject;
        private Document _currentQuery;

        internal MongoQueryObject Translate(Expression e)
        {
            _currentQuery = new Document();
            _queryObject = new MongoQueryObject();
            Visit(e);
            return _queryObject;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            return base.VisitBinary(b);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType != typeof(Queryable) || m.Method.Name != "Where")
                throw new NotSupportedException(string.Format("The method {0} is not supported.", m.Method.Name));

            this.Visit(m.Arguments[0]);

            var oldQuery = _currentQuery;
            _currentQuery = new Document();
            var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
            this.Visit(lambda.Body);
            _currentQuery = oldQuery.Merge(_currentQuery);
            return m;
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    var oldQuery = _currentQuery;
                    _currentQuery = new Document();
                    Visit(u.Operand);
                    _currentQuery = oldQuery.Merge(new Document("$not", _currentQuery));
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator {0} is not supported.", u.NodeType));
            }

            return u;
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
                e = ((UnaryExpression)e).Operand;
            return e;
        }
    }
}