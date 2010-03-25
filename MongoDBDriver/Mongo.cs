using System;
using MongoDB.Driver.Connections;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver
{
    /// <summary>
    /// Description of Mongo.
    /// </summary>
    public class Mongo : IDisposable
    {
        private readonly Connection _connection;
        private readonly ISerializationFactory _serializationFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mongo"/> class.
        /// </summary>
        public Mongo ()
            : this(string.Empty, SerializationFactory.Default)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mongo"/> class.
        /// </summary>
        /// <param name="serializationFactory">The serialization factory.</param>
        public Mongo(ISerializationFactory serializationFactory)
            : this(string.Empty, serializationFactory)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mongo"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public Mongo(string connectionString)
            : this(connectionString, SerializationFactory.Default)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mongo"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="serializationFactory">The serialization factory.</param>
        public Mongo (string connectionString, ISerializationFactory serializationFactory)
        {
            if(connectionString == null)
                throw new ArgumentNullException("connectionString");
            if(serializationFactory == null)
                throw new ArgumentNullException("serializationFactory");
            
            _connection = ConnectionFactory.GetConnection(connectionString);
            _serializationFactory = serializationFactory;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString {
            get { return _connection.ConnectionString; }
        }

        /// <summary>
        /// Gets the named database.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public MongoDatabase GetDatabase (String name){
            return new MongoDatabase (_serializationFactory, _connection, name);
        }

        /// <summary>
        /// Gets the <see cref="MongoDatabase"/> with the specified name.
        /// </summary>
        /// <value></value>
        public MongoDatabase this[String name] {
            get { return GetDatabase (name); }
        }

        /// <summary>
        /// Connects to server.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MongoDB.Driver.MongoConnectionException">Thrown when connection fails.</exception>
        public void Connect()
        {
            _connection.Open();
        }

        /// <summary>
        /// Tries to connect to server.
        /// </summary>
        /// <returns></returns>
        public bool TryConnect (){
            try
            {
                _connection.Open();
                return _connection.State == ConnectionState.Opened;
            }
            catch(MongoException)
            {
                return _connection.State == ConnectionState.Opened;
            }
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        /// <returns></returns>
        public bool Disconnect (){
            _connection.Close ();
            return _connection.State == ConnectionState.Closed;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose (){
            _connection.Dispose ();
        }
    }
}
