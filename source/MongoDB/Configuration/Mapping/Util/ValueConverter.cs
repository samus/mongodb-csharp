using System;

namespace MongoDB.Configuration.Mapping.Util
{
    internal static class ValueConverter
    {
        public static object Convert(object value, Type type)
        {
            if(value == null)
                return type.IsValueType ? Activator.CreateInstance(type) : null;

            var valueType = value.GetType();

            if(valueType != type)
                try
                {
                    var code = System.Convert.GetTypeCode(value);

                    if(type.IsEnum)
                        if(value is string)
                            value = Enum.Parse(type, (string)value);
                        else
                            value = Enum.ToObject(type, value);
                    else if(type.IsGenericType &&
                            type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        value = System.Convert.ChangeType(value, Nullable.GetUnderlyingType(type));
                    else if(code != TypeCode.Object)
                        value = System.Convert.ChangeType(value, type);
                    else if(valueType==typeof(Binary)&&type==typeof(byte[]))
                        value = (byte[])(Binary)value;
                }
                catch(FormatException exception)
                {
                    throw new MongoException("Can not convert value from " + valueType + " to " + type, exception);
                }
                catch(ArgumentException exception)
                {
                    throw new MongoException("Can not convert value from " + valueType + " to " + type, exception);
                }

            return value;
        }

        public static Array ConvertArray(object[] elements, Type type)
        {
            if(elements == null)
                return null;

            var array = Array.CreateInstance(type, elements.Length);

            for(var i = 0; i < elements.Length; i++)
                array.SetValue(Convert(elements[i], type), i);

            return array;
        }

        public static string ConvertKey(object key)
        {
            if(key == null)
                throw new ArgumentNullException("key");

            if(key is Enum)
                return System.Convert.ToInt64(key).ToString();

            return key.ToString();
        }
    }
}