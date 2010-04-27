using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace MongoDB.Connections
{
    /// <summary>
    /// Represents a raw connection on the wire which is managed by the 
    /// connection pool.
    /// </summary>
    public class RawConnection : IDisposable
    {
        private readonly TcpClient _client = new TcpClient();
        private readonly List<string> _authenticatedDatabases = new List<string>();
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawConnection"/> class.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="connectionTimeout">The connection timeout.</param>
        public RawConnection(MongoServerEndPoint endPoint,TimeSpan connectionTimeout)
        {
            if(endPoint == null)
                throw new ArgumentNullException("endPoint");

            EndPoint = endPoint;
            CreationTime = DateTime.UtcNow;
            
            _client.NoDelay = true;
            _client.ReceiveTimeout = (int)connectionTimeout.TotalMilliseconds;
            _client.SendTimeout = (int)connectionTimeout.TotalMilliseconds;

            //Todo: custom exception?
            _client.Connect(EndPoint.Host, EndPoint.Port);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="RawConnection"/> is reclaimed by garbage collection.
        /// </summary>
        ~RawConnection()
        {
            // make sure the connection is disposed when the user forget it.
            Dispose();    
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <returns></returns>
        public NetworkStream GetStream()
        {
            return _client.GetStream();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is invalid.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is invalid; otherwise, <c>false</c>.
        /// </value>
        public bool IsInvalid { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get { return _client.Client != null && _client.Connected; }
        }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        /// <value>The creation time.</value>
        public DateTime CreationTime { get; private set; }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>The end point.</value>
        public MongoServerEndPoint EndPoint { get; private set; }

        /// <summary>
        /// Determines whether the specified database name is authenticated.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <returns>
        /// 	<c>true</c> if the specified database name is authenticated; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAuthenticated(string databaseName){
            if(databaseName == null)
                throw new ArgumentNullException("databaseName");

            return _authenticatedDatabases.Contains(databaseName);
        }

        /// <summary>
        /// Marks as authenticated.
        /// </summary>
        public void MarkAuthenticated(string databaseName){
            if(databaseName == null)
                throw new ArgumentNullException("databaseName");

            _authenticatedDatabases.Add(databaseName);
        }

        /// <summary>
        /// Marks as invalid.
        /// </summary>
        public void MarkAsInvalid()
        {
            IsInvalid = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if(_isDisposed)
                return;

            _client.Close();
            
            _isDisposed = true;
        }
    }
}