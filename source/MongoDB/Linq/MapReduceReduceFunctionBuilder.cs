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
        private enum Mode { Declare, Loop, Return };
        private JavascriptFormatter _formatter;
        private Mode _mode;
        private StringBuilder _builder;
        private string _currentAggregateName;

        public MapReduceReduceFunctionBuilder()
        {
            _formatter = new JavascriptFormatter();
        }

        public string Build(ReadOnlyCollection<FieldDeclaration> fields)
        {
            _builder = new StringBuilder();
            _builder.Append("function(key, values) {");

            _mode = Mode.Declare;
            VisitFieldDeclarationList(fields);
            
            _builder.Append("values.forEach(function(doc) {");
            _mode = Mode.Loop;
            VisitFieldDeclarationList(fields);
            _builder.Append("});");

            _builder.Append("return { ");
            _mode = Mode.Return;
            VisitFieldDeclarationList(fields);
            _builder.Append("};}");

            return _builder.ToString();
        }

        protected override Expression VisitAggregate(AggregateExpression aggregate)
        {
            switch (aggregate.AggregateType)
            {
                case AggregateType.Average:
                    throw new NotImplementedException();
                case AggregateType.Count:
                    SumAggregate(aggregate);
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
                if (_mode != Mode.Return)
                {
                    _currentAggregateName = fields[i].Name;
                    Visit(fields[i].Expression);
                }
                else
                {
                    if (i > 0)
                        _builder.Append(", ");
                    _builder.AppendFormat("\"{0}\": {0}", fields[i].Name);
                }
            }

            return fields;
        }

        private void CountAggregate(AggregateExpression aggregate)
        {
            switch (_mode)
            {
                case Mode.Declare:
                    _builder.AppendFormat("var {0} = 0;", _currentAggregateName);
                    break;
                case Mode.Loop:
                    _builder.AppendFormat("{0}++;", _currentAggregateName);
                    break;
            }
        }

        private void MaxAggregate(AggregateExpression aggregate)
        {
            switch (_mode)
            {
                case Mode.Declare:
                    _builder.AppendFormat("var {0} = Number.MIN_VALUE;", _currentAggregateName);
                    break;
                case Mode.Loop:
                    _builder.AppendFormat("if(doc.{0} > {0}) {0} = doc.{0};", _currentAggregateName);
                    break;
            }
        }

        private void MinAggregate(AggregateExpression aggregate)
        {
            switch (_mode)
            {
                case Mode.Declare:
                    _builder.AppendFormat("var {0} = Number.MAX_VALUE;", _currentAggregateName);
                    break;
                case Mode.Loop:
                    _builder.AppendFormat("if(doc.{0} < {0}) {0} = doc.{0};", _currentAggregateName);
                    break;
            }
        }

        private void SumAggregate(AggregateExpression aggregate)
        {
            switch (_mode)
            {
                case Mode.Declare:
                    _builder.AppendFormat("var sum{0} = 0;", _currentAggregateName);
                    break;
                case Mode.Loop:
                    _builder.AppendFormat("sum{0} += doc.{0};", _currentAggregateName);
                    break;
            }
        }

    }
}