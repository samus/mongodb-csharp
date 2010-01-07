using System;
using NUnit.Framework;
using MongoDB.Driver;
using MongoDB.Driver.IO;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver.GridFS.Tests
{
    [TestFixture]
    public class GridFSTest
    {
        [Test]
        public void TestOpenNewGridFile()
        {
            Mongo db = new Mongo();
            db.Connect();
            GridFS gridFS = new GridFS(db);
            using (GridFile gf = new GridFile(gridFS))
            {                
                gf.Open("newfile.txt");
                Console.WriteLine(gf.Id.ToString());
            }
            db.Disconnect();
        }

    }
}
