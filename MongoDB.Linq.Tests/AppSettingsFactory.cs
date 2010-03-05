using System.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Connections;

namespace MongoDB.Linq.Tests {
    public static class AppSettingsFactory {

        public static string Host { get { return ConfigurationManager.AppSettings["mongo.host"]; } }
        public static int Port { get { return int.Parse(ConfigurationManager.AppSettings["mongo.port"]); } }

        public static Mongo CreateMongo() {
            var builder = new MongoConnectionStringBuilder();
            builder.AddServer(Host,Port);
            return new Mongo(builder.ToString());
        }

        public static Connection CreateConnection() {
            var builder = new MongoConnectionStringBuilder();
            builder.AddServer(Host, Port);
            return ConnectionFactory.GetConnection(builder.ToString());
        }
    }
}