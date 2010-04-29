using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MongoDB.Linq.Expressions;
using System.Linq.Expressions;

namespace MongoDB.Linq
{
    internal class MapReduceMapFunctionBuilder : MongoExpressionVisitor
    {
        private JavascriptFormatter _formatter;
        private Dictionary<string, string> _initMap;
        private string _currentAggregateName;

        public MapReduceMapFunctionBuilder()
        {
            _formatter = new JavascriptFormatter();
        }

        public string Build(ReadOnlyCollection<FieldDeclaration> fields, Expression groupBy)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("function() { emit(");

            if (groupBy == null)
                sb.Append("1");
            else
                sb.Append(_formatter.FormatJavascript(groupBy));

            sb.Append(", ");

            _initMap = new Dictionary<string, string>();
            VisitFieldDeclarationList(fields);
            FormatInit(sb);

            sb.Append("); }");

            return sb.ToString();
        }

        protected override Expression VisitAggregate(AggregateExpression aggregate)
        {
            switch (aggregate.AggregateType)
            {
                case AggregateType.Count:
                    _initMap[_currentAggregateName] = "1";
                    break;
                case AggregateType.Max:
                case AggregateType.Min:
                case AggregateType.Sum:
                    _initMap[_currentAggregateName] = _formatter.FormatJavascript(aggregate.Argument);
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
            }

            return fields;
        }

        private void FormatInit(StringBuilder sb)
        {
            sb.Append("{");
            var isFirst = true;
            foreach (var field in _initMap)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(", ");

                sb.AppendFormat("\"{0}\": {1}", field.Key, field.Value);
            }
            sb.Append("}");
        }

    }
}