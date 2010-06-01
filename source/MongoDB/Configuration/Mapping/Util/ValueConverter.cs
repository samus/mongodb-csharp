using System;

namespace MongoDB.Configuration.Mapping.Util
{
    internal static class ValueConverter
    {
        public static object Convert(object value, Type type)
        {
            var valueType = value != null ? value.GetType() : typeof(object);

            if(valueType != type)
                try
                {
                    var code = System.Convert.GetTypeCode(value);

                    if(type.IsEnum)
                        value = Enum.ToObject(type, value);
                    else if(type.IsGenericType &&
                            type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        if(value != null)
                            value = System.Convert.ChangeType(value, Nullable.GetUnderlyingType(type));
                    }
                    else if(code != TypeCode.Object)
                        value = System.Convert.ChangeType(value, type);
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
    }
}