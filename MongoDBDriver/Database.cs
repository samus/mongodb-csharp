
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver
{
    public class Database
    {
        private Connection connection;
        
        private String name;
        public string Name {
            get { return name; }
        }

        private DatabaseMetaData metaData;
        public DatabaseMetaData MetaData {
            get { 
                if(metaData == null){
                    metaData = new DatabaseMetaData(this.Name,this.connection);
                }
                return metaData;
            }
        }
        
        public Database(Connection conn, String name){
            this.connection = conn;
            this.name = name;
        }

        public List<String> GetCollectionNames(){
            IMongoCollection namespaces = this["system.namespaces"];
            ICursor cursor = namespaces.Find(null);
            List<String> names = new List<string>();
            foreach (Document doc in cursor.Documents){
                names.Add((String)doc["name"]); //Fix Me: Should filter built-ins
            }
            return names;
        }

        public IMongoCollection this[ String name ]  {
            get{
                return this.GetCollection(name);
            }
        }   
        
		public IMongoCollection GetCollection(String name)
		{
            IMongoCollection col = new Collection(name, this.connection, this.Name);
            return col;
        }
        
        public Document FollowReference(DBRef reference){
            if(reference == null) throw new ArgumentNullException("reference cannot be null");
            Document query = new Document().Append("_id", reference.Id);
            return this[reference.CollectionName].FindOne(query);
        }
        
        public bool Authenticate(string username, string password){
            IMongoCollection cmd = this["$cmd"];
            Document nonceResult = cmd.FindOne(new Document().Append("getnonce", 1.0));
            String nonce = (String)nonceResult["nonce"];
            if (nonce == null){
                throw new MongoException("Error retrieving nonce", null);
            }
            else {
                string pwd = Database.Hash(username + ":mongo:" + password);
                Document auth = new Document();
                auth.Add("authenticate", 1.0);
                auth.Add("user", username);
                auth.Add("nonce", nonce);
                auth.Add("key", Database.Hash(nonce + username + pwd));
                Document authResult = cmd.FindOne(auth);
                return (1.0 == (double)authResult["ok"]);
            }
        }

        public void Logout(){
            IMongoCollection cmd = this["$cmd"];
            Document logoutResult = cmd.FindOne(new Document().Append("logout", 1.0));
            double ok = (double)logoutResult["ok"];
            if (ok != 1.0){
                throw new MongoException(String.Format("Error logging out: {0}",(string)logoutResult["msg"]), null);
            }
        }
        
        internal static string Hash(string text){
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(text));
            return BitConverter.ToString(hash).Replace("-","").ToLower();
        }

    }
}
