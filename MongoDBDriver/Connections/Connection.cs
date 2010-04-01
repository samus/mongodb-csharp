using System;
using System.IO;
using MongoDB.Driver.Bson;
using MongoDB.Driver.CommandResults;
using MongoDB.Driver.Protocol;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver.Connections
{
    /// <summary>
    /// Connection is a managment unit which uses a RawConnection from connection pool
    /// to comunicate with the server.
    /// <remarks>
    /// If an connection error occure, the RawConnection is transparently replaced
    /// by a new fresh connection.
    /// </remarks>
    /// </summary>
    public class Connection : IDisposable
    {
        private readonly IConnectionFactory _factory;
        private RawConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="factory">The pool.</param>
        public Connection(IConnectionFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException ("factory");
            
            _factory = factory;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Connection"/> is reclaimed by garbage collection.
        /// </summary>
        ~Connection (){
            // make sure the connection returns to pool if the user forget it.
            Dispose ();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is authenticated.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
        /// </value>
        public bool IsAuthenticated {
            get { return _connection.IsAuthenticated; }
        }

        /// <summary>
        /// Masks as authenticated.
        /// </summary>
        public void MaskAuthenticated (){
            _connection.MarkAuthenticated ();
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString {
            get { return _factory.ConnectionString; }
        }

        /// <summary>
        /// Gets the end point.
        /// </summary>
        /// <value>The end point.</value>
        public MongoServerEndPoint EndPoint{
            get { return _connection.EndPoint; }
        }

         /// <summary>
        /// Sends the two way message.
        /// </summary>
        /// <param name="message">The MSG.</param>
        /// <returns></returns>
        public ReplyMessage<Document> SendTwoWayMessage(IRequestMessage message){
            return SendTwoWayMessage<Document>(message,new BsonDocumentBuilder());
        }

        /// <summary>
        /// Used for sending a message that gets a reply such as a query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <param name="objectBuilder">The object builder.</param>
        /// <returns></returns>
        /// <exception cref="IOException">A reconnect will be issued but it is up to the caller to handle the error.</exception>
        public ReplyMessage<T> SendTwoWayMessage<T>(IRequestMessage message, IBsonObjectBuilder objectBuilder) where T:class {
            if (State != ConnectionState.Opened) {
                throw new MongoConnectionException ("Operation cannot be performed on a closed connection.", this);
            }
            try {
                var reply = new ReplyMessage<T>(objectBuilder);
                lock (_connection) {
                    message.Write (_connection.GetStream ());
                    reply.Read (_connection.GetStream ());
                }
                return reply;
            } catch (IOException) {
                ReplaceInvalidConnection ();
                throw;
            }
            
        }

        /// <summary>
        /// Used for sending a message that gets no reply such as insert or update.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="IOException">A reconnect will be issued but it is up to the caller to handle the error.</exception>
        public void SendMessage (IRequestMessage message){
            if (State != ConnectionState.Opened) {
                throw new MongoConnectionException ("Operation cannot be performed on a closed connection.", this);
            }
            try {
                lock (_connection) {
                    message.Write (_connection.GetStream ());
                }
            } catch (IOException) {
                //Sending doesn't seem to always trigger the detection of a closed socket.
                ReplaceInvalidConnection ();
                throw;
            }
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        public ConnectionState State {
            get { return _connection != null ? ConnectionState.Opened : ConnectionState.Closed; }
        }

        /// <summary>
        /// Just sends a simple message string to the database. 
        /// </summary>
        /// <param name="message">
        /// A <see cref="System.String"/>
        /// </param>
        public void SendMsgMessage (String message){
            SendMessage(new MsgMessage{Message = message});
        }

        /// <summary>
        /// Opens this instance.
        /// </summary>
        public void Open (){
            _connection = _factory.Open();
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close (){
            if (_connection == null)
                return;

            _factory.Close(_connection);
            _connection = null;
        }

        /// <summary>
        /// Replaces the invalid connection.
        /// </summary>
        private void ReplaceInvalidConnection (){
            if (_connection == null)
                return;
            
            _connection.MarkAsInvalid ();
            _factory.Close (_connection);
            _connection = _factory.Open();
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <returns></returns>
        public Stream GetStream (){
            return _connection.GetStream ();
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="database">The database.</param>
        /// <param name="rootType">Type of the command.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public Document SendCommand(ISerializationFactory factory, string database, Type rootType, Document command)
        {
            var result = SendCommandCore<Document>(factory, database, rootType, command);

            if((double)result["ok"] != 1.0)
            {
                var msg = string.Empty;
                if(result.Contains("msg"))
                    msg = (string)result["msg"];
                else if(result.Contains("errmsg"))
                    msg = (string)result["errmsg"];
                throw new MongoCommandException(msg, result, command);
            }
            
            return result;
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="database">The database.</param>
        /// <param name="rootType">Type of serialization root.</param>
        /// <param name="command">The spec.</param>
        /// <returns></returns>
        public T SendCommand<T>(ISerializationFactory factory, string database, Type rootType, object command) 
            where T : CommandResultBase
        {
            var result = SendCommandCore<T>(factory, database, rootType, command);

            if(!result.Success)
                throw new MongoCommandException(result.ErrorMessage, null, null);

            return result;
        }


        /// <summary>
        /// Sends the command core.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="database">The database.</param>
        /// <param name="rootType">Type of serialization root.</param>
        /// <param name="command">The spec.</param>
        /// <returns></returns>
        private T SendCommandCore<T>(ISerializationFactory factory, string database, Type rootType, object command) 
            where T : class
        {
            var descriptor = factory.GetBsonDescriptor(rootType);

            var query = new QueryMessage(descriptor)
            {
                FullCollectionName = database + ".$cmd",
                NumberToReturn = -1,
                Query = command
            };

            var builder = factory.GetBsonBuilder(typeof(T));

            try
            {
                var reply = SendTwoWayMessage<T>(query, builder);

                SendMessage(new KillCursorsMessage(reply.CursorId));

                foreach(var document in reply.Documents)
                    return document;

                return null;
            }
            catch(IOException exception)
            {
                throw new MongoConnectionException("Could not read data, communication failure", this, exception);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose (){
            Close ();
        }
    }
}
