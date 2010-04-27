using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using MongoDB.Driver.Util;

namespace MongoDB.Driver.Configuration.Mapping.Model
{
    /// <summary>
    /// 
    /// </summary>
    public static class MemberReflectionOptimizer
    {
        private static readonly Dictionary<string, Func<object, object>> getterCache = new Dictionary<string, Func<object, object>>();
        private static readonly Dictionary<string, Action<object, object>> setterCache = new Dictionary<string, Action<object, object>>();

        /// <summary>
        /// Gets the getter.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns></returns>
        public static Func<object, object> GetGetter(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                throw new ArgumentNullException("memberInfo");
            if (memberInfo.MemberType != MemberTypes.Field && memberInfo.MemberType != MemberTypes.Property)
                throw new ArgumentException("Only fields and properties are supported.", "memberInfo");

            if (memberInfo.MemberType == MemberTypes.Field)
                return GetFieldGetter(memberInfo as FieldInfo);
            
            if (memberInfo.MemberType == MemberTypes.Property)
                return GetPropertyGetter(memberInfo as PropertyInfo);

            throw new InvalidOperationException("Can only create getters for fields or properties.");
        }

        /// <summary>
        /// Gets the field getter.
        /// </summary>
        /// <param name="fieldInfo">The field info.</param>
        /// <returns></returns>
        public static Func<object, object> GetFieldGetter(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("fieldInfo");

            var key = CreateKey(fieldInfo);
            if (getterCache.ContainsKey(key))
                return getterCache[key];

            ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");

            MemberExpression member = Expression.Field(Expression.Convert(instanceParameter, fieldInfo.DeclaringType), fieldInfo);

            Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(
                    Expression.Convert(member, typeof(object)),
                    instanceParameter);

            var result = lambda.Compile();
            getterCache[key] = result;
            return result;
        }

        /// <summary>
        /// Gets the property getter.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public static Func<object, object> GetPropertyGetter(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            var key = CreateKey(propertyInfo);
            if (getterCache.ContainsKey(key))
                return getterCache[key];

            if (!propertyInfo.CanRead)
                throw new InvalidOperationException("Cannot create a getter for a writeonly property.");

            ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");

            MemberExpression member = Expression.Property(Expression.Convert(instanceParameter, propertyInfo.DeclaringType), propertyInfo);

            Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(
                    Expression.Convert(member, typeof(object)),
                    instanceParameter);

            var result = lambda.Compile();
            getterCache[key] = result;
            return result;
        }

        /// <summary>
        /// Gets the setter.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns></returns>
        public static Action<object, object> GetSetter(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                throw new ArgumentNullException("memberInfo");
            if (memberInfo.MemberType != MemberTypes.Field && memberInfo.MemberType != MemberTypes.Property)
                throw new ArgumentException("Only fields and properties are supported.", "memberInfo");

            if (memberInfo.MemberType == MemberTypes.Field)
                return GetFieldSetter(memberInfo as FieldInfo);
            
            if (memberInfo.MemberType == MemberTypes.Property)
                return GetPropertySetter(memberInfo as PropertyInfo);

            throw new InvalidOperationException("Can only create setters for fields or properties.");
        }

        /// <summary>
        /// Gets the field setter.
        /// </summary>
        /// <param name="fieldInfo">The field info.</param>
        /// <returns></returns>
        public static Action<object, object> GetFieldSetter(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("fieldInfo");

            var key = CreateKey(fieldInfo);
            if (setterCache.ContainsKey(key))
                return setterCache[key];

            if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral)
                throw new InvalidOperationException("Cannot create a setter for a readonly field.");

            var sourceType = fieldInfo.DeclaringType;
            var method = new DynamicMethod("Set" + fieldInfo.Name, null, new[] { typeof(object), typeof(object) }, true);
            var gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Castclass, sourceType);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
            gen.Emit(OpCodes.Stfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            var result = (Action<object, object>)method.CreateDelegate(typeof(Action<object, object>));
            setterCache[key] = result;
            return result;
        }

        /// <summary>
        /// Gets the property setter.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public static Action<object, object> GetPropertySetter(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            var key = CreateKey(propertyInfo);
            if (setterCache.ContainsKey(key))
                return setterCache[key];

            if (!propertyInfo.CanWrite)
                throw new InvalidOperationException("Cannot create a setter for a readonly property.");

            var method = new DynamicMethod("Set" + propertyInfo.Name, null, new[] { typeof(object), typeof(object) }, true);
            var gen = method.GetILGenerator();

            var sourceType = propertyInfo.DeclaringType;
            var setter = propertyInfo.GetSetMethod(true);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Castclass, sourceType);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
            gen.Emit(OpCodes.Callvirt, setter);
            gen.Emit(OpCodes.Ret);

            var result = (Action<object, object>)method.CreateDelegate(typeof(Action<object, object>));
            setterCache[key] = result;
            return result;
        }

        /// <summary>
        /// Creates the key.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
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