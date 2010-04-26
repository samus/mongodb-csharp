using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver.Configuration
{
    public interface IMongoConfiguration
    {
        /// <summary>
        /// Builds the connection string.
        /// </summary>
        /// <returns></returns>
        string BuildConnectionString();

        /// <summary>
        /// Builds the serialization factory.
        /// </summary>
        /// <returns></returns>
        ISerializationFactory BuildSerializationFactory();
    }
}