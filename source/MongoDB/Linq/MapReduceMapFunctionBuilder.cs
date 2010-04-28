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
        private Dictionary<string, string> _map;
        private string _currentAggregateName;

        public MapReduceMapFunctionBuilder()
        {
            _formatter = new JavascriptFormatter();
        }

        public string Build(ReadOnlyCollection<FieldDeclaration> fields, ReadOnlyCollection<Expression> groupBys)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("function() { emit(");

            _map = new Dictionary<string, string>();
            VisitExpressionList(groupBys);
            FormatString(sb, true);

            sb.Append(", ");

            _map = new Dictionary<string, string>();
            VisitFieldDeclarationList(fields);
            FormatString(sb, false);

            sb.Append("); }");

            return sb.ToString();
        }

        protected override Expression VisitField(FieldExpression field)
        {
            var fieldJs = _formatter.FormatJavascript(field);
            if (string.IsNullOrEmpty(_currentAggregateName))
                _map[field.Name] = fieldJs;
            else
                _map[_currentAggregateName] = fieldJs;
            return field;
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

        private void FormatString(StringBuilder sb, bool isGroup)
        {
            if (_map.Count == 0)
                sb.Append("1");
            else if (_map.Count == 1 && isGroup)
            {
                sb.Append(_map.Single().Value);
            }
            else
            {
                sb.Append("{");
                var isFirst = true;
                foreach (var field in _map)
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
}