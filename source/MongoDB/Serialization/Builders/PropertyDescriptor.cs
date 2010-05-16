using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Serialization.Builders
{
    public class PropertyDescriptor
    {
        public Type Type { get; set; }

        public bool IsDictionary { get; set; }
    }
}
