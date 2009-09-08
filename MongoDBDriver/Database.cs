
using System;
using System.Collections;
using System.Collections.Generic;

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
            Collection namespaces = this["system.namespaces"];
            Cursor cursor = namespaces.Find(null);
            List<String> names = new List<string>();
            foreach (Document doc in cursor.Documents){
                names.Add((String)doc["name"]); //Fix Me: Should filter built-ins
            }
            return names;
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
