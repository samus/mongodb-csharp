using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    public class DefaultDefaultValueConvention : IDefaultValueConvention
    {
        public static readonly DefaultDefaultValueConvention Instance = new DefaultDefaultValueConvention();

        private DefaultDefaultValueConvention()
        { }

        public object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }
    }
}
