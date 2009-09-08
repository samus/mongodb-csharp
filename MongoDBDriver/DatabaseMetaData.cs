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
            db["$cmd"].FindOne(cmd);
            return new Collection(name, connection, this.name);
        }

                
        public Boolean DropCollection(Collection col){
            return this.DropCollection(col.Name);
        }

        public Boolean DropCollection(String name){
            Document result = db["$cmd"].FindOne(new Document().Append("drop",name));
            return result.Contains("ok") && ((double)result["ok"] == 1);
        }
        
        public Boolean DropDatabase(){
            throw new NotImplementedException();
        }       
    }
}
