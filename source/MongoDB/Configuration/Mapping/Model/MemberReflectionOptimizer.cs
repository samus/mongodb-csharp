using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using MongoDB.Util;

namespace MongoDB.Configuration.Mapping.Model
{
    /// <summary>
    /// </summary>
    public static class MemberReflectionOptimizer
    {
        private static readonly Dictionary<string, Func<object, object>> GetterCache = new Dictionary<string, Func<object, object>>();
        private static readonly Dictionary<string, Action<object, object>> SetterCache = new Dictionary<string, Action<object, object>>();

        /// <summary>
        ///   Gets the getter.
        /// </summary>
        /// <param name = "memberInfo">The member info.</param>
        /// <returns></returns>
        public static Func<object, object> GetGetter(MemberInfo memberInfo)
        {
            if(memberInfo == null)
                throw new ArgumentNullException("memberInfo");
            if(memberInfo.MemberType != MemberTypes.Field && memberInfo.MemberType != MemberTypes.Property)
                throw new ArgumentException("Only fields and properties are supported.", "memberInfo");

            if(memberInfo.MemberType == MemberTypes.Field)
                return GetFieldGetter(memberInfo as FieldInfo);

            if(memberInfo.MemberType == MemberTypes.Property)
                return GetPropertyGetter(memberInfo as PropertyInfo);

            throw new InvalidOperationException("Can only create getters for fields or properties.");
        }

        /// <summary>
        ///   Gets the field getter.
        /// </summary>
        /// <param name = "fieldInfo">The field info.</param>
        /// <returns></returns>
        public static Func<object, object> GetFieldGetter(FieldInfo fieldInfo)
        {
            if(fieldInfo == null)
                throw new ArgumentNullException("fieldInfo");

            var key = CreateKey(fieldInfo);
            if(GetterCache.ContainsKey(key))
                return GetterCache[key];

            var instanceParameter = Expression.Parameter(typeof(object), "target");

            var member = Expression.Field(Expression.Convert(instanceParameter, fieldInfo.DeclaringType), fieldInfo);

            var lambda = Expression.Lambda<Func<object, object>>(
                Expression.Convert(member, typeof(object)),
                instanceParameter);

            var result = lambda.Compile();
            GetterCache[key] = result;
            return result;
        }

        /// <summary>
        ///   Gets the property getter.
        /// </summary>
        /// <param name = "propertyInfo">The property info.</param>
        /// <returns></returns>
        public static Func<object, object> GetPropertyGetter(PropertyInfo propertyInfo)
        {
            if(propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            var key = CreateKey(propertyInfo);
            if(GetterCache.ContainsKey(key))
                return GetterCache[key];

            if(!propertyInfo.CanRead)
                throw new InvalidOperationException("Cannot create a getter for a writeonly property.");

            var instanceParameter = Expression.Parameter(typeof(object), "target");

            var member = Expression.Property(Expression.Convert(instanceParameter, propertyInfo.DeclaringType), propertyInfo);

            var lambda = Expression.Lambda<Func<object, object>>(
                Expression.Convert(member, typeof(object)),
                instanceParameter);

            var result = lambda.Compile();
            GetterCache[key] = result;
            return result;
        }

        /// <summary>
        ///   Gets the setter.
        /// </summary>
        /// <param name = "memberInfo">The member info.</param>
        /// <returns></returns>
        public static Action<object, object> GetSetter(MemberInfo memberInfo)
        {
            if(memberInfo == null)
                throw new ArgumentNullException("memberInfo");
            if(memberInfo.MemberType != MemberTypes.Field && memberInfo.MemberType != MemberTypes.Property)
                throw new ArgumentException("Only fields and properties are supported.", "memberInfo");

            if(memberInfo.MemberType == MemberTypes.Field)
                return GetFieldSetter(memberInfo as FieldInfo);

            if(memberInfo.MemberType == MemberTypes.Property)
                return GetPropertySetter(memberInfo as PropertyInfo);

            throw new InvalidOperationException("Can only create setters for fields or properties.");
        }

        /// <summary>
        ///   Gets the field setter.
        /// </summary>
        /// <param name = "fieldInfo">The field info.</param>
        /// <returns></returns>
        public static Action<object, object> GetFieldSetter(FieldInfo fieldInfo)
        {
            if(fieldInfo == null)
                throw new ArgumentNullException("fieldInfo");

            var key = CreateKey(fieldInfo);
            if(SetterCache.ContainsKey(key))
                return SetterCache[key];

            if(fieldInfo.IsInitOnly || fieldInfo.IsLiteral)
                throw new InvalidOperationException("Cannot create a setter for a readonly field.");

            var sourceType = fieldInfo.DeclaringType;
            var method = new DynamicMethod("Set" + fieldInfo.Name, null, new[] {typeof(object), typeof(object)}, true);
            var gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Castclass, sourceType);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
            gen.Emit(OpCodes.Stfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            var result = (Action<object, object>)method.CreateDelegate(typeof(Action<object, object>));
            SetterCache[key] = result;
            return result;
        }

        /// <summary>
        ///   Gets the property setter.
        /// </summary>
        /// <param name = "propertyInfo">The property info.</param>
        /// <returns></returns>
        public static Action<object, object> GetPropertySetter(PropertyInfo propertyInfo)
        {
            if(propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            var key = CreateKey(propertyInfo);
            if(SetterCache.ContainsKey(key))
                return SetterCache[key];

            if(!propertyInfo.CanWrite)
                throw new InvalidOperationException("Cannot create a setter for a readonly property.");

            var instanceParameter = Expression.Parameter(typeof(object), "target");
            var valueParameter = Expression.Parameter(typeof(object), "value");

            var lambda = Expression.Lambda<Action<object, object>>(
                Expression.Call(
                    Expression.Convert(instanceParameter, propertyInfo.DeclaringType),
                    propertyInfo.GetSetMethod(true),
                    Expression.Convert(valueParameter, propertyInfo.PropertyType)),
                instanceParameter,
                valueParameter);

            var result = lambda.Compile();
            SetterCache[key] = result;
            return result;
        }

        /// <summary>
        ///   Creates the key.
        /// </summary>
        /// <param name = "memberInfo">The member info.</param>
        /// <returns></returns>
        private static string CreateKey(MemberInfo memberInfo)
        {
            return string.Format("{0}_{1}_{2}_{3}",
                memberInfo.DeclaringType.FullName,
                memberInfo.MemberType,
                memberInfo.GetReturnType(),
                memberInfo.Name);
        }
    }
}