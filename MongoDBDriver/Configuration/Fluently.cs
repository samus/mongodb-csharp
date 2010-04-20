using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Serialization;
using MongoDB.Driver.Configuration.Fluent;

namespace MongoDB.Driver.Configuration
{
    public static class Fluently
    {

        public static FluentConfiguration Configure()
        {
            return new FluentConfiguration();
        }

    }
}