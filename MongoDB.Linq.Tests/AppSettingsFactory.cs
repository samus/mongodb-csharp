using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using MongoDB.Driver;

namespace MongoDB.Linq.Tests {
    public static class AppSettingsFactory {

        public static string Host { get { return ConfigurationManager.AppSettings["mongo.host"]; } }
        public static int Port { get { return int.Parse(ConfigurationManager.AppSettings["mongo.port"]); } }

        public static Mongo CreateMongo() {
            return new Mongo(Host, Port);
        }

        public static Connection CreateConnection() {
            return new Connection(Host, Port);
        }
    }
}