using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Driver.Connections;
using MongoDB.Driver.Protocol;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver
{
    public class Cursor<T> : ICursor<T>
        where T : class
    {
        private readonly Connection _connection;
        private readonly Document _specOpts = new Document();
        private bool _isModifiable = true;
        private object _spec;
        private object _fields;
        private int _limit;
        private QueryOptions _options;
        private ReplyMessage<T> _reply;
        private int _skip;
        private ISerializationFactory _serializationFactory = SerializationFactory.Default;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Cursor&lt;T&gt;" /> class.
        /// </summary>
        /// <param name = "connection">The conn.</param>
        /// <param name = "fullCollectionName">Full name of the collection.</param>
        public Cursor(Connection connection, string fullCollectionName){
            //Todo: should be internal
            Id = -1;
            _connection = connection;
            FullCollectionName = fullCollectionName;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Cursor&lt;T&gt;" /> class.
        /// </summary>
        /// <param name = "connection">The conn.</param>
        /// <param name = "fullCollectionName">Full name of the collection.</param>
        /// <param name = "spec">The spec.</param>
        /// <param name = "limit">The limit.</param>
        /// <param name = "skip">The skip.</param>
        /// <param name = "fields">The fields.</param>
        public Cursor(Connection connection, string fullCollectionName, object spec, int limit, int skip, object fields)
            : this(connection, fullCollectionName){
            //Todo: should be internal
            if(spec == null)
                spec = new Document();
            _spec = spec;
            _limit = limit;
            _skip = skip;
            _fields = fields;
        }

        /// <summary>
        /// Gets or sets the full name of the collection.
        /// </summary>
        /// <value>The full name of the collection.</value>
        public string FullCollectionName { get; private set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public long Id { get; private set; }

        /// <summary>
        /// Specs the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public ICursor<T> Spec(Document spec){
            return Spec((object)spec);
        }

        /// <summary>
        /// Specs the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public ICursor<T> Spec(object spec){
            TryModify();
            _spec = spec;
            return this;
        }

        /// <summary>
        /// Limits the specified limit.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        public ICursor<T> Limit(int limit){
            TryModify();
            _limit = limit;
            return this;
        }

        /// <summary>
        /// Skips the specified skip.
        /// </summary>
        /// <param name="skip">The skip.</param>
        /// <returns></returns>
        public ICursor<T> Skip(int skip){
            TryModify();
            _skip = skip;
            return this;
        }

        /// <summary>
        /// Fieldses the specified fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ICursor<T> Fields(Document fields){
            return Fields((object)fields);
        }

        /// <summary>
        /// Fieldses the specified fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ICursor<T> Fields(object fields){
            TryModify();
            _fields = fields;
            return this;
        }

        /// <summary>
        ///   Sorts the specified field.
        /// </summary>
        /// <param name = "field">The field.</param>
        /// <returns></returns>
        public ICursor<T> Sort(string field){
            return Sort(field, IndexOrder.Ascending);
        }

        /// <summary>
        ///   Sorts the specified field.
        /// </summary>
        /// <param name = "field">The field.</param>
        /// <param name = "order">The order.</param>
        /// <returns></returns>
        public ICursor<T> Sort(string field, IndexOrder order){
            return Sort(new Document().Append(field, order));
        }

        /// <summary>
        /// Sorts the specified fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ICursor<T> Sort(Document fields){
            return Sort((object)fields);
        }

        /// <summary>
        /// Sorts the specified fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ICursor<T> Sort(object fields)
        {
            TryModify();
            AddOrRemoveSpecOpt("$orderby", fields);
            return this;
        }

        /// <summary>
        ///   Hints the specified index.
        /// </summary>
        /// <param name = "index">The index.</param>
        /// <returns></returns>
        public ICursor<T> Hint(Document index){
            return Hint((object)index);
        }

        /// <summary>
        /// Hints the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public ICursor<T> Hint(object index)
        {
            TryModify();
            AddOrRemoveSpecOpt("$hint", index);
            return this;
        }

        /// <summary>
        ///   Snapshots the specified index.
        /// </summary>
        /// <param name = "index">The index.</param>
        /// <returns></returns>
        public ICursor<T> Snapshot(Document index){
            return Snapshot((object)index);
        }

        /// <summary>
        /// Snapshots the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public ICursor<T> Snapshot(object index)
        {
            TryModify();
            AddOrRemoveSpecOpt("$snapshot", index);
            return this;
        }

        /// <summary>
        ///   Explains this instance.
        /// </summary>
        /// <returns></returns>
        public T Explain(){
            //Todo: i am not sure that this will work now.
            TryModify();
            _specOpts["$explain"] = true;

            var documents = Documents;
            
            using((IDisposable)documents){
                foreach(var document in documents)
                    return document;
            }

            throw new InvalidOperationException("Explain failed.");
        }

        /// <summary>
        ///   Gets a value indicating whether this <see cref = "Cursor&lt;T&gt;" /> is modifiable.
        /// </summary>
        /// <value><c>true</c> if modifiable; otherwise, <c>false</c>.</value>
        public bool IsModifiable{
            get { return _isModifiable; }
        }

        /// <summary>
        ///   Gets the documents.
        /// </summary>
        /// <value>The documents.</value>
        public IEnumerable<T> Documents{
            get{
                if(_reply == null)
                    RetrieveData();
                if(_reply == null)
                    throw new InvalidOperationException("Expecting reply but get null");

                var documents = _reply.Documents;
                var documentCount = 0;
                var shouldBreak = false;

                while(!shouldBreak){
                    foreach(var document in documents)
                        if((_limit == 0) || (_limit != 0 && documentCount < _limit)){
                            documentCount++;
                            yield return document;
                        }
                        else
                            yield break;

                    if(Id != 0){
                        RetrieveMoreData();
                        documents = _reply.Documents;
                        if(documents == null)
                            shouldBreak = true;
                    }
                    else
                        shouldBreak = true;
                }
            }
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose(){
            if(Id == 0) //All server side resources disposed of.
                return;

            var killCursorsMessage = new KillCursorsMessage(Id);
            try{
                _connection.SendMessage(killCursorsMessage);
                Id = 0;
            }
            catch(IOException exception){
                throw new MongoCommException("Could not read data, communication failure", _connection, exception);
            }
        }

        /// <summary>
        ///   Optionses the specified options.
        /// </summary>
        /// <param name = "options">The options.</param>
        /// <returns></returns>
        public ICursor<T> Options(QueryOptions options){
            TryModify();
            _options = options;
            return this;
        }

        /// <summary>
        ///   Retrieves the data.
        /// </summary>
        private void RetrieveData(){
            var descriptor = _serializationFactory.GetDescriptor(null, _connection);

            var query = new QueryMessage<T>(descriptor){
                FullCollectionName = FullCollectionName,
                Query = BuildSpec(),
                NumberToReturn = _limit,
                NumberToSkip = _skip,
                Options = _options
            };

            if(_fields != null)
                query.ReturnFieldSelector = _fields;
            try{
                _reply = _connection.SendTwoWayMessage<T>(query);
                Id = _reply.CursorId;
                if(_limit < 0)
                    _limit = _limit*-1;
                _isModifiable = false;
            }
            catch(IOException exception){
                throw new MongoCommException("Could not read data, communication failure", _connection, exception);
            }
        }

        /// <summary>
        ///   Retrieves the more data.
        /// </summary>
        private void RetrieveMoreData(){
            var getMoreMessage = new GetMoreMessage(FullCollectionName, Id, _limit);

            try{
                _reply = _connection.SendTwoWayMessage<T>(getMoreMessage);
                Id = _reply.CursorId;
            }
            catch(IOException exception){
                Id = 0;
                throw new MongoCommException("Could not read data, communication failure", _connection, exception);
            }
        }

        /// <summary>
        ///   Tries the modify.
        /// </summary>
        private void TryModify(){
            if(_isModifiable)
                return;
            throw new InvalidOperationException("Cannot modify a cursor that has already returned documents.");
        }

        /// <summary>
        ///   Adds the or remove spec opt.
        /// </summary>
        /// <param name = "key">The key.</param>
        /// <param name = "doc">The doc.</param>
        private void AddOrRemoveSpecOpt(string key, object doc){
            if(doc == null)
                _specOpts.Remove(key);
            else
                _specOpts[key] = doc;
        }

        /// <summary>
        ///   Builds the spec.
        /// </summary>
        /// <returns></returns>
        private object BuildSpec(){
            if(_specOpts.Count == 0)
                return _spec;

            var document = new Document();
            _specOpts.CopyTo(document);
            document["$query"] = _spec;
            return document;
        }
    }
}