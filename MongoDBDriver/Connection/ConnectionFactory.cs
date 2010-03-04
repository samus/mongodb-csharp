using System;
using System.Collections.Generic;
using System.Threading;

namespace MongoDB.Driver.Connection
{
    public static class ConnectionFactory
    {
        private static readonly TimeSpan MaintenaceWakeup = TimeSpan.FromSeconds(30);
        private static readonly Timer MaintenanceTimer = new Timer(o => OnMaintenaceWakeup());
        private static readonly Dictionary<string,ConnectionPool> Pools = new Dictionary<string, ConnectionPool>();
        private static readonly object SyncObject = new object();

        /// <summary>
        /// Initializes the <see cref="ConnectionFactory"/> class.
        /// </summary>
        static ConnectionFactory()
        {
            MaintenanceTimer.Change(MaintenaceWakeup, MaintenaceWakeup);
        }

        /// <summary>
        /// Gets the pool count.
        /// </summary>
        /// <value>The pool count.</value>
        public static int PoolCount
        {
            get
            {
                lock(SyncObject)
                    return Pools.Count;
            }
        }

        /// <summary>
        /// Called when [maintenace wakeup].
        /// </summary>
        private static void OnMaintenaceWakeup()
        {
            lock(SyncObject)
            {
                foreach(var pool in Pools.Values)
                    pool.Cleanup();
            }
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static Connection GetConnection(string connectionString)
        {
            if(connectionString == null)
                throw new ArgumentNullException("connectionString");

            ConnectionPool pool;
            
            lock(SyncObject)
            {
                if(!Pools.TryGetValue(connectionString, out pool))
                    Pools.Add(connectionString, pool = new ConnectionPool(connectionString));
            }

            return new Connection(pool);
        }
    }
}