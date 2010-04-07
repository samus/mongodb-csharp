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

        internal MongoQueryObject Translate(Expression e)
        {
            _queryObject = new MongoQueryObject();
            Visit(e);
            return _queryObject;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (b.NodeType == ExpressionType.And || b.NodeType == ExpressionType.AndAlso)
            {
                Visit(b.Left);
                Visit(b.Right);
                return b;
            }

            var left = b.Left;
            var right = b.Right;
            if (left.NodeType != ExpressionType.MemberAccess && right.NodeType != ExpressionType.MemberAccess)
                throw new InvalidQueryException();
            else if (left.NodeType == ExpressionType.MemberAccess && right.NodeType == ExpressionType.MemberAccess)
                throw new InvalidQueryException();
           
            if (right.NodeType == ExpressionType.MemberAccess)
            {
                left = b.Right;
                right = b.Left;
                //reverse the order so that the member access is on the left side...
            }

            if (right.NodeType != ExpressionType.Constant)
                throw new InvalidQueryException();

            var memberPath = EvaluateMemberAccess((MemberExpression)left);
            _queryObject.PushConditionScope(memberPath);
            switch (b.NodeType)
            {
                case ExpressionType.Equal:
                    _queryObject.AddCondition(EvaluateConstant((ConstantExpression)right));
                    _queryObject.PopConditionScope();
                    break;
                case ExpressionType.GreaterThan:
                    _queryObject.AddCondition(Op.GreaterThan(EvaluateConstant((ConstantExpression)right)));
                    _queryObject.PopConditionScope();
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _queryObject.AddCondition(Op.GreaterThanOrEqual(EvaluateConstant((ConstantExpression)right)));
                    _queryObject.PopConditionScope();
                    break;
                case ExpressionType.LessThan:
                    _queryObject.AddCondition(Op.LessThan(EvaluateConstant((ConstantExpression)right)));
                    _queryObject.PopConditionScope();
                    break;
                case ExpressionType.LessThanOrEqual:
                    _queryObject.AddCondition(Op.LessThanOrEqual(EvaluateConstant((ConstantExpression)right)));
                    _queryObject.PopConditionScope();
                    break;
                case ExpressionType.NotEqual:
                    _queryObject.AddCondition(Op.NotEqual(EvaluateConstant((ConstantExpression)right)));
                    _queryObject.PopConditionScope();
                    break;
            }

            return b;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable))
            {
                if (m.Method.Name == "Where")
                {
                    this.Visit(m.Arguments[0]);
                    var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    this.Visit(lambda.Body);
                    return m;
                }
                else if (m.Method.Name == "Select")
                {
                    var lamda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    var parameter = Expression.Parameter(lamda.Parameters[0].Type, "document");
                    _queryObject.Projection = new MongoFieldProjector().ProjectFields(lamda.Body, parameter);
                    Visit(m.Arguments[0]);
                    return m;
                }
            }

            throw new NotSupportedException(string.Format("The method {0} is not supported.", m.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    _queryObject.PushConditionScope("$not");
                    Visit(u.Operand);
                    _queryObject.PopConditionScope();
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator {0} is not supported.", u.NodeType));
            }

            return u;
        }

        private static object EvaluateConstant(ConstantExpression c)
        {
            return c.Value;
        }

        private static string EvaluateMemberAccess(MemberExpression m)
        {
            var sb = new StringBuilder();
            var p = m;
            while (p.Expression != null && p.Expression.NodeType == ExpressionType.MemberAccess)
            {
                sb.AppendFormat("{0}.", p.Member.Name);
                p = (MemberExpression)p.Expression;
            }

            if (p.Expression != null && p.Expression.NodeType == ExpressionType.Parameter)
                return sb.Append(p.Member.Name).ToString();

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
                e = ((UnaryExpression)e).Operand;
            return e;
        }
    }
}