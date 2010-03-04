using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MongoDB.Driver
{
    public class Database
    {
        private Connection.Connection connection;
        private IMongoCollection command;

        public Database(Connection.Connection conn, String name){
            this.connection = conn;
            this.name = name;
            this.command = this["$cmd"];
        }

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

        private DatabaseJS js;
        public DatabaseJS JS {
            get {
                if(js == null){
                    js = new DatabaseJS(this);
                }
                return js;
            }
        }

        public List<String> GetCollectionNames(){
            IMongoCollection namespaces = this["system.namespaces"];
            ICursor cursor = namespaces.Find(new Document());
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

        public IMongoCollection GetCollection(String name){
            IMongoCollection col = new Collection(name, this.connection, this.Name);
            return col;
        }

        /// <summary>
        /// Gets the document that a reference is pointing to.
        /// </summary>
        public Document FollowReference(DBRef reference){
            if(reference == null) throw new ArgumentNullException("reference cannot be null");
            Document query = new Document().Append("_id", reference.Id);
            return this[reference.CollectionName].FindOne(query);
        }
        
        /// <summary>
        /// Most operations do not have a return code in order to save the client from having to wait for results.
        /// GetLastError can be called to retrieve the return code if clients want one. 
        /// </summary>
        public Document GetLastError(){
            return SendCommand("getlasterror");
        }
        
        /// <summary>
        /// Retrieves the last error and forces the database to fsync all files before returning. 
        /// </summary>
        /// <remarks>Server version 1.3+</remarks>
        public Document GetLastErrorAndFSync(){
            return SendCommand(new Document(){{"getlasterror", 1.0},{"fsync", true}});
        }
        
        /// <summary>
        /// Call after sending a bulk operation to the database. 
        /// </summary>
        public Document GetPreviousError(){
            return SendCommand("getpreverror");
        }
        
        /// <summary>
        /// Resets last error.  This is good to call before a bulk operation.
        /// </summary>
        public void ResetError(){
            SendCommand("reseterror");   
        }
        
        public bool Authenticate(string username, string password){
            Document nonceResult = this.SendCommand("getnonce");
            String nonce = (String)nonceResult["nonce"];
            
            if (nonce == null){
                throw new MongoException("Error retrieving nonce", null);
            }
            
            string pwd = Database.Hash(username + ":mongo:" + password);
            Document auth = new Document();
            auth.Add("authenticate", 1.0);
            auth.Add("user", username);
            auth.Add("nonce", nonce);
            auth.Add("key", Database.Hash(nonce + username + pwd));
            try{
                this.SendCommand(auth);
                return true;
            }catch(MongoCommandException){
                return false;
            }
        }

        public void Logout(){
            this.SendCommand("logout");
        }

        public Document Eval(string javascript){
            return Eval(javascript, new Document());
        }

        public Document Eval(string javascript, Document scope){
            return Eval(new CodeWScope(javascript, scope));
        }

        public Document Eval(CodeWScope cw){
            Document cmd = new Document().Append("$eval", cw);
            return SendCommand(cmd);
        }

        public Document SendCommand(string command){
            Document cmd = new Document().Append(command,1.0);
            return this.SendCommand(cmd);
        }

        public Document SendCommand(Document cmd){
            Document result = this.command.FindOne(cmd);
            double ok = (double)result["ok"];
            if (ok != 1.0){
                string msg;
                if(result.Contains("msg")){
                    msg = (string)result["msg"];
                }else{
                    msg = string.Empty;
                }
                throw new MongoCommandException(msg,result,cmd);
            }
            return result;
        }

        internal static string Hash(string text){
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(text));
            return BitConverter.ToString(hash).Replace("-","").ToLower();
        }
    }
}