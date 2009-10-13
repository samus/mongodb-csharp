using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.Driver
{
    public enum IndexOrder:int{
        Descending = -1,
        Ascending = 1
    }
    
    /// <summary>
    /// Lazily loaded meta data on the collection.
    /// </summary>
    public class CollectionMetaData
    {
        private string fullName;        
        private string name;
        private Database db;
        
        public CollectionMetaData(string dbName, string name, Connection conn){
            this.fullName = dbName + "." + name;
            this.name = name;
            this.db = new Database(conn, dbName);
        }
        
        private Document options = null;
        public Document Options {
            get { 
                if(options != null)return options;
                Document doc = db["system.namespaces"].FindOne(new Document().Append("name", this.fullName));
                if(doc == null) doc = new Document();
                if(doc.Contains("create"))doc.Remove("create"); //Not sure why this is here.  The python driver has it.
                this.options = doc;
                return this.options;
            }
        }
        
        private bool gotIndexes = false;
        private Dictionary<string, Document> indexes = new Dictionary<string, Document>();
        public Dictionary<string, Document> Indexes {
            get { 
                if(gotIndexes)return indexes;
                
                indexes.Clear();
                
				ICursor docs = db["system.indexes"].Find(new Document().Append("ns", this.fullName));
                foreach(Document doc in docs.Documents){
                    indexes.Add((string)doc["name"],doc);
                }
                
                return indexes;
            }
        }

        public void CreateIndex(string name, Document fieldsAndDirections, bool unique){
            Document index = new Document();
            index["name"] = name;
            index["ns"] = this.fullName;
            index["key"] = fieldsAndDirections;
            index["unique"] = unique;
            db["system.indexes"].Insert(index);
            this.refresh();
        }
        
        public void CreateIndex(Document fieldsAndDirections, bool unique){
            string name = this.generateIndexName(fieldsAndDirections,unique);
            this.CreateIndex(name, fieldsAndDirections,unique);
        }
        
        public void DropIndex(string name){
            Document cmd = new Document();
            cmd.Append("deleteIndexes",this.name).Append("index",name);
			db.SendCommand(cmd);
            this.refresh();
        }
        
        public void refresh(){
            indexes.Clear();
            gotIndexes = false;
            options = null;
        }
        
        protected string generateIndexName(Document fieldsAndDirections, bool unique){
            StringBuilder sb = new StringBuilder("_");
            foreach(string key in fieldsAndDirections.Keys){
                sb.Append(key).Append("_");
            }
            if(unique) sb.Append("unique_");
            
            return sb.ToString();
        }
        
    }
}
