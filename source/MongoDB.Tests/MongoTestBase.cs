using System;
using System.Configuration;
using NUnit.Framework;
using MongoDB.Configuration;

namespace MongoDB
{
    public abstract class MongoTestBase
    {
        public Mongo Mongo{get;set;}

        public IMongoDatabase DB{
            get{
                return this.Mongo["tests"];
            }
        }

        /// <summary>
        /// Comma separated list of collections to clean at startup.
        /// </summary>
        public abstract string TestCollections{get;}
        
        /// <summary>
        /// Override to add custom initialization code.
        /// </summary>
        public virtual void OnInit(){}
        
        /// <summary>
        /// Override to add custom code to invoke during the test end.
        /// </summary>
        public virtual void OnDispose(){}
        
        /// <summary>
        /// Sets up the test environment.  You can either override this OnInit to add custom initialization.
        /// </summary>
        [TestFixtureSetUp]
        public virtual void Init(){
            this.Mongo = new Mongo(GetConfiguration().BuildConfiguration());
            this.Mongo.Connect();
            CleanDB();
            OnInit();
        }
        
        
        [TestFixtureTearDown]
        public virtual void Dispose(){
            OnDispose();
            this.Mongo.Disconnect();
        }
        
        protected void CleanDB(){
            foreach(string col in this.TestCollections.Split(',')){
                DB["$cmd"].FindOne(new Document(){{"drop", col.Trim()}});
                //Console.WriteLine("Dropping " + col);
            }
        }

        protected virtual MongoConfigurationBuilder GetConfiguration()
        {
            var builder = new MongoConfigurationBuilder();
            builder.ReadConnectionStringFromAppSettings("tests");
            return builder;
        }
    }
}