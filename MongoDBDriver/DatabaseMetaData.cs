using System;

namespace MongoDB.Driver
{
    /// <summary>
    /// Administration of metadata for a database.
    /// </summary>
    public class DatabaseMetaData
    {
        
        private Connection connection;  
        private string name;
        private Database db;
        
        public DatabaseMetaData(string name, Connection conn){
            this.connection = conn;
            this.name = name;
            this.db = new Database(conn, name);
        }
        
        public Collection CreateCollection(String name){
            return this.CreateCollection(name,null);
        }
        
        public Collection CreateCollection(String name, Document options){
            Document cmd = new Document();
            cmd.Append("create", name).Update(options);
            db.SendCommand(cmd);
            return new Collection(name, connection, this.name);
        }

                
        public Boolean DropCollection(Collection col){
            return this.DropCollection(col.Name);
        }

        public Boolean DropCollection(String name){
			Document result = db.SendCommand(new Document().Append("drop",name));
            return result.Contains("ok") && ((double)result["ok"] == 1);
        }
        
        public Boolean DropDatabase(){
            throw new NotImplementedException();
        }
        
        public void AddUser(string username, string password){
            IMongoCollection users = db["system.users"];
            string pwd = Database.Hash(username + ":mongo:" + password);
            Document user = new Document().Append("user", username).Append("pwd", pwd);
            if (FindUser(username) != null){
                throw new MongoException("A user with the name " + username + " already exists in this database.", null);
            }
            else{
               users.Insert(user);
            }
        }

        public void RemoveUser(string username){
            IMongoCollection users = db["system.users"];
            users.Delete(new Document().Append("user", username));
        }

        public ICursor ListUsers(){
            IMongoCollection users = db["system.users"];
            return users.FindAll();
        }

        public Document FindUser(string username){
            return FindUser(new Document().Append("user",username));
        }

        public Document FindUser(Document spec){
            return db["system.users"].FindOne(spec);
        }
    }
}
