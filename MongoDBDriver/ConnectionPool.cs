using System;
using System.Collections.Generic;
using System.Threading;

namespace MongoDB.Driver
{
    /// <summary>
    ///   Connection pool implementation based on this document:
    ///   http://msdn.microsoft.com/en-us/library/8xx3tyca%28VS.100%29.aspx
    /// </summary>
    public class ConnectionPool : IDisposable
    {
        private readonly MongoConnectionStringBuilder _connectionStringBuilder;
        private readonly object _syncObject = new object();
        private readonly Queue<RawConnection> _freeConnections = new Queue<RawConnection>();
        private readonly List<RawConnection> _usedConnections = new List<RawConnection>();
        private readonly List<RawConnection> _invalidConnections = new List<RawConnection>();
        private int _endPointPointer;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ConnectionPool"/>
        /// class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ConnectionPool(string connectionString)
        {
            if(connectionString == null)
                throw new ArgumentNullException("connectionString");

            _connectionStringBuilder = new MongoConnectionStringBuilder(connectionString);

            if(_connectionStringBuilder.MaximumPoolSize < 1)
                throw new ArgumentException("MaximumPoolSize have to be greater or equal then 1");
            if(_connectionStringBuilder.MinimumPoolSize < 0)
                throw new ArgumentException("MinimumPoolSize have to be greater or equal then 0");
            if(_connectionStringBuilder.ConnectionLifetime.TotalSeconds < 0)
                throw new ArgumentException("ConnectionLifetime have to be greater or equal then 0");

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
        public void Cleanup()
        {
            CheckFreeConnectionsAlive();

            DisposeInvalidConnections();
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

            if(_connectionStringBuilder.ConnectionLifetime!=TimeSpan.Zero)
                if(connection.CreationTime.Add(_connectionStringBuilder.ConnectionLifetime) < DateTime.Now)
                    return false;

            if(!connection.IsConnected)
                return false;

            return !connection.IsInvalid;
        }

        /// <summary>
        ///   Borrows the connection.
        /// </summary>
        /// <returns></returns>
        public RawConnection BorrowConnection()
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

                if(PoolSize >= _connectionStringBuilder.MaximumPoolSize)
                {
                    Monitor.Wait(_syncObject);
                    return BorrowConnection();
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
        public void ReturnConnection(RawConnection connection)
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
                while(PoolSize < _connectionStringBuilder.MinimumPoolSize)
                    _freeConnections.Enqueue(CreateRawConnection());
        }

        /// <summary>
        /// Creates the raw connection.
        /// </summary>
        /// <returns></returns>
        private RawConnection CreateRawConnection()
        {
            var endPoint = GetNextEndPoint();
            return new RawConnection(endPoint);
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
            var servers = _connectionStringBuilder.Servers;
            var endPoint = servers[_endPointPointer];

            _endPointPointer++;

            if(_endPointPointer >= servers.Length)
                _endPointPointer = 0;

            return endPoint;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
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