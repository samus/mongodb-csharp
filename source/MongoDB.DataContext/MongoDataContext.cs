using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Configuration;
using MongoDB.Linq;

namespace MongoDB.DataContext
{
    public class MongoDataContext : IMongoDataContext
    {
        private readonly IMongoConfiguration _configuration;
        private readonly IMongoDatabase _database;
        private IMongo _mongo;

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IMongoConfiguration Configuration
        {
            get { return _configuration; }
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        public IMongoDatabase Database
        {
            get { return _database; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDataContext"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public MongoDataContext(IMongoConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            configuration.Validate();

            _configuration = configuration;

            var databaseName = new MongoConnectionStringBuilder(configuration.ConnectionString).Database;
            if (string.IsNullOrEmpty(databaseName))
                databaseName = MongoConnectionStringBuilder.DefaultDatabase;

            _mongo = new Mongo(_configuration);
            _mongo.Connect();
            _database = _mongo.GetDatabase(databaseName);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="MongoDataContext"/> is reclaimed by garbage collection.
        /// </summary>
        ~MongoDataContext()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Begins a queryable find.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQueryable<T> Find<T>() where T : class
        {
            return _database.GetCollection<T>().Linq();
        }

        /// <summary>
        /// Finds all the documents matching the predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _database.GetCollection<T>().Find(predicate).Documents;
        }

        /// <summary>
        /// Finds all the documents matching the predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IEnumerable<T> Find<T>(object predicate) where T : class
        {
            return _database.GetCollection<T>().Find(predicate).Documents;
        }

        /// <summary>
        /// Removes the specified document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document">The document.</param>
        public void Remove<T>(T document) where T : class
        {
            _database.GetCollection<T>().Remove(document);
        }

        /// <summary>
        /// Removes all the entities matching the predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        public void Remove<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            _database.GetCollection<T>().Remove(predicate);
        }

        /// <summary>
        /// Removes all the entities matching the predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        public void Remove<T>(object predicate) where T : class
        {
            _database.GetCollection<T>().Remove(predicate);
        }

        /// <summary>
        /// Saves the specified document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document">The document.</param>
        public void Save<T>(T document) where T : class
        {
            _database.GetCollection<T>().Save(document);
        }

        /// <summary>
        /// Updates all the documents that match the predicate with the specified values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <param name="predicate">The predicate.</param>
        public void Update<T>(object values, object predicate) where T : class
        {
            _database.GetCollection<T>().Update(values, predicate);
        }

        /// <summary>
        /// Updates all the documents that match the predicate with the specified values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <param name="predicate">The predicate.</param>
        public void Update<T>(object values, Expression<Func<T, bool>> predicate) where T : class
        {
            _database.GetCollection<T>().Update(values, predicate);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            var disposable = _mongo as IDisposable;
            if (disposable != null)
                disposable.Dispose();
            else
                _mongo.Disconnect();

            _mongo = null;
        }
        
    }
}
