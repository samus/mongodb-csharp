using System;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace MongoDB.Linq.Tests {
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestExpressions {
        [Test]
        public void Getting_at_member_expression_values() {
            var foo = new { Bar = "abc" };
            Expression<Func<string>> expression = () => foo.Bar;
            var memberExpression = expression.Body as MemberExpression;
            switch (memberExpression.Member.MemberType) {
                case MemberTypes.Property:
                    var propertyInfo = (PropertyInfo)memberExpression.Member;
                    var innerMember = (MemberExpression)memberExpression.Expression; //ConstantExpression in Mono
                    var fieldInfo = (FieldInfo)innerMember.Member;
                    var obj = fieldInfo.GetValue(((ConstantExpression)innerMember.Expression).Value);
                    Assert.AreEqual("abc", propertyInfo.GetValue(obj, null));
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }

        [Test]
        public void Getting_field_from_closure() {
            string key = "xyz";
            Expression<Func<string>> expression = () => key;
            var memberExpression = expression.Body as MemberExpression;
            switch (memberExpression.Member.MemberType) {
                case MemberTypes.Field:
                    var fieldInfo = (FieldInfo)memberExpression.Member;
                    Assert.AreEqual("xyz", fieldInfo.GetValue(((ConstantExpression)memberExpression.Expression).Value));
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }

        [Test]
        public void Evaluating_a_MethodCallExpression_with_known_return_type() {
            Expression<Func<DateTime>> expression = () => DateTime.Parse("2009/10/10");
            var methodCallExpression = expression.Body as MethodCallExpression;
            Expression<Func<DateTime>> lambda = Expression.Lambda<Func<DateTime>>(methodCallExpression);
            var value = lambda.Compile()();
            Assert.AreEqual(DateTime.Parse("2009/10/10"), value);
        }

        [Test]
        public void Evaluating_a_MethodCallExpression_with_unknown_return_type() {
            Expression<Func<DateTime>> expression = () => DateTime.Parse("2009/10/10");
            var methodCallExpression = expression.Body as MethodCallExpression;
            var lambda = Expression.Lambda(methodCallExpression);
            var value = lambda.Compile().DynamicInvoke();
            Assert.AreEqual(DateTime.Parse("2009/10/10"), value);
        }
    }
    // ReSharper restore InconsistentNaming
}
