using System;
using System.Collections.Generic;
using System.Threading;

namespace MongoDB.Driver
{
    internal static class ConnectionFactory
    {
        private static readonly TimeSpan MaintenaceWakeup = TimeSpan.FromSeconds(30);
        private static readonly Timer MaintenanceTimer;

        static ConnectionFactory()
        {
            MaintenanceTimer = new Timer(o => OnMaintenaceWakeup(), null, MaintenaceWakeup, MaintenaceWakeup);

        }

        private static void OnMaintenaceWakeup()
        {
            
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

            var builder = new MongoConnectionStringBuilder(connectionString);

            var servers = new List<MongoServerEndPoint>(builder.Servers);
            if(servers.Count == 1)
            {
                var server = servers[0];
                return new Connection(server.Host, server.Port);
            }

            if(servers.Count == 2)
            {
                var leftServer = servers[0];
                var rightServer = servers[1];
                return new PairedConnection(leftServer.Host, leftServer.Port, rightServer.Host, rightServer.Port);
            }
            
            throw new InvalidOperationException("Currently are only two servers supported.");
        }
    }
}