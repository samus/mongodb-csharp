using System;
using System.Collections.Generic;
using System.Threading;

namespace MongoDB.Connections
{
    /// <summary>
    ///   Connection pool implementation based on this document:
    ///   http://msdn.microsoft.com/en-us/library/8xx3tyca%28VS.100%29.aspx
    /// </summary>
    internal class PooledConnectionFactory : ConnectionFactoryBase
    {
        private readonly object _syncObject = new object();
        private readonly Queue<RawConnection> _freeConnections = new Queue<RawConnection>();
        private readonly List<RawConnection> _usedConnections = new List<RawConnection>();
        private readonly List<RawConnection> _invalidConnections = new List<RawConnection>();

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="PooledConnectionFactory"/>
        /// class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public PooledConnectionFactory(string connectionString)
            :base(connectionString)
        {
            if(Builder.MaximumPoolSize < 1){
                throw new ArgumentException("MaximumPoolSize have to be greater or equal then 1");
            }
            if(Builder.MinimumPoolSize < 0){
                throw new ArgumentException("MinimumPoolSize have to be greater or equal then 0");
            }
            if(Builder.MinimumPoolSize > Builder.MaximumPoolSize){
                throw new ArgumentException("MinimumPoolSize must be smaller than MaximumPoolSize");
            }
            if(Builder.ConnectionLifetime.TotalSeconds < 0){
                throw new ArgumentException("ConnectionLifetime have to be greater or equal then 0");
            }

            EnsureMinimalPoolSize();
        }

        /// <summary>
        /// Gets the size of the pool.
        /// </summary>
        /// <value>The size of the pool.</value>
        public int PoolSize
        {
            get
            {
                lock(_syncObject)
                    return _freeConnections.Count + _usedConnections.Count;
            }
        }

        /// <summary>
        /// Cleanups the connections.
        /// </summary>
        public override void Cleanup()
        {
            CheckFreeConnectionsAlive();

            DisposeInvalidConnections();

            EnsureMinimalPoolSize();
        }

        /// <summary>
        /// Checks the free connections alive.
        /// </summary>
        private void CheckFreeConnectionsAlive()
        {
            lock(_syncObject)
            {
                var freeConnections = _freeConnections.ToArray();
                _freeConnections.Clear();

                foreach(var freeConnection in freeConnections)
                    if(IsAlive(freeConnection))
                        _freeConnections.Enqueue(freeConnection);
                    else
                        _invalidConnections.Add(freeConnection);
            }
        }

        /// <summary>
        /// Disposes the invalid connections.
        /// </summary>
        private void DisposeInvalidConnections()
        {
            RawConnection[] invalidConnections;

            lock(_syncObject)
            {
                invalidConnections = _invalidConnections.ToArray();
                _invalidConnections.Clear();
            }

            foreach(var invalidConnection in invalidConnections)
                invalidConnection.Dispose();
        }

        /// <summary>
        /// Determines whether the specified connection is alive.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>
        /// 	<c>true</c> if the specified connection is alive; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAlive(RawConnection connection)
        {
            if(connection == null)
                throw new ArgumentNullException("connection");

            if(!connection.IsConnected)
                return false;

            if(connection.IsInvalid)
                return false;

            if(Builder.ConnectionLifetime != TimeSpan.Zero)
                if(connection.CreationTime.Add(Builder.ConnectionLifetime) < DateTime.Now)
                    return false;

            return true;
        }

        /// <summary>
        ///   Borrows the connection.
        /// </summary>
        /// <returns></returns>
        public override RawConnection Open()
        {
            RawConnection connection;

            lock(_syncObject)
            {
                if(_freeConnections.Count > 0)
                {
                    connection = _freeConnections.Dequeue();
                    _usedConnections.Add(connection);
                    return connection;
                }

                if(PoolSize >= Builder.MaximumPoolSize)
                {
                    if(!Monitor.Wait(_syncObject, Builder.ConnectionTimeout))
                        //Todo: custom exception?
                        throw new MongoException("Timeout expired. The timeout period elapsed prior to obtaining a connection from pool. This may have occured because all pooled connections were in use and max poolsize was reached.");

                    return Open();
                }
            }

            connection = CreateRawConnection();

            lock(_syncObject)
                _usedConnections.Add(connection);

            return connection;
        }

        /// <summary>
        ///   Returns the connection.
        /// </summary>
        /// <param name = "connection">The connection.</param>
        public override void Close(RawConnection connection)
        {
            if(connection == null)
                throw new ArgumentNullException("connection");

            if(!IsAlive(connection))
            {
                lock(_syncObject)
                {
                    _usedConnections.Remove(connection);
                    _invalidConnections.Add(connection);
                }

                return;
            }

            lock(_syncObject)
            {
                _usedConnections.Remove(connection);
                _freeConnections.Enqueue(connection);
                Monitor.Pulse(_syncObject);
            }
        }

        /// <summary>
        /// Ensures the size of the minimal pool.
        /// </summary>
        private void EnsureMinimalPoolSize()
        {
            lock(_syncObject)
                while(PoolSize < Builder.MinimumPoolSize)
                    _freeConnections.Enqueue(CreateRawConnection());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            lock(_syncObject)
            {
                foreach(var usedConnection in _usedConnections)
                    usedConnection.Dispose();
                
                foreach(var freeConnection in _freeConnections)
                    freeConnection.Dispose();

                foreach(var invalidConnection in _invalidConnections)
                    invalidConnection.Dispose();

                _usedConnections.Clear();
                _freeConnections.Clear();
                _invalidConnections.Clear();
            }
        }
    }
}