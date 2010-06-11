using System;
using System.Globalization;
using System.Net;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MongoDB
{
    /// <summary>
    /// Represents a mongodb server with host and port.
    /// </summary>
    [Serializable]
    public sealed class MongoServerEndPoint : EndPoint, IEquatable<MongoServerEndPoint>, IXmlSerializable
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

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(MongoServerEndPoint other)
        {
            if(ReferenceEquals(null, other))
                return false;
            if(ReferenceEquals(this, other))
                return true;
            return Equals(other.Host, Host) && other.Port == Port;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
                return false;
            if(ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == typeof(MongoServerEndPoint) && Equals((MongoServerEndPoint)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Host != null ? Host.GetHashCode() : 0)*397) ^ Port;
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(MongoServerEndPoint left, MongoServerEndPoint right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(MongoServerEndPoint left, MongoServerEndPoint right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if(reader.MoveToAttribute("host"))
                Host = reader.Value;
            if(reader.MoveToAttribute("port"))
                Port = int.Parse(reader.Value);
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if(Host!=null)
                writer.WriteAttributeString("host",Host);

            writer.WriteAttributeString("port",Port.ToString());
        }
    }
}