using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.Driver
{
    public enum UpdateFlags:int
    {
        Upsert = 0,
        MultiUpdate = 1
    }    
    public interface IMongoCollection
    {
        string Name { get; }
        string DbName { get; }
        string FullName { get; }
        CollectionMetaData MetaData { get; }
        Document FindOne (Document spec);
        ICursor FindAll ();
        ICursor Find (String @where);
        ICursor Find (Document spec);
        ICursor Find (Document spec, int limit, int skip);
        ICursor Find (Document spec, int limit, int skip, Document fields);
        MapReduce MapReduce ();
        MapReduceBuilder MapReduceBuilder ();
        long Count ();
        long Count (Document spec);
        void Insert (Document doc);
        void Insert (IEnumerable<Document> docs);
        void Delete (Document selector);
        void Update (Document doc);
        void Update (Document doc, Document selector);
        void Update (Document doc, Document selector, int upsert);
        void Update (Document doc, Document selector, UpdateFlags flags);
        void UpdateAll (Document doc, Document selector);
    }
}
