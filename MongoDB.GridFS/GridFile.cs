using System;
using System.IO;
using MongoDB.Driver;

namespace MongoDB.GridFS
{
    public class GridFile{

        private Database db;
        
        private string name;
        public string Name {
            get { return name; }
        }
        
        private IMongoCollection files;
        public IMongoCollection Files{
            get { return this.files; }
        }

        private IMongoCollection chunks;
        public IMongoCollection Chunks{
            get { return this.chunks; }
        }        
        
        public GridFile(Database db):this(db,"fs"){}

        public GridFile(Database db, string bucket){
            this.db = db;
            this.files = db[bucket + ".files"];
            this.chunks = db[bucket + ".chunks"];
            this.chunks.MetaData.CreateIndex(new Document().Append("files_id", 1).Append("n", 1),true);
            this.name = bucket;
        }
        
        public ICursor ListFiles(){
            return this.ListFiles(new Document());
        }
        
        public ICursor ListFiles(Document query){
            return this.files.Find(new Document().Append("query",query)
                                                .Append("orderby", new Document()
                                                .Append("filename", 1)));
        }
        
        /// <summary>
        /// Copies one file to another.  The destination file must not exist or an IOException will be thrown.
        /// </summary>
        /// <exception cref="FileNotFoundException">Source file not found.</exception>
        /// <exception cref="IOException">Destination file already exists.</exception>
        /// <exception cref="MongoCommandException">A database error occurred executing the copy function.</exception>
        public void Copy(String src, String dest){
            if(Exists(src) == false) throw new FileNotFoundException("Not found in the database.", src);
            if(Exists(dest) == true) throw new IOException("Destination file already exists.");
            
            Document scope = new Document().Append("bucket", this.name).Append("srcfile", src).Append("destfile",dest);
            String func ="function(){\n" +
                            //"   print(\"copying \" + srcfile);\n" +
                            "   var files = db[bucket + \".files\"];\n" +
                            "   var chunks = db[bucket + \".chunks\"];\n" +
                            "   var srcdoc = files.findOne({filename:srcfile});\n" +
                            //"   return srcdoc; \n" +
                            "   if(srcdoc != undefined){\n" +
                            "       var srcid = srcdoc._id;\n" +
                            "       var newid = ObjectId();\n" +
                            "       srcdoc._id = newid\n" +
                            "       srcdoc.filename = destfile;\n" +
                            "       files.insert(srcdoc);\n" +
                            "       chunks.find({files_id:srcid}).forEach(function(chunk){\n" +
                            //"           print(\"copying chunk...\");\n" +
                            "           chunk._id = ObjectId();\n" +
                            "           chunk.files_id = newid;\n" +
                            "           chunks.insert(chunk);\n" +
                            "       });\n" +
                            "       return true;\n" +
                            "   }\n" +
                            "   return false;\n" +
                            "}";
            Document result = db.Eval(func,scope);
        }
        
        #region Create
        public GridFileStream Create(String filename){
            return Create(filename, FileMode.Create);
        }
        
        public GridFileStream Create(String filename, FileMode mode){
            return Create(filename,mode,FileAccess.ReadWrite);
        }
        
        public GridFileStream Create(String filename, FileMode mode, FileAccess access){
            //Create is delegated to a GridFileInfo because the stream needs access to the gfi and it
            //is easier to do it this way and only write the implementation once.
            GridFileInfo gfi = new GridFileInfo(this.db,this.name,filename);
            return gfi.Create(mode,access);

        }
        #endregion

        #region Opens
        public GridFileStream Open(string filename, FileMode mode, FileAccess access){
            return new GridFileInfo(this.db, this.name, filename).Open(mode, access);
        }

        public GridFileStream OpenRead(String filename){
            GridFileInfo gfi = new GridFileInfo(this.db, this.name, filename);
            return gfi.OpenRead();
        }

        public GridFileStream OpenWrite(String filename){
            GridFileInfo gfi = new GridFileInfo(this.db, this.name, filename);
            return gfi.OpenWrite();
        }
        #endregion

        
        #region Delete
        public void Delete(Object id){
            files.Delete(new Document().Append("_id",id));
            chunks.Delete(new Document().Append("files_id",id));
        }
        
        public void Delete(String filename){
            files.Delete(new Document().Append("filename",filename));
        }
        
        public void Delete(Document query ){
            foreach(Document doc in ListFiles(query).Documents){
                Delete((Oid)doc["_id"]);
            }
        }
        #endregion
        
        #region Exists        
        public Boolean Exists(string name){
            return this.files.FindOne(new Document().Append("filename",name)) != null;
        }

        public Boolean Exists(Object id){
            return this.files.FindOne(new Document().Append("_id",id)) != null;
        }
        #endregion        
        
        #region Move
        public void Move(String src, String dest){
            this.files.Update(new Document().Append("$set", new Document().Append("filename",dest)), new Document().Append("filename", src));
        }
        
        public void Move(Object id, String dest){
            this.files.Update(new Document().Append("$set", new Document().Append("filename",dest)), new Document().Append("_id", id));
        }
        #endregion      
        
    }

}
