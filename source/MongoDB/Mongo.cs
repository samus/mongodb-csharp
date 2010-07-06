using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Configuration;
using MongoDB.Connections;

namespace MongoDB
{
    /// <summary>
    ///   Description of Mongo.
    /// </summary>
    public class Mongo : IDisposable, IMongo
    {
        private readonly MongoConfiguration _configuration;
        private readonly Connection _connection;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Mongo" /> class.
        /// </summary>
        public Mongo()
            : this(new MongoConfiguration())
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Mongo" /> class.
        /// </summary>
        /// <param name = "connectionString">The connection string.</param>
        public Mongo(string connectionString)
            : this(new MongoConfiguration {ConnectionString = connectionString})
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Mongo" /> class.
        /// </summary>
        /// <param name = "configuration">The mongo configuration.</param>
        public Mongo(MongoConfiguration configuration){
            if(configuration == null)
                throw new ArgumentNullException("configuration");

            configuration.ValidateAndSeal();

            _configuration = configuration;
            _connection = ConnectionFactoryFactory.GetConnection(configuration.ConnectionString);
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _connection.Dispose();
        }

        /// <summary>
        ///   Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get { return _connection.ConnectionString; }
        }

        /// <summary>
        ///   Gets the named database.
        /// </summary>
        /// <param name = "name">The name.</param>
        /// <returns></returns>
        public IMongoDatabase GetDatabase(String name)
        {
            return new MongoDatabase(_configuration, _connection, name);
        }

        /// <summary>
        ///   Gets the <see cref = "MongoDatabase" /> with the specified name.
        /// </summary>
        /// <value></value>
        public IMongoDatabase this[String name]
        {
            get { return GetDatabase(name); }
        }

        /// <summary>
        ///   Connects to server.
        /// </summary>
        /// <returns></returns>
        /// <exception cref = "MongoDB.MongoConnectionException">Thrown when connection fails.</exception>
        public void Connect()
        {
            _connection.Open();
        }

        /// <summary>
        ///   Tries to connect to server.
        /// </summary>
        /// <returns></returns>
        public bool TryConnect()
        {
            try
            {
                _connection.Open();
                return _connection.IsConnected;
            }
            catch(MongoException)
            {
                return _connection.IsConnected;
            }
        }

        /// <summary>
        ///   Disconnects this instance.
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            _connection.Close();
            return _connection.IsConnected;
        }

        /// <summary>
        ///   Gets the databases.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMongoDatabase> GetDatabases()
        {
            var result = _connection.SendCommand(_configuration.SerializationFactory, "admin", typeof(Document), new Document("listDatabases", 1));

            return ((IEnumerable<Document>)result["databases"])
                .Select(database => (string)database["name"])
                .Select(name => new MongoDatabase(_configuration, _connection, name))
                .Cast<IMongoDatabase>();
        }
    }
}
