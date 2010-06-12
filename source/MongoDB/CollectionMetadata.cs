using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Configuration;
using MongoDB.Connections;

namespace MongoDB
{
    /// <summary>
    ///   Lazily loaded meta data on the collection.
    /// </summary>
    public class CollectionMetadata
    {
        private readonly MongoDatabase _database;
        private readonly string _fullName;
        private readonly Dictionary<string, Document> _indexes = new Dictionary<string, Document>();
        private readonly string _name;
        private bool _gotIndexes;
        private Document _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionMetadata"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="collectionName">The name.</param>
        /// <param name="connection">The connection.</param>
        internal CollectionMetadata(MongoConfiguration configuration, string databaseName, string collectionName, Connection connection)
        {
            //Todo: Add public constrcutors for users to call
            _fullName = databaseName + "." + collectionName;
            _name = collectionName;
            _database = new MongoDatabase(configuration, connection, databaseName);
        }

        /// <summary>
        ///   Gets the options.
        /// </summary>
        /// <value>The options.</value>
        public Document Options
        {
            get
            {
                if(_options != null)
                    return _options;
                var doc = _database["system.namespaces"].FindOne(new Document().Add("name", _fullName)) ?? new Document();
                if(doc.Contains("create"))
                    doc.Remove("create");
                //Not sure why this is here.  The python driver has it.
                _options = doc;
                return _options;
            }
        }

        /// <summary>
        ///   Gets the indexes.
        /// </summary>
        /// <value>The indexes.</value>
        public Dictionary<string, Document> Indexes
        {
            get
            {
                if(_gotIndexes)
                    return _indexes;

                _indexes.Clear();

                var docs = _database["system.indexes"].Find(new Document().Add("ns", _fullName));
                foreach(var doc in docs.Documents)
                    _indexes.Add((string)doc["name"], doc);

                return _indexes;
            }
        }

        /// <summary>
        ///   Creates the index.
        /// </summary>
        /// <param name = "name">The name.</param>
        /// <param name = "fieldsAndDirections">The fields and directions.</param>
        /// <param name = "unique">if set to <c>true</c> [unique].</param>
        public void CreateIndex(string name, Document fieldsAndDirections, bool unique)
        {
            var index = new Document();
            index["name"] = name;
            index["ns"] = _fullName;
            index["key"] = fieldsAndDirections;
            index["unique"] = unique;
            _database["system.indexes"].Insert(index);
            Refresh();
        }

        /// <summary>
        ///   Creates the index.
        /// </summary>
        /// <param name = "fieldsAndDirections">The fields and directions.</param>
        /// <param name = "unique">if set to <c>true</c> [unique].</param>
        public void CreateIndex(Document fieldsAndDirections, bool unique)
        {
            var name = generateIndexName(fieldsAndDirections, unique);
            CreateIndex(name, fieldsAndDirections, unique);
        }

        /// <summary>
        ///   Drops the index.
        /// </summary>
        /// <param name = "name">The name.</param>
        public void DropIndex(string name)
        {
            var cmd = new Document();
            cmd.Add("deleteIndexes", _name).Add("index", name);
            _database.SendCommand(cmd);
            Refresh();
        }

        /// <summary>
        ///   Renames the specified new name.
        /// </summary>
        /// <param name = "newName">The new name.</param>
        public void Rename(string newName)
        {
            if(string.IsNullOrEmpty(newName))
                throw new ArgumentException("Name must not be null or empty", "newName");

            var cmd = new Document();
            cmd.Add("renameCollection", _fullName).Add("to", _database.Name + "." + newName);
            _database.GetSisterDatabase("admin").SendCommand(cmd);
            Refresh();
        }

        /// <summary>
        ///   Refreshes this instance.
        /// </summary>
        public void Refresh()
        {
            _indexes.Clear();
            _gotIndexes = false;
            _options = null;
        }

        /// <summary>
        ///   Generates the name of the index.
        /// </summary>
        /// <param name = "fieldsAndDirections">The fields and directions.</param>
        /// <param name = "unique">if set to <c>true</c> [unique].</param>
        /// <returns></returns>
        protected string generateIndexName(Document fieldsAndDirections, bool unique)
        {
            var sb = new StringBuilder("_");
            foreach(var key in fieldsAndDirections.Keys)
                sb.Append(key).Append("_");
            if(unique)
                sb.Append("unique_");

            return sb.ToString();
        }
    }
}