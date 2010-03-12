using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Driver.Connections;
using MongoDB.Driver.Protocol;

namespace MongoDB.Driver
{
    public class Cursor<T> : ICursor<T>
        where T : class
    {
        private bool _modifiable = true;
        private readonly Connection _connection;
        private readonly Document _specOpts = new Document();
        private Document _fields;
        private int _limit;
        private QueryOptions _options;
        private ReplyMessage<T> _reply;
        private int _skip;
        private Document _spec;

        public Cursor(Connection conn, string fullCollectionName){
            //Todo: should be internal
            Id = -1;
            _connection = conn;
            FullCollectionName = fullCollectionName;
        }

        public Cursor(Connection conn, String fullCollectionName, Document spec, int limit, int skip, Document fields)
            :this(conn, fullCollectionName){
            //Todo: should be internal
            if(spec == null)
                spec = new Document();
            _spec = spec;
            _limit = limit;
            _skip = skip;
            _fields = fields;
        }

        /// <summary>
        ///   Gets or sets the full name of the collection.
        /// </summary>
        /// <value>The full name of the collection.</value>
        public string FullCollectionName { get; private set; }

        /// <summary>
        ///   Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public long Id { get; private set; }

        /// <summary>
        ///   Specs the specified spec.
        /// </summary>
        /// <param name = "spec">The spec.</param>
        /// <returns></returns>
        public ICursor<T> Spec(Document spec){
            TryModify();
            _spec = spec;
            return this;
        }

        /// <summary>
        /// Limits the specified limit.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        public ICursor<T> Limit(int limit)
        {
            TryModify();
            _limit = limit;
            return this;
        }

        /// <summary>
        /// Skips the specified skip.
        /// </summary>
        /// <param name="skip">The skip.</param>
        /// <returns></returns>
        public ICursor<T> Skip(int skip)
        {
            TryModify();
            _skip = skip;
            return this;
        }

        /// <summary>
        /// Fieldses the specified fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ICursor<T> Fields(Document fields)
        {
            TryModify();
            _fields = fields;
            return this;
        }

        /// <summary>
        /// Sorts the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public ICursor<T> Sort(string field)
        {
            return Sort(field, IndexOrder.Ascending);
        }

        /// <summary>
        /// Sorts the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public ICursor<T> Sort(string field, IndexOrder order)
        {
            return Sort(new Document().Append(field, order));
        }

        /// <summary>
        /// Sorts the specified fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ICursor<T> Sort(Document fields)
        {
            TryModify();
            AddOrRemoveSpecOpt("$orderby", fields);
            return this;
        }

        /// <summary>
        /// Hints the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public ICursor<T> Hint(Document index)
        {
            TryModify();
            AddOrRemoveSpecOpt("$hint", index);
            return this;
        }

        /// <summary>
        /// Snapshots the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public ICursor<T> Snapshot(Document index)
        {
            TryModify();
            AddOrRemoveSpecOpt("$snapshot", index);
            return this;
        }

        /// <summary>
        /// Explains this instance.
        /// </summary>
        /// <returns></returns>
        public T Explain(){
            TryModify();
            _specOpts["$explain"] = true;

            var docs = Documents;
            using((IDisposable)docs){
                foreach(var doc in docs)
                    return doc;
            }
            throw new InvalidOperationException("Explain failed.");
        }

        /// <summary>
        /// Optionses the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public ICursor<T> Options(QueryOptions options)
        {
            TryModify();
            _options = options;
            return this;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Cursor&lt;T&gt;"/> is modifiable.
        /// </summary>
        /// <value><c>true</c> if modifiable; otherwise, <c>false</c>.</value>
        public bool Modifiable{
            get { return _modifiable; }
        }

        /// <summary>
        /// Gets the documents.
        /// </summary>
        /// <value>The documents.</value>
        public IEnumerable<T> Documents{
            get{
                if(_reply == null)
                    RetrieveData();
                var docsReturned = 0;
                var docs = _reply.Documents;
                var shouldBreak = false;
                while(!shouldBreak){
                    foreach(var doc in docs)
                        if((_limit == 0) || (_limit != 0 && docsReturned < _limit)){
                            docsReturned++;
                            yield return doc;
                        }
                        else
                            yield break;
                    
                    if(Id != 0){
                        RetrieveMoreData();
                        docs = _reply.Documents;
                        if(docs == null)
                            shouldBreak = true;
                    }
                    else
                        shouldBreak = true;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose(){
            if(Id == 0)//All server side resources disposed of.
                return; 
            
            var killCursorsMessage = new KillCursorsMessage(Id);
            try{
                _connection.SendMessage(killCursorsMessage);
                Id = 0;
            }
            catch(IOException ioe){
                throw new MongoCommException("Could not read data, communication failure", _connection, ioe);
            }
        }

        /// <summary>
        /// Retrieves the data.
        /// </summary>
        private void RetrieveData(){
            var query = new QueryMessage<T>{
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
                _modifiable = false;
            }
            catch(IOException ioe){
                throw new MongoCommException("Could not read data, communication failure", _connection, ioe);
            }
        }

        /// <summary>
        /// Retrieves the more data.
        /// </summary>
        private void RetrieveMoreData(){
            var getMoreMessage = new GetMoreMessage(FullCollectionName, Id, _limit);

            try{
                _reply = _connection.SendTwoWayMessage<T>(getMoreMessage);
                Id = _reply.CursorId;
            }
            catch(IOException ioe){
                Id = 0;
                throw new MongoCommException("Could not read data, communication failure", _connection, ioe);
            }
        }

        /// <summary>
        /// Tries the modify.
        /// </summary>
        private void TryModify(){
            if(_modifiable)
                return;
            throw new InvalidOperationException("Cannot modify a cursor that has already returned documents.");
        }

        /// <summary>
        /// Adds the or remove spec opt.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="doc">The doc.</param>
        private void AddOrRemoveSpecOpt(string key, Document doc){
            if(doc == null)
                _specOpts.Remove(key);
            else
                _specOpts[key] = doc;
        }

        /// <summary>
        /// Builds the spec.
        /// </summary>
        /// <returns></returns>
        private Document BuildSpec(){
            if(_specOpts.Count == 0)
                return _spec;
            
            var document = new Document();
            _specOpts.CopyTo(document);
            document["$query"] = _spec;
            return document;
        }
    }
}