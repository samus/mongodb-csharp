
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

		

		public bool Authenticate(string username, string password){
            bool result = false;
            if (this.connection.State == ConnectionState.Opened)
            {
                Collection cmd = this["$cmd"];
                Document nonceResult = cmd.FindOne(new Document().Append("getnonce", 1.0));
                String nonce = (String)nonceResult["nonce"];
                if (nonce == null)
                {
                    throw new MongoException("Error retrieveing nonce", null);
                }
                else {
                    string pwd = md5Hash(username + ":mongo:" + password);
                    Document auth = new Document();
                    auth.Add("authenticate", 1.0);
                    auth.Add("user", username);
                    auth.Add("nonce", nonce);
                    auth.Add("key", md5Hash(nonce + username + pwd));
                    Document authResult = cmd.FindOne(auth);
                    double ok = (double)authResult["ok"];
                    if (ok == 1.0)
                    {
                        result = true;
                    }
                }
                
            }
            else{
                throw new MongoCommException("Operation cannot be performed on a closed connection.", this.connection);
            }
            return result;
		}

        public void AddUser(string username, string password){
            if (this.connection.State == ConnectionState.Opened){
                Collection users = this.GetCollection("system.users");
                string pwd = md5Hash(username + ":mongo:" + password);
                Document user = new Document().Append("user", username).Append("pwd", pwd);
                Document userExists = users.FindOne(new Document().Append("user",username));
                if (userExists != null){
                    throw new MongoException("A user with the name " + username + " already exists in this database.", null);
                }
                else{
                   users.Insert(user);
                }
            }
        }

        public void Logout(){
            Collection cmd = this["$cmd"];
            Document logoutResult = cmd.FindOne(new Document().Append("logout", 1.0));
            double ok = (double)logoutResult["ok"];
            if (ok != 1.0){
                throw new MongoException("An error occured logging out.", null);
            }
        }


		
        public List<String> GetCollectionNames(){
            Collection namespaces = this["system.namespaces"];
            Cursor cursor = namespaces.Find(null);
            List<String> names = new List<string>();
            foreach (Document doc in cursor.Documents){
                names.Add((String)doc["name"]); //Fix Me: Should filter built-ins
            }
            return names;
        }

        private string md5Hash(string text)
        {
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(text));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();

        }
        
        public Collection this[ String name ]  {
            get{
                return this.GetCollection(name);
            }
        }   
        
        public Collection GetCollection(String name){
            Collection col = new Collection(name, this.connection, this.Name);
            return col;
        }
        
        public Document FollowReference(DBRef reference){
            if(reference == null) throw new ArgumentNullException("reference cannot be null");
            Document query = new Document().Append("_id", reference.Id);
            return this[reference.CollectionName].FindOne(query);
        }       
    }
}
