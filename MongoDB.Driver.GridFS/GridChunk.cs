using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;

namespace MongoDB.Driver.GridFS
{
    public struct GridChunk : IComparable
    {
        public GridChunk(object filesId, int n, byte[] data){
            OidGenerator oidGenerator = new OidGenerator();
            this.id = oidGenerator.Generate();
            this.filesId = filesId;
            this.n = n;
            this.data = new Binary(data);
        }

        public GridChunk(Document doc)
        {
            this.id = (Oid)doc["_id"];
            this.filesId = (Object)doc["files_id"];
            this.n = (int)doc["n"];
            this.data = (Binary)doc["data"];
        }

        // object id of the chunk in the _chunks collection
        private Oid id;
        public Oid Id{
            get { return this.id; }
            set { this.id = value; }
        }
        // id value of the owning {{files}} collection entry
        private Object filesId;
        public Object FilesId{
            get { return this.filesId; }
            set { this.filesId = value; }
        }

        //Chunk number
        private int n;
        public int N{
            get { return this.n; }
            set { this.n = value; }
        }

        private Binary data;
        public Binary Data
        {
            get { return this.data; }
        }

        //Allow sorting by chunk number
        public int CompareTo(Object obj){
            GridChunk chunk = (GridChunk)obj;
            return this.n.CompareTo(chunk.N);            
           }

        public Document ToDocument()
        {
            Document doc = new Document();
            doc["_id"] = this.id;
            doc["files_id"] = this.filesId;
            doc["n"] = this.n;
            doc["data"] = this.data;
            return doc;
        }

    }
}
