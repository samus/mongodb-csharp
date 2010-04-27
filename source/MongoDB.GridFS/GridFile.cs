using System;
using System.IO;
using MongoDB;

namespace MongoDB.GridFS
{
    /// <summary>
    /// 
    /// </summary>
    public class GridFile{

        private IMongoDatabase db;
        
        private string name;
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name {
            get { return name; }
        }

        private IMongoCollection files;
        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>The files.</value>
        public IMongoCollection Files
        {
            get { return this.files; }
        }

        private IMongoCollection chunks;
        /// <summary>
        /// Gets the chunks.
        /// </summary>
        /// <value>The chunks.</value>
        public IMongoCollection Chunks
        {
            get { return this.chunks; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridFile"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        public GridFile(IMongoDatabase db):this(db,"fs"){}

        /// <summary>
        /// Initializes a new instance of the <see cref="GridFile"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="bucket">The bucket.</param>
        public GridFile(IMongoDatabase db, string bucket){
            this.db = db;
            this.files = db[bucket + ".files"];
            this.chunks = db[bucket + ".chunks"];
            this.chunks.MetaData.CreateIndex(new Document().Add("files_id", 1).Add("n", 1), true);
            this.name = bucket;
        }

        /// <summary>
        /// Lists the files.
        /// </summary>
        /// <returns></returns>
        public ICursor ListFiles(){
            return this.ListFiles(new Document());
        }

        /// <summary>
        /// Lists the files.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public ICursor ListFiles(Document query)
        {
            return this.files.Find(new Document().Add("query", query)
                                                .Add("orderby", new Document()
                                                .Add("filename", 1)));
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

            Document scope = new Document().Add("bucket", this.name).Add("srcfile", src).Add("destfile", dest);
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
            db.Eval(func,scope);
        }
        
        #region Create
        /// <summary>
        /// Creates the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public GridFileStream Create(String filename){
            return Create(filename, FileMode.Create);
        }

        /// <summary>
        /// Creates the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public GridFileStream Create(String filename, FileMode mode){
            return Create(filename,mode,FileAccess.ReadWrite);
        }

        /// <summary>
        /// Creates the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="access">The access.</param>
        /// <returns></returns>
        public GridFileStream Create(String filename, FileMode mode, FileAccess access){
            //Create is delegated to a GridFileInfo because the stream needs access to the gfi and it
            //is easier to do it this way and only write the implementation once.
            GridFileInfo gfi = new GridFileInfo(this.db,this.name,filename);
            return gfi.Create(mode,access);

        }
        #endregion

        #region Opens
        /// <summary>
        /// Opens the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="access">The access.</param>
        /// <returns></returns>
        public GridFileStream Open(string filename, FileMode mode, FileAccess access){
            return new GridFileInfo(this.db, this.name, filename).Open(mode, access);
        }

        /// <summary>
        /// Opens the read.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public GridFileStream OpenRead(String filename){
            GridFileInfo gfi = new GridFileInfo(this.db, this.name, filename);
            return gfi.OpenRead();
        }

        /// <summary>
        /// Opens the write.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public GridFileStream OpenWrite(String filename){
            GridFileInfo gfi = new GridFileInfo(this.db, this.name, filename);
            return gfi.OpenWrite();
        }
        #endregion

        
        #region Delete
        
        /// <summary>
        /// Permanently removes a file from the database. 
        /// </summary>
        public void Delete(Object id){
            files.Delete(new Document().Add("_id", id));
            chunks.Delete(new Document().Add("files_id", id));
        }
        
        /// <summary>
        /// Permanently removes a file from the database. 
        /// </summary>        
        public void Delete(String filename){
            files.Delete(new Document().Add("filename", filename));
        }
        
        /// <summary>
        /// Permanently removes all files from the database that match the query. 
        /// </summary>
        public void Delete(Document query ){
            foreach(Document doc in ListFiles(query).Documents){
                Delete((Oid)doc["_id"]);
            }
        }
        #endregion
        
        #region Exists
        /// <summary>
        /// Gets a value indicating whether the file exists.
        /// </summary>
        public Boolean Exists(string name){
            return this.files.FindOne(new Document().Add("filename", name)) != null;
        }
        /// <summary>
        /// Gets a value indicating whether the file exists.
        /// </summary>
        public Boolean Exists(Object id){
            return this.files.FindOne(new Document().Add("_id", id)) != null;
        }
        #endregion        
        
        #region Move
        /// <summary>
        /// Moves the specified SRC.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <param name="dest">The dest.</param>
        public void Move(String src, String dest){
            this.files.Update(new Document().Add("$set", new Document().Add("filename", dest)), new Document().Add("filename", src));
        }

        /// <summary>
        /// Moves the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="dest">The dest.</param>
        public void Move(Object id, String dest){
            this.files.Update(new Document().Add("$set", new Document().Add("filename", dest)), new Document().Add("_id", id));
        }
        #endregion      
        
    }

}
