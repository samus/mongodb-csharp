using System;
using MongoDB.Driver.Connections;

namespace MongoDB.Driver
{
    /// <summary>
    /// Administration of metadata for a database.
    /// </summary>
    public class DatabaseMetaData
    {        
        private Connection connection;  
        private string name;
        private MongoDatabase db;
        
        public DatabaseMetaData(string name, Connection conn){
            this.connection = conn;
            this.name = name;
            this.db = new MongoDatabase(conn, name);
        }
        
        public MongoCollection<Document> CreateCollection(String name){
            return this.CreateCollection(name,null);
        }

        public MongoCollection<Document> CreateCollection(String name, Document options)
        {
            Document cmd = new Document();
            cmd.Add("create", name).Merge(options);
            db.SendCommand(cmd);
            return new MongoCollection<Document>(connection, this.name, name);
        }


        public Boolean DropCollection(MongoCollection<Document> col)
        {
            return this.DropCollection(col.Name);
        }

        public Boolean DropCollection(String name){
            Document result = db.SendCommand(new Document().Add("drop", name));
            return result.Contains("ok") && ((double)result["ok"] == 1);
        }
        
        public Boolean DropDatabase(){
			Document result = db.SendCommand("dropDatabase");
			return result.Contains("ok") && ((double)result["ok"] == 1);
        }
        
        public void AddUser(string username, string password){
            IMongoCollection<Document> users = db["system.users"];
            string pwd = MongoDatabase.Hash(username + ":mongo:" + password);
            Document user = new Document().Add("user", username).Add("pwd", pwd);
            
            if (FindUser(username) != null){
                throw new MongoException("A user with the name " + username + " already exists in this database.", null);
            }
            users.Insert(user);
        }

        public void RemoveUser(string username){
            IMongoCollection<Document> users = db["system.users"];
            users.Delete(new Document().Add("user", username));
        }

        public ICursor<Document> ListUsers(){
            IMongoCollection<Document> users = db["system.users"];
            return users.FindAll();
        }

        public Document FindUser(string username){
            return FindUser(new Document().Add("user", username));
        }

        public Document FindUser(Document spec){
            return db["system.users"].FindOne(spec);
        }
    }
}
