using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDB.Linq.Expressions
{
    internal class JoinExpression : MongoExpression
    {
        private readonly JoinType _joinType;
        private readonly Expression _left;
        private readonly Expression _right;
        private readonly Expression _condition;

        public JoinType Join
        {
            get { return _joinType; }
        }

        public Expression Left
        {
            get { return _left; }
        }

        public Expression Right
        {
            get { return _right; }
        }

        public new Expression Condition
        {
            get { return _condition; }
        }

        public JoinExpression(JoinType joinType, Expression left, Expression right, Expression condition)
            : base(MongoExpressionType.Join, typeof(void))
        {
            _joinType = joinType;
            _left = left;
            _right = right;
            _condition = condition;
        }
        
    }
}
