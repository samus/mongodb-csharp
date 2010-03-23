using MongoDB.Driver.Connections;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver
{
    /// <summary>
    ///   Administration of metadata for a database.
    /// </summary>
    public class DatabaseMetaData
    {
        private readonly Connection _connection;
        private readonly MongoDatabase _database;
        private readonly string _name;
        private readonly ISerializationFactory _serializationFactory;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DatabaseMetaData" /> class.
        /// </summary>
        /// <param name = "serializationFactory">The serialization factory.</param>
        /// <param name = "name">The name.</param>
        /// <param name = "conn">The conn.</param>
        public DatabaseMetaData(ISerializationFactory serializationFactory, string name, Connection conn)
        {
            this._serializationFactory = serializationFactory;
            _connection = conn;
            this._name = name;
            _database = new MongoDatabase(serializationFactory, conn, name);
        }

        /// <summary>
        /// Creates the collection.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IMongoCollection CreateCollection(string name)
        {
            return CreateCollection(name, null);
        }

        /// <summary>
        /// Creates the collection.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public IMongoCollection CreateCollection(string name, Document options)
        {
            var cmd = new Document();
            cmd.Add("create", name).Merge(options);
            _database.SendCommand(cmd);
            return new MongoCollection(_serializationFactory, _connection, this._name, name);
        }

        /// <summary>
        /// Drops the collection.
        /// </summary>
        /// <param name="collection">The col.</param>
        /// <returns></returns>
        public bool DropCollection(MongoCollection collection)
        {
            return DropCollection(collection.Name);
        }

        /// <summary>
        /// Drops the collection.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public bool DropCollection(string name)
        {
            var result = _database.SendCommand(new Document().Add("drop", name));
            return result.Contains("ok") && ((double)result["ok"] == 1);
        }

        /// <summary>
        /// Drops the database.
        /// </summary>
        /// <returns></returns>
        public bool DropDatabase()
        {
            var result = _database.SendCommand("dropDatabase");
            return result.Contains("ok") && ((double)result["ok"] == 1);
        }

        /// <summary>
        /// Adds the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public void AddUser(string username, string password)
        {
            var users = _database["system.users"];
            var pwd = MongoDatabase.Hash(username + ":mongo:" + password);
            var user = new Document().Add("user", username).Add("pwd", pwd);

            if(FindUser(username) != null)
                throw new MongoException("A user with the name " + username + " already exists in this database.", null);
            users.Insert(user);
        }

        /// <summary>
        /// Removes the user.
        /// </summary>
        /// <param name="username">The username.</param>
        public void RemoveUser(string username)
        {
            var users = _database["system.users"];
            users.Delete(new Document().Add("user", username));
        }

        /// <summary>
        /// Lists the users.
        /// </summary>
        /// <returns></returns>
        public ICursor ListUsers()
        {
            var users = _database["system.users"];
            return users.FindAll();
        }

        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public Document FindUser(string username)
        {
            return FindUser(new Document().Add("user", username));
        }

        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public Document FindUser(Document spec)
        {
            return _database["system.users"].FindOne(spec);
        }
    }
}