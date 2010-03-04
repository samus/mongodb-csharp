/*
 * User: scorder
 * Date: 7/7/2009
 */
using System;

namespace MongoDB.Driver
{
    /// <summary>
    /// Description of Mongo.
    /// </summary>
    public class Mongo : IDisposable
    {
        private Connection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mongo"/> class.
        /// </summary>
        public Mongo():this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mongo"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public Mongo(string connectionString)
        {
            if(connectionString == null)
                throw new ArgumentNullException("connectionString");

            connection = ConnectionFactory.GetConnection(connectionString);
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get { return connection.ConnectionString; }
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Database GetDatabase(String name){
            return new Database(connection, name);
        }

        /// <summary>
        /// Gets the <see cref="MongoDB.Driver.Database"/> with the specified name.
        /// </summary>
        /// <value></value>
        public Database this[ String name ]  {
            get{
                return this.GetDatabase(name);
            }
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns></returns>
        public Boolean Connect(){
            connection.Open();
            return connection.State == ConnectionState.Opened;
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        /// <returns></returns>
        public Boolean Disconnect(){
            connection.Close();
            return connection.State == ConnectionState.Closed;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose (){
            Dispose();
        }
    }
}
