using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Serialization
{
    public class DocumentObjectDescriptorAdapter : IObjectDescriptor
    {
        public object GenerateId(object instance)
        {
            return Oid.NewOid();
        }

        public object GetPropertyValue(object instance, string mongoName)
        {
            return ((Document)instance)[mongoName];
        }

        public void SetPropertyValue(object instance, string mongoName, object value)
        {
            ((Document)instance)[mongoName] = value;
        }

        public IEnumerable<string> GetMongoPropertyNames()
        {
            return Enumerable.Empty<string>();
        }
    }
}