using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using MongoDB.Linq.Expressions;

namespace MongoDB.Linq
{
    public class ExecutionBuilder : MongoExpressionVisitor
    {
        private List<Expression> _initializers;
        private bool _isTop;
        private int _numCursors;
        private Expression _provider;
        private MemberInfo _receivingMember;
        private List<ParameterExpression> _variables;


        public Expression Build(Expression expression, Expression provider)
        {
            _provider = provider;
            expression = Visit(expression);
            expression = AddVariables(expression);
            return expression;
        }




        private class Scope
        {

        }
    }
}