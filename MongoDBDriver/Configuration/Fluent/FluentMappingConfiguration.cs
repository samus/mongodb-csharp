using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Configuration.Mapping.Auto;

namespace MongoDB.Driver.Configuration.Fluent
{
    public class FluentMappingConfiguration
    {

        public FluentMappingConfiguration DefaultProfile(Action<FluentAutoMappingProfileConfiguration> config)
        {
            var profile = new AutoMappingProfile();
            var configuration = new FluentAutoMappingProfileConfiguration(profile);
            config(configuration);
            return this;
        }

    }
}