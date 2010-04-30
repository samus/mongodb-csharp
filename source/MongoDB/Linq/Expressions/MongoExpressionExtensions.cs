using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDB.Linq.Expressions
{
    internal static class MongoExpressionExtensions
    {
        public static SelectExpression AddField(this SelectExpression select, FieldDeclaration field)
        {
            List<FieldDeclaration> fields = new List<FieldDeclaration>(select.Fields);
            fields.Add(field);
            return select.SetFields(fields);
        }

        public static ProjectionExpression AddOuterJoinTest(this ProjectionExpression projection)
        {
            string fieldName = projection.Source.GetAvailableFieldName("Test");
            SelectExpression newSource = projection.Source.AddField(new FieldDeclaration(fieldName, Expression.Constant(1, typeof(int?))));
            Expression newProjector =
                new OuterJoinedExpression(
                    new FieldExpression(Expression.Constant(1, typeof(int?)), newSource.Alias, fieldName),
                    projection.Projector);
            return new ProjectionExpression(newSource, newProjector, projection.Aggregator);
        }

        public static string GetAvailableFieldName(this SelectExpression select, string baseName)
        {
            string name = baseName;
            int n = 0;
            while (!IsUniqueName(select, name))
            {
                name = baseName + (n++);
            }
            return name;
        }

        public static SelectExpression RemoveField(this SelectExpression select, FieldDeclaration field)
        {
            List<FieldDeclaration> fields = new List<FieldDeclaration>(select.Fields);
            fields.Remove(field);
            return select.SetFields(fields);
        }

        public static SelectExpression SetFields(this SelectExpression select, IEnumerable<FieldDeclaration> fields)
        {
            return new SelectExpression(select.Alias, fields.OrderBy(f => f.Name), select.From, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take);
        }

        private static bool IsUniqueName(SelectExpression select, string name)
        {
            foreach (var field in select.Fields)
            {
                if (field.Name == name)
                    return false;
            }
            return true;
        }
    }
}