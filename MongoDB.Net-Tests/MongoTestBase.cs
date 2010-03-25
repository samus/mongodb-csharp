using System;
using System.Configuration;
using NUnit.Framework;

namespace MongoDB.Driver
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
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; private set; }

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
            ConnectionString = ConfigurationManager.AppSettings["tests"];
            if(String.IsNullOrEmpty(ConnectionString))
                throw new ArgumentNullException("Connection string not found.");
            this.Mongo = new Mongo(ConnectionString);
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
    }
}