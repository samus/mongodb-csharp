using System;

namespace MongoDB.Driver.Connections
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ConnectionFactoryBase : IConnectionFactory
    {
        private int _endPointPointer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFactoryBase"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        protected ConnectionFactoryBase(string connectionString){
            if(connectionString == null)
                throw new ArgumentNullException("connectionString");

            ConnectionString = connectionString;
            Builder = new MongoConnectionStringBuilder(connectionString);
        }

        /// <summary>
        /// Gets or sets the builder.
        /// </summary>
        /// <value>The builder.</value>
        protected MongoConnectionStringBuilder Builder { get; private set; }

        /// <summary>
        /// Opens a connection.
        /// </summary>
        /// <returns></returns>
        public abstract RawConnection Open();

        /// <summary>
        /// Closes the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public abstract void Close(RawConnection connection);

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        public virtual void Cleanup(){
        }

        /// <summary>
        /// Creates the raw connection.
        /// </summary>
        /// <returns></returns>
        protected RawConnection CreateRawConnection()
        {
            var endPoint = GetNextEndPoint();
            return new RawConnection(endPoint, Builder.ConnectionTimeout);
        }

        /// <summary>
        ///   Gets the next end point.
        /// </summary>
        /// <remarks>
        ///   Currently is only cyles to the server list.
        /// </remarks>
        /// <returns></returns>
        private MongoServerEndPoint GetNextEndPoint()
        {
            var servers = Builder.Servers;
            var endPoint = servers[_endPointPointer];

            _endPointPointer++;

            if(_endPointPointer >= servers.Length)
                _endPointPointer = 0;

            return endPoint;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose(){
        }
    }
}