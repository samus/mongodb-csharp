using System;
using System.Collections.Generic;
using System.Threading;

namespace MongoDB.Connections
{
    /// <summary>
    /// 
    /// </summary>
    internal static class ConnectionFactory
    {
        private static readonly TimeSpan MaintenaceWakeup = TimeSpan.FromSeconds(10);
        private static readonly Timer MaintenanceTimer = new Timer(o => OnMaintenaceWakeup());
        private static readonly Dictionary<string, IConnectionFactory> Factorys = new Dictionary<string, IConnectionFactory>();
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
                    return Factorys.Count;
            }
        }

        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        public static void Shutdown()
        {
            lock(SyncObject)
            {
                foreach(var pool in Factorys.Values)
                    pool.Dispose();
                
                Factorys.Clear();
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

            IConnectionFactory pool;
            
            lock(SyncObject)
            {
                if(!Factorys.TryGetValue(connectionString, out pool))
                    Factorys.Add(connectionString, pool = CreateFactory(connectionString));
            }

            return new Connection(pool);
        }

        /// <summary>
        /// Creates the factory.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        private static IConnectionFactory CreateFactory(string connectionString){
            var builder = new MongoConnectionStringBuilder(connectionString);
            
            if(builder.Pooled)
                return new PooledConnectionFactory(connectionString);
            
            return new SimpleConnectionFactory(connectionString);
        }

        /// <summary>
        /// Called when [maintenace wakeup].
        /// </summary>
        private static void OnMaintenaceWakeup()
        {
            lock(SyncObject)
            {
                foreach(var pool in Factorys.Values)
                    pool.Cleanup();
            }
        }
    }
}