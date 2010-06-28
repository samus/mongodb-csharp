using System;
namespace MongoDB
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMongo : IDisposable
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get; }

        /// <summary>
        /// Gets the named database.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IMongoDatabase GetDatabase(string name);

        /// <summary>
        /// Gets the <see cref="MongoDB.IMongoDatabase"></see> with the specified name.
        /// </summary>
        /// <value></value>
        IMongoDatabase this[string name] { get; }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns></returns>
        void Connect();
        
        /// <summary>
        /// Tries to connect to server.
        /// </summary>
        /// <returns></returns>
        bool TryConnect();

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        /// <returns></returns>
        bool Disconnect();
    }
}
