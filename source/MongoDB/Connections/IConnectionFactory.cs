using System;

namespace MongoDB.Connections
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConnectionFactory : IDisposable
    {
        /// <summary>
        /// Opens a connection.
        /// </summary>
        /// <returns></returns>
        RawConnection Open();

        /// <summary>
        /// Closes the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        void Close(RawConnection connection);

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get; }

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        void Cleanup();
    }
}