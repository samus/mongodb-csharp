using System;

namespace MongoDB.Connections
{
    /// <summary>
    /// Simple connection factory which only creates and closes connections.
    /// </summary>
    public class SimpleConnectionFactory : ConnectionFactoryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SimpleConnectionFactory(string connectionString)
            : base(connectionString){
        }

        /// <summary>
        /// Opens a connection.
        /// </summary>
        /// <returns></returns>
        public override RawConnection Open(){
            return CreateRawConnection();
        }

        /// <summary>
        /// Closes the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public override void Close(RawConnection connection){
            if(connection == null)
                throw new ArgumentNullException("connection");
            
            connection.Dispose();
        }
    }
}