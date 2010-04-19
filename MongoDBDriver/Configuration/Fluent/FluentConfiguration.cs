using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver.Configuration.Fluent
{
    public class FluentConfiguration
    {

        public FluentConfiguration Mappings(Action<FluentMappingConfiguration> config)
        {
            var mappings = new FluentMappingConfiguration();
            config(mappings);
            //TODO: do something with mappings...
            return this;
        }


        public ISerializationFactory Build()
        {
            return new SerializationFactory();
        }
    }
}