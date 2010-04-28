using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MongoDB.Linq.Expressions;
using System.Linq.Expressions;

namespace MongoDB.Linq
{
    internal class MapReduceReduceFunctionBuilder : MongoExpressionVisitor
    {
        private JavascriptFormatter _formatter;
        private StringBuilder _declare;
        private StringBuilder _loop;
        private StringBuilder _return;
        private string _currentAggregateName;

        public MapReduceReduceFunctionBuilder()
        {
            _formatter = new JavascriptFormatter();
        }

        public string Build(ReadOnlyCollection<FieldDeclaration> fields)
        {
            _declare = new StringBuilder();
            _loop = new StringBuilder();
            _return = new StringBuilder();
            _declare.Append("function(key, values) {");
            _loop.Append("values.forEach(function(doc) {");
            _return.Append("return { ");

            VisitFieldDeclarationList(fields);

            _loop.Append("});");
            _return.Append("};}");

            return _declare.ToString() + _loop.ToString() + _return.ToString();
        }

        protected override Expression VisitAggregate(AggregateExpression aggregate)
        {
            switch (aggregate.AggregateType)
            {
                case AggregateType.Average:
                    throw new NotImplementedException();
                case AggregateType.Count:
                    CountAggregate(aggregate);
                    break;
                case AggregateType.Max:
                    MaxAggregate(aggregate);
                    break;
                case AggregateType.Min:
                    MinAggregate(aggregate);
                    break;
                case AggregateType.Sum:
                    SumAggregate(aggregate);
                    break;
            }

            return aggregate;
        }

        protected override ReadOnlyCollection<FieldDeclaration> VisitFieldDeclarationList(ReadOnlyCollection<FieldDeclaration> fields)
        {
            for (int i = 0, n = fields.Count; i < n; i++)
            {
                _currentAggregateName = fields[i].Name;
                Visit(fields[i].Expression);

                if (i > 0)
                    _return.Append(", ");
                _return.AppendFormat("\"{0}\": {0}", fields[i].Name);
            }

            return fields;
        }

        private void CountAggregate(AggregateExpression aggregate)
        {
            _declare.AppendFormat("var {0} = 0;", _currentAggregateName);
            _loop.AppendFormat("{0} += doc.{0};", _currentAggregateName);
        }

        private void MaxAggregate(AggregateExpression aggregate)
        {
            _declare.AppendFormat("var {0} = Number.MIN_VALUE;", _currentAggregateName);
            _loop.AppendFormat("if(doc.{0} > {0}) {0} = doc.{0};", _currentAggregateName);
        }

        private void MinAggregate(AggregateExpression aggregate)
        {
            _declare.AppendFormat("var {0} = Number.MAX_VALUE;", _currentAggregateName);
            _loop.AppendFormat("if(doc.{0} < {0}) {0} = doc.{0};", _currentAggregateName);
        }

        private void SumAggregate(AggregateExpression aggregate)
        {
            _declare.AppendFormat("var {0} = 0;", _currentAggregateName);
            _loop.AppendFormat("{0} += doc.{0};", _currentAggregateName);
        }

    }
}