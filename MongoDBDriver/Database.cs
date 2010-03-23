using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using MongoDB.Driver.CommandResults;
using MongoDB.Driver.Connections;

namespace MongoDB.Driver
{
    public class Database : IMongoDatabase
    {
        private readonly Connection _connection;
        private DatabaseJavascript _javascript;
        private DatabaseMetaData _metaData;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDatabase"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="name">The name.</param>
        public Database(string connectionString, String name)
            :this(ConnectionFactory.GetConnection(connectionString),name)
        {
            if(name == null)
                throw new ArgumentNullException("name");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDatabase"/> class.
        /// </summary>
        /// <param name="connection">The conn.</param>
        /// <param name="name">The name.</param>
        public Database(Connection connection, String name){
            //Todo: should be internal
            Name = name;
            _connection = connection;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the meta data.
        /// </summary>
        /// <value>The meta data.</value>
        public DatabaseMetaData MetaData{
            get { return _metaData ?? (_metaData = new DatabaseMetaData(Name, _connection)); }
        }

        /// <summary>
        /// Gets the javascript.
        /// </summary>
        /// <value>The javascript.</value>
        public DatabaseJavascript Javascript{
            get { return _javascript ?? (_javascript = new DatabaseJavascript(this)); }
        }

        /// <summary>
        /// Gets the <see cref="MongoDB.Driver.IMongoCollection&lt;MongoDB.Driver.Document&gt;"/> with the specified name.
        /// </summary>
        /// <value></value>
        public IMongoCollection this[String name]{
            get { return GetCollection(name); }
        }

        /// <summary>
        /// Gets the collection names.
        /// </summary>
        /// <returns></returns>
        public List<String> GetCollectionNames(){
            var namespaces = this["system.namespaces"];
            var cursor = namespaces.Find(new Document());
            var names = new List<string>();
            foreach(var document in cursor.Documents)
                names.Add((String)document["name"]); //Todo: Should filter built-ins
            return names;
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IMongoCollection GetCollection(String name){
            return new Collection(_connection, Name, name);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IMongoCollection<T> GetCollection<T>(String name) where T : class{
            return new Collection<T>(_connection, Name, name);
        }

        /// <summary>
        /// Gets the document that a reference is pointing to.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        public Document FollowReference(DBRef reference){
            if(reference == null)
                throw new ArgumentNullException("reference", "cannot be null");
            var query = new Document().Add("_id", reference.Id);
            return this[reference.CollectionName].FindOne(query);
        }

        /// <summary>
        /// Follows the reference.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        public T FollowReference<T>(DBRef reference) where T:class
        {
            if(reference == null)
                throw new ArgumentNullException("reference", "cannot be null");
            var query = new Document().Add("_id", reference.Id);
            return GetCollection<T>(reference.CollectionName).FindOne(query);
        }

        /// <summary>
        /// Most operations do not have a return code in order to save the client from having to wait for results.
        /// GetLastError can be called to retrieve the return code if clients want one.
        /// </summary>
        /// <returns></returns>
        public Document GetLastError(){
            return SendCommand("getlasterror");
        }

        /// <summary>
        /// Retrieves the last error and forces the database to fsync all files before returning.
        /// </summary>
        /// <param name="fsync">if set to <c>true</c> [fsync].</param>
        /// <returns></returns>
        /// <remarks>
        /// Server version 1.3+
        /// </remarks>
        public Document GetLastError(bool fsync){
            return SendCommand(new Document { { "getlasterror", 1.0 }, { "fsync", fsync } });
        }

        /// <summary>
        /// Call after sending a bulk operation to the database.
        /// </summary>
        /// <returns></returns>
        public Document GetPreviousError(){
            return SendCommand("getpreverror");
        }

        /// <summary>
        /// Gets the sister database on the same Mongo connection with the given name.
        /// </summary>
        /// <param name="sisterDatabaseName">Name of the sister database.</param>
        /// <returns></returns>
        public Database GetSisterDatabase(string sisterDatabaseName){
            return new Database(_connection, sisterDatabaseName);
        }

        /// <summary>
        ///   Resets last error.  This is good to call before a bulk operation.
        /// </summary>
        public void ResetError(){
            SendCommand("reseterror");
        }

        /// <summary>
        /// Evals the specified javascript.
        /// </summary>
        /// <param name="javascript">The javascript.</param>
        /// <returns></returns>
        public Document Eval(string javascript){
            return Eval(javascript, new Document());
        }

        /// <summary>
        /// Evals the specified javascript.
        /// </summary>
        /// <param name="javascript">The javascript.</param>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        public Document Eval(string javascript, Document scope){
            return Eval(new CodeWScope(javascript, scope));
        }

        /// <summary>
        /// Evals the specified code scope.
        /// </summary>
        /// <param name="codeScope">The code scope.</param>
        /// <returns></returns>
        public Document Eval(CodeWScope codeScope){
            var cmd = new Document().Add("$eval", codeScope);
            return SendCommand(cmd);
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="commandName">The command name.</param>
        /// <returns></returns>
        public Document SendCommand(string commandName){
            return SendCommand(new Document().Add(commandName, 1.0));
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The CMD.</param>
        /// <returns></returns>
        public Document SendCommand(Document command){
            AuthenticateIfRequired();
            return SendCommandCore(command);
        }

        /// <summary>
        /// Sends the command core.
        /// </summary>
        /// <remarks>Required only for auth.</remarks>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        private Document SendCommandCore(Document command){
            var result = FindOneCommand<Document>(command);
            var ok = (double)result["ok"];
            if(ok != 1.0)
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
        /// <param name="commandName">Name of the command.</param>
        /// <returns></returns>
        public T SendCommand<T>(string commandName) where T : CommandResultBase{
            return SendCommand<T>(new Document().Add(commandName, 1.0));
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public T SendCommand<T>(object command) where T : CommandResultBase{
            AuthenticateIfRequired();

            var result = FindOneCommand<T>(command);
            if(!result.Success){
                throw new MongoCommandException(result.ErrorMessage, null, null);
            }

            return result;
        }

        /// <summary>
        /// Finds the one command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        private T FindOneCommand<T>(object spec) where T:class{
            var cursor = new Cursor<T>(_connection, Name + ".$cmd", spec??new Document(), -1, 0, null);
            
            foreach(var document in cursor.Documents)
            {
                cursor.Dispose();
                return document;
            }

            return null;
        }

        /// <summary>
        ///   Authenticates the on first request.
        /// </summary>
        private void AuthenticateIfRequired(){
            if(_connection.IsAuthenticated)
                return;

            var builder = new MongoConnectionStringBuilder(_connection.ConnectionString);

            if(string.IsNullOrEmpty(builder.Username))
                return;

            var nonceResult = SendCommandCore(new Document().Add("getnonce", 1.0));
            var nonce = (String)nonceResult["nonce"];

            if(nonce == null)
                throw new MongoException("Error retrieving nonce", null);

            var pwd = Hash(builder.Username + ":mongo:" + builder.Password);
            var auth = new Document{
                {"authenticate", 1.0},
                {"user", builder.Username},
                {"nonce", nonce},
                {"key", Hash(nonce + builder.Username + pwd)}
            };
            try{
                SendCommandCore(auth);
            }
            catch(MongoCommandException exception){
                //Todo: use custom exception?
                throw new MongoException("Authentication faild for " + builder.Username, exception);
            }

            _connection.MaskAuthenticated();
        }

        /// <summary>
        ///   Hashes the specified text.
        /// </summary>
        /// <param name = "text">The text.</param>
        /// <returns></returns>
        internal static string Hash(string text){
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.Default.GetBytes(text));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}