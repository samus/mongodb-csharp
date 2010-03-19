using System.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Connections;

namespace MongoDB.Linq.Tests {
    public static class AppSettingsFactory {
        public static Mongo CreateMongo() {
            var connectionString = ConfigurationManager.AppSettings["tests"];
            return new Mongo(connectionString);
        }

        public static Connection CreateConnection() {
            var connectionString = ConfigurationManager.AppSettings["tests"];
            return ConnectionFactory.GetConnection(connectionString);
        }
    }
}