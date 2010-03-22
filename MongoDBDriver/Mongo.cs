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
        private readonly Connection connection;
        private readonly ISerializationFactory serializationFactory;

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
            this.connection = ConnectionFactory.GetConnection(connectionString);
            this.serializationFactory = serializationFactory;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString {
            get { return connection.ConnectionString; }
        }

        /// <summary>
        /// Gets the named database.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Database GetDatabase (String name){
            return new Database (serializationFactory, connection, name);
        }

        /// <summary>
        /// Gets the <see cref="MongoDatabase"/> with the specified name.
        /// </summary>
        /// <value></value>
        public Database this[String name] {
            get { return this.GetDatabase (name); }
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns></returns>
        public Boolean Connect (){
            connection.Open ();
            return connection.State == ConnectionState.Opened;
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        /// <returns></returns>
        public Boolean Disconnect (){
            connection.Close ();
            return connection.State == ConnectionState.Closed;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose (){
            connection.Dispose ();
        }
    }
}
