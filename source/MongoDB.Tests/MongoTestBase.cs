using System;
using System.Configuration;
using NUnit.Framework;

namespace MongoDB
{
    public abstract class MongoTestBase
    {
        public Mongo Mongo { get; set; }

        /// <summary>
        ///   Gets the tests database.
        /// </summary>
        /// <value>The tests database.</value>
        public IMongoDatabase TestsDatabase
        {
            get { return Mongo["tests"]; }
        }

        /// <summary>
        ///   Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; private set; }

        /// <summary>
        ///   Comma separated list of collections to clean at startup.
        /// </summary>
        public abstract string TestCollections { get; }

        /// <summary>
        ///   Override to add custom initialization code.
        /// </summary>
        public virtual void OnInit()
        {
        }

        /// <summary>
        ///   Override to add custom code to invoke during the test end.
        /// </summary>
        public virtual void OnDispose()
        {
        }

        /// <summary>
        ///   Sets up the test environment.  You can either override this OnInit to add custom initialization.
        /// </summary>
        [TestFixtureSetUp]
        public virtual void Init()
        {
            ConnectionString = ConfigurationManager.AppSettings["tests"];
            if(String.IsNullOrEmpty(ConnectionString))
                throw new ArgumentNullException("Connection string not found.");
            Mongo = new Mongo(ConnectionString);
            Mongo.Connect();
            CleanDatabase();
            OnInit();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        [TestFixtureTearDown]
        public virtual void Dispose()
        {
            OnDispose();
            Mongo.Disconnect();
        }

        /// <summary>
        /// Cleans the DB.
        /// </summary>
        protected void CleanDatabase()
        {
            foreach(var col in TestCollections.Split(','))
                TestsDatabase["$cmd"].FindOne(new Document {{"drop", col.Trim()}});
                //Console.WriteLine("Dropping " + col);
        }
    }
}