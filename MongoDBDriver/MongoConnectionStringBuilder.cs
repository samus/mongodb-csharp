using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MongoDB.Driver
{
    [Serializable]
    public class MongoConnectionStringBuilder
    {
        public const int DefaultMaximumPoolSize = 100;
        public const int DefaultMinimumPoolSize = 0;
        public static readonly TimeSpan DefaultConnectionLifeTime = TimeSpan.Zero;

        private static readonly Regex PairRegex = new Regex(@"^\s*(.*)\s*=\s*(.*)\s*$");
        private static readonly Regex ServerRegex = new Regex(@"\s*([^:]+)(?::(\d+))?\s*$");

        private readonly List<MongoServerEndPoint> _servers = new List<MongoServerEndPoint>();

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref = "MongoConnectionStringBuilder" />
        ///   class. Uses the default server connection when
        ///   no server is added.
        /// </summary>
        public MongoConnectionStringBuilder()
        {
            ConnectionLifetime = DefaultConnectionLifeTime;
            MaximumPoolSize = DefaultMaximumPoolSize;
            MinimumPoolSize = DefaultMinimumPoolSize;
        }

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref = "MongoConnectionStringBuilder" />
        ///   class. Uses the default server connection when
        ///   no server is added.
        /// </summary>
        /// <param name = "connectionString">The connection string.</param>
        public MongoConnectionStringBuilder(string connectionString)
            : this()
        {

            if(!string.IsNullOrEmpty(connectionString))
                Parse(connectionString);
        }

        /// <summary>
        /// Gets the servers.
        /// </summary>
        /// <value>The servers.</value>
        public MongoServerEndPoint[] Servers
        {
            get{return _servers.Count == 0 ? new[] {MongoServerEndPoint.Default} : _servers.ToArray();}
        }

        /// <summary>
        ///   Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        ///   Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }


        /// <summary>
        /// Gets or sets the maximum size of the connection pool.
        /// </summary>
        /// <value>The maximum size of the pool.</value>
        public int MaximumPoolSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the minimum connection pool.
        /// </summary>
        /// <value>The size of the minimal pool.</value>
        public int MinimumPoolSize { get; set; }

        /// <summary>
        /// Gets or sets the connection lifetime in connection pool.
        /// </summary>
        /// <value>The connection lifetime.</value>
        public TimeSpan ConnectionLifetime { get; set; }

        /// <summary>
        ///   Parses the specified connection string.
        /// </summary>
        /// <param name = "connectionString">The connection string.</param>
        private void Parse(string connectionString)
        {
            if(connectionString == null)
                throw new ArgumentNullException("connectionString");

            var segments = connectionString.Split(';');

            foreach(var segment in segments)
            {
                var pairMatch = PairRegex.Match(segment);
                if(!pairMatch.Success)
                    throw new FormatException(string.Format("Invalid connection string on: {0}", pairMatch.Value));

                var key = pairMatch.Groups[1].Value;
                var value = pairMatch.Groups[2].Value;

                switch(key)
                {
                    case "Username":
                    case "User Id":
                    case "User":
                    {
                        Username = value;
                        break;
                    }
                    case "Passwort":
                    {
                        Password = value;
                        break;
                    }
                    case "MaximumPoolSize":
                    case "Max Pool Size":
                    {
                        try
                        {
                            MaximumPoolSize = int.Parse(value);
                        }
                        catch(FormatException exception)
                        {
                            throw new FormatException("Invalid number for MaximumPoolSize in connection string", exception);
                        }
                        break;
                    }
                    case "MinimumPoolSize":
                    case "Min Pool Size":
                    {
                        try
                        {
                            MinimumPoolSize = int.Parse(value);
                        }
                        catch(FormatException exception)
                        {
                            throw new FormatException("Invalid number for MinimumPoolSize in connection string", exception);
                        }
                        break;
                    }
                    case "ConnectionLifetime":
                    case "Connection Lifetime":
                    {
                        try
                        {
                            var seconds = int.Parse(value);

                            ConnectionLifetime = seconds>0 ? TimeSpan.FromSeconds(seconds) : DefaultConnectionLifeTime;
                        }
                        catch(FormatException exception)
                        {
                            throw new FormatException("Invalid number for ConnectionLifetime in connection string", exception);
                        }
                        break;
                    }
                    case "Server":
                    case "Servers":
                    {
                        var servers = value.Split(',');

                        foreach(var server in servers)
                        {
                            var serverMatch = ServerRegex.Match(server);
                            if(!serverMatch.Success)
                                throw new FormatException(string.Format("Invalid server in connection string: {0}", serverMatch.Value));

                            var serverHost = serverMatch.Groups[1].Value;

                            int port;
                            if(int.TryParse(serverMatch.Groups[2].Value,out port))
                                AddServer(serverHost,port);
                            else
                                AddServer(serverHost);
                        }

                        break;
                    }
                    default:
                        throw new FormatException(string.Format("Unknown connection string option: {0}", key));
                }
            }
        }

        /// <summary>
        /// Adds the server.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        public void AddServer(MongoServerEndPoint endPoint)
        {
            if(endPoint == null)
                throw new ArgumentNullException("endPoint");

            _servers.Add(endPoint);
        }

        /// <summary>
        /// Clears the servers.
        /// </summary>
        public void ClearServers()
        {
            _servers.Clear();
        }

        /// <summary>
        /// Adds the server with the given host and default port.
        /// </summary>
        /// <param name="host">The host.</param>
        public void AddServer(string host)
        {
            AddServer(new MongoServerEndPoint(host));
        }

        /// <summary>
        /// Adds the server with the given host and port.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public void AddServer(string host, int port)
        {
            AddServer(new MongoServerEndPoint(host,port));
        }

        /// <summary>
        ///   Returns a
        ///   <see cref = "System.String" />
        ///   that represents this instance.
        /// </summary>
        /// <returns>A
        ///   <see cref = "System.String" />
        ///   that represents this instance.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            if(!string.IsNullOrEmpty(Username))
            {
                builder.AppendFormat("Username={0}", Username);
                builder.Append(';');
            }
            
            if(!string.IsNullOrEmpty(Password))
            {
                builder.AppendFormat("Passwort={0}", Password);
                builder.Append(';');
            }

            if(_servers.Count>0)
            {
                builder.Append("Server=");

                foreach(var server in _servers)
                {
                    builder.Append(server.Host);

                    if(server.Port != MongoServerEndPoint.DefaultPort)
                        builder.AppendFormat(":{0}", server.Port);

                    builder.Append(',');
                }

                // remove last ,
                builder.Remove(builder.Length - 1, 1);

                builder.Append(';');
            }

            if(MaximumPoolSize != DefaultMaximumPoolSize)
            {
                builder.AppendFormat("MaximumPoolSize={0}", MaximumPoolSize);
                builder.Append(';');
            }

            if(MinimumPoolSize != DefaultMinimumPoolSize)
            {
                builder.AppendFormat("MinimumPoolSize={0}", MinimumPoolSize);
                builder.Append(';');
            }

            if(ConnectionLifetime != DefaultConnectionLifeTime)
            {
                builder.AppendFormat("ConnectionLifetime={0}", Convert.ToInt32(ConnectionLifetime.TotalSeconds));
                builder.Append(';');
            }

            // remove last ;
            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }
    }
}