using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Configuration
{
    [Serializable]
    public class MongoConfigurationException : MongoException
    {
        public MongoConfigurationException(string message) : base(message) 
        { }

        public MongoConfigurationException(string message, Exception inner) 
            : base(message, inner) 
        { }
    }
}
