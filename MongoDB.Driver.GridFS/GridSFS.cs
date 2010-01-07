using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace MongoDB.Driver.GridFS
{
    public class GridSFS
    {
        public GridSFS(Mongo mongo)
        {
            this.mongo = mongo;
            this.files = mongo["fs"]["files"];
            this.chunks = mongo["fs"]["chunks"];
        }

        public GridSFS(Mongo mongo, string collection){
            this.mongo = mongo;
            this.files = mongo["fs"][collection];
            this.chunks = mongo["fs"][collection];
        }
        
        //public void StoreFile(string filepath)
        //{
        //    if (File.Exists(filepath))
        //    {
        //        fileStream = new FileStream(filepath, FileMode.Open);
        //        binaryReader = new BinaryReader(fileStream);
        //        int chunkNumber = 0;
        //        int offset = 0;
        //        int lastSize = (int)fileStream.Length % this.gridFile.ChunkSize;
        //        double nthChunk = 0;
        //        if (fileStream.Length > gridFile.ChunkSize)
        //        {
        //            nthChunk = Math.Ceiling(fileStream.Length / (double)gridFile.ChunkSize);
        //        }
        //        while (offset < fileStream.Length)
        //        {
        //            byte[] data = new byte[gridFile.ChunkSize];
        //            if (chunkNumber < nthChunk)
        //            {
        //                data = binaryReader.ReadBytes(gridFile.ChunkSize);
        //            }
        //            else
        //            {
        //                data = binaryReader.ReadBytes(lastSize);
        //            }
                 
        //            gridFile.Chunks.Add(new GridChunk(gridFile.Id, chunkNumber, data));
        //            offset += gridFile.ChunkSize;
        //            chunkNumber++;
        //        }
        //    }
        //    else
        //    {
        //        throw new IOException("This file does not exist.");
        //    }
        //}

        private Mongo mongo;
        public Mongo Mongo
        {
            get { return this.mongo; }
        }
        private IMongoCollection files;
        public IMongoCollection Files
        {
            get { return this.files; }
        }

        private IMongoCollection chunks;
        public IMongoCollection Chunks
        {
            get { return this.chunks; }
        }
    
    }

}
