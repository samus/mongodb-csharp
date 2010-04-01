using System;
using System.Collections.Generic;

namespace MongoDB.Driver
{
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
		Document FindAndModify(Document doc, Document spec);
		Document FindAndModify(Document doc, Document spec, Document sort);
		Document FindAndModify(Document doc, Document spec, bool returnNewDoc);
		Document FindAndModify(Document doc, Document spec, Document sort, bool returnNewDoc);
        MapReduce MapReduce ();
        MapReduceBuilder MapReduceBuilder ();
        long Count ();
        long Count (Document spec);
        void Insert (Document doc);
        void Insert (Document doc, bool safemode);
        void Insert (IEnumerable<Document> docs);
        void Insert (IEnumerable<Document> docs, bool safemode);        
        void Delete (Document selector);
        void Delete (Document selector, bool safemode);
        void Update (Document doc);
        void Update (Document doc, Document selector);
        void Update (Document doc, Document selector, int upsert);
        void Update (Document doc, Document selector, UpdateFlags flags);
        void Update (Document doc, bool safemode);
        void Update (Document doc, Document selector, bool safemode);
        void Update (Document doc, Document selector, int upsert, bool safemode);
        void Update (Document doc, Document selector, UpdateFlags flags, bool safemode);
        void UpdateAll (Document doc, Document selector);
        void UpdateAll (Document doc, Document selector, bool safemode);
    }
}
