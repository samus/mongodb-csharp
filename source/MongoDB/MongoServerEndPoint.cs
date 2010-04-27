using System;
using System.Globalization;
using System.Net;

namespace MongoDB
{
    /// <summary>
    /// Represents a mongodb server with host and port.
    /// </summary>
    [Serializable]
    public class MongoServerEndPoint : EndPoint
    {
        /// <summary>
        /// The mongo default host name.
        /// </summary>
        public const string DefaultHost = "localhost";
        /// <summary>
        /// The mongo default server port.
        /// </summary>
        public const int DefaultPort = 27017;

        /// <summary>
        /// The default MongoServerEndPoint.
        /// </summary>
        public static readonly MongoServerEndPoint Default = new MongoServerEndPoint();

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoServerEndPoint"/> class.
        /// </summary>
        public MongoServerEndPoint()
            : this(DefaultHost, DefaultPort)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoServerEndPoint"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        public MongoServerEndPoint(string host)
            : this(host, DefaultPort)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoServerEndPoint"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public MongoServerEndPoint(int port)
            : this(DefaultHost, port)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoServerEndPoint"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public MongoServerEndPoint(string host, int port)
        {
            if(host == null)
                throw new ArgumentNullException("host");

            Host = host;
            Port = port;
        }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host { get; private set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}:{1}", Host, Port);
        }
    }
}