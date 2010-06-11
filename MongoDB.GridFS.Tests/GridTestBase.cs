using System;
using System.Configuration;
using System.Text;

using NUnit.Framework;

using MongoDB.Driver;

namespace MongoDB.GridFS
{
    public abstract class GridTestBase : MongoTestBase
    {
        /// <summary>
        /// Comma separated list of collections to clean at startup.
        /// </summary>
        public abstract string TestFileSystems{get;}

        /// <summary>
        /// Turns the TestFileSystems string into a comma delimited set of collections 
        /// </summary>
        public override string TestCollections {
            get {
                StringBuilder sb = new StringBuilder();
                foreach(string fs in this.TestFileSystems.Split(',')){
                    sb.Append(fs + ".files,");
                    sb.Append(fs + ".chunks,");
                }
                sb.Remove(sb.Length - 1,1); //remove last ,
                return sb.ToString();
            }
        }
        
        public long CountChunks(string filesystem, Object fileid){
            return DB[filesystem + ".chunks"].Count(new Document().Append("files_id", fileid));
        }        
        
    }
}
