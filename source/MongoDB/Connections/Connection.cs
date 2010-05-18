using System;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Configuration;
using MongoDB.Protocol;
using MongoDB.Results;
using MongoDB.Serialization;
using MongoDB.Util;

namespace MongoDB.Connections
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
        private bool _disposed;

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
            Dispose (false);
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
            return SendTwoWayMessage<Document>(message,new BsonReaderSettings());
        }

        /// <summary>
        /// Used for sending a message that gets a reply such as a query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <param name="readerSettings">The reader settings.</param>
        /// <returns></returns>
        /// <exception cref="IOException">A reconnect will be issued but it is up to the caller to handle the error.</exception>
        public ReplyMessage<T> SendTwoWayMessage<T>(IRequestMessage message, BsonReaderSettings readerSettings) where T:class {
            if(!IsConnected)
                throw new MongoConnectionException ("Operation cannot be performed on a closed connection.", this);
            
            try {
                var reply = new ReplyMessage<T>(readerSettings);
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
            if(!IsConnected)
                throw new MongoConnectionException("Operation cannot be performed on a closed connection.", this);
            
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
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get { return _connection != null && _connection.IsConnected; }
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
        internal Stream GetStream (){
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
            AuthenticateIfRequired(database);

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
            AuthenticateIfRequired(database);

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
            var writerSettings = factory.GetBsonWriterSettings(rootType);

            var query = new QueryMessage(writerSettings)
            {
                FullCollectionName = database + ".$cmd",
                NumberToReturn = -1,
                Query = command
            };

            var readerSettings = factory.GetBsonReaderSettings(typeof(T));

            try
            {
                var reply = SendTwoWayMessage<T>(query, readerSettings);

                if(reply.CursorId > 0)
                    SendMessage(new KillCursorsMessage(reply.CursorId));

                return reply.Documents.FirstOrDefault();
            }
            catch(IOException exception)
            {
                throw new MongoConnectionException("Could not read data, communication failure", this, exception);
            }
        }

        /// <summary>
        /// Authenticates the on first request.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        private void AuthenticateIfRequired(string databaseName)
        {
            if(databaseName == null)
                throw new ArgumentNullException("databaseName");

            if(_connection.IsAuthenticated(databaseName))
                return;

            var builder = new MongoConnectionStringBuilder(ConnectionString);

            if(string.IsNullOrEmpty(builder.Username))
                return;

            var serializationFactory = MongoConfiguration.Default.SerializationFactory;

            var document = new Document().Add("getnonce", 1.0);
            var nonceResult = SendCommandCore<Document>(serializationFactory, databaseName, typeof(Document), document);
            var nonce = (string)nonceResult["nonce"];

            if(nonce == null)
                throw new MongoException("Error retrieving nonce", null);

            var pwd = MongoHash.Generate(builder.Username + ":mongo:" + builder.Password);
            var auth = new Document{
                {"authenticate", 1.0},
                {"user", builder.Username},
                {"nonce", nonce},
                {"key", MongoHash.Generate(nonce + builder.Username + pwd)}
            };
            try
            {
                SendCommandCore<Document>(serializationFactory, databaseName, typeof(Document), auth);
            }
            catch(MongoCommandException exception)
            {
                //Todo: use custom exception?
                throw new MongoException("Authentication faild for " + builder.Username, exception);
            }

            _connection.MarkAuthenticated(databaseName);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose (){
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if(_disposed)
                return;
            
            if (disposing)
            {
                // Cleanup Managed Resources Here
                Close();
            }

            // Cleanup Unmanaged Resources Here

            // Then mark object as disposed
            _disposed = true;
        }
    }
}
