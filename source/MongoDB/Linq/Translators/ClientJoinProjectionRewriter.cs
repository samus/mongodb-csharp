using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Linq.Expressions;

namespace MongoDB.Linq.Translators
{
    internal class ClientJoinProjectionRewriter : MongoExpressionVisitor
    {
        private bool _isTopLevel;
        private SelectExpression _currentSelect;
        private FieldMapper _fieldMapper;

        public ClientJoinProjectionRewriter()
        {
            _fieldMapper = new FieldMapper();
        }

        public Expression Rewrite(Expression expression)
        {
            _isTopLevel = true;
            return Visit(expression);
        }

        protected override Expression VisitProjection(ProjectionExpression projection)
        {
            var saveCurrentSelect = _currentSelect;
            _currentSelect = projection.Source;
            try
            {
                if (!_isTopLevel)
                {
                    if (CanJoinOnClient(_currentSelect))
                    {
                        var newOuterSelect = (SelectExpression)new QueryDuplicator().Duplicate(saveCurrentSelect);
                        var newInnerSelect = (SelectExpression)_fieldMapper.Map(projection.Source, newOuterSelect.Alias, saveCurrentSelect.Alias);

                        var newInnerProjection = new ProjectionExpression(newInnerSelect, projection.Projector).AddOuterJoinTest();
                        newInnerSelect = newInnerProjection.Source;
                        var newProjector = newInnerProjection.Projector;

                        var newAlias = new Alias();
                        var fieldProjection = new FieldProjector(QueryBinder.CanBeField).ProjectFields(newProjector, newAlias, newOuterSelect.Alias, newInnerSelect.Alias);
                        var join = new JoinExpression(JoinType.OuterApply, newOuterSelect, newInnerSelect, null);
                        var joinedSelect = new SelectExpression(newAlias, fieldProjection.Fields, join, null, null, null, projection.IsSingleton, null, null);

                        _currentSelect = joinedSelect;
                        newProjector = Visit(fieldProjection.Projector);

                        var outerKeys = new List<Expression>();
                        var innerKeys = new List<Expression>();
                        var fieldMapper = new FieldMapper();
                        if (GetEquiJoinKeyExpressions(newInnerSelect.Where, newOuterSelect.Alias, outerKeys, innerKeys))
                        {
                            var outerKey = outerKeys.Select(k => _fieldMapper.Map(k, saveCurrentSelect.Alias, newOuterSelect.Alias));
                            var innerKey = innerKeys.Select(k => _fieldMapper.Map(k, joinedSelect.Alias, ((FieldExpression)k).Alias));
                            var newProjection = new ProjectionExpression(joinedSelect, newProjector, projection.Aggregator);
                            return new ClientJoinExpression(newProjection, outerKey, innerKey);
                        }
                    }
                }
                else
                    _isTopLevel = false;

                return base.VisitProjection(projection);
            }
            finally
            {
                _currentSelect = saveCurrentSelect;
            }
        }

        protected override Expression VisitSubquery(SubqueryExpression subquery)
        {
            return subquery;
        }

        private bool CanJoinOnClient(SelectExpression select)
        {
            return !select.IsDistinct
                && select.GroupBy != null
                && !new AggregateChecker().HasAggregates(select);
        }

        private bool GetEquiJoinKeyExpressions(Expression predicate, Alias outerAlias, List<Expression> outerExpressions, List<Expression> innerExpressions)
        {
            BinaryExpression b = predicate as BinaryExpression;
            if (b != null)
            {
                switch (predicate.NodeType)
                {
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        return GetEquiJoinKeyExpressions(b.Left, outerAlias, outerExpressions, innerExpressions)
                            && GetEquiJoinKeyExpressions(b.Right, outerAlias, outerExpressions, innerExpressions);
                    case ExpressionType.Equal:
                        var left = b.Left as FieldExpression;
                        var right = b.Right as FieldExpression;
                        if (left != null && right != null)
                        {
                            if (left.Alias == outerAlias)
                            {
                                outerExpressions.Add(left);
                                innerExpressions.Add(right);
                                return true;
                            }
                            else if (right.Alias == outerAlias)
                            {
                                innerExpressions.Add(left);
                                outerExpressions.Add(right);
                                return true;
                            }
                        }
                        break;
                }
            }
            return false;
        }
    }
}