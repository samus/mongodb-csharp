using System;
using System.IO;
using MongoDB.Bson;

namespace MongoDB.Driver.Benchmark
{
    /// <summary>
    ///   This is the standard 10gen benchmark program.
    /// </summary>
    internal class MainClass
    {
        private static readonly Document large = new Document();
        private static readonly Document medium = new Document();
        private static readonly Document small = new Document();

        private static int batchSize = 100;
        private static int perTrial = 5000;
        private static int trials = 1;

        public static void Main(string[] args)
        {
            SetupDocuments();

            var m = new Mongo();
            m.Connect();
            var db = m["benchmark"];

            db.Metadata.DropDatabase();
            Console.WriteLine("Starting Tests");

            RunEncodeTest("encode (small)", small);
            RunEncodeTest("encode (medium)", medium);
            RunEncodeTest("encode (large)", large);

            RunDecodeTest("decode (small)", small);
            RunDecodeTest("decode (medium)", medium);
            RunDecodeTest("decode (large)", large);

            db.Metadata.DropDatabase();
            RunInsertTest("insert (small, no index)", db, "small_none", small, false, false);
            RunInsertTest("insert (medium, no index)", db, "medium_none", medium, false, false);
            RunInsertTest("insert (large, no index)", db, "large_none", large, false, false);

            RunInsertTest("insert (small, indexed)", db, "small_index", small, true, false);
            RunInsertTest("insert (medium, indexed)", db, "medium_index", medium, true, false);
            RunInsertTest("insert (large, indexed)", db, "large_index", large, true, false);

            RunInsertTest("batch insert (small, no index)", db, "small_bulk", small, false, true);
            RunInsertTest("batch insert (medium, no index)", db, "medium_bulk", medium, false, true);
            RunInsertTest("batch insert (large, no index)", db, "large_bulk", large, false, true);

            var fonespec = new Document().Add("x", perTrial/2);
            RunFindTest("find_one (small, no index)", db, "small_none", fonespec, false);
            RunFindTest("find_one (medium, no index)", db, "medium_none", fonespec, false);
            RunFindTest("find_one (large, no index)", db, "large_none", fonespec, false);

            RunFindTest("find_one (small, indexed)", db, "small_index", fonespec, false);
            RunFindTest("find_one (medium, indexed)", db, "medium_index", fonespec, false);
            RunFindTest("find_one (large, indexed)", db, "large_index", fonespec, false);

            RunFindTest("find (small, no index)", db, "small_none", fonespec, true);
            RunFindTest("find (medium, no index)", db, "medium_none", fonespec, true);
            RunFindTest("find (large, no index)", db, "large_none", fonespec, true);

            RunFindTest("find (small, indexed)", db, "small_index", fonespec, true);
            RunFindTest("find (medium, indexed)", db, "medium_index", fonespec, true);
            RunFindTest("find (large, indexed)", db, "large_index", fonespec, true);

            var findRange = new Document().Add("x", new Document().Add("$gt", perTrial/2).Add("$lt", perTrial/2 + batchSize));
            RunFindTest("find range (small, indexed)", db, "small_index", findRange, true);
            RunFindTest("find range (medium, indexed)", db, "medium_index", findRange, true);
            RunFindTest("find range (large, indexed)", db, "large_index", findRange, true);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void SetupDocuments()
        {
            medium.Add("integer", 5);
            medium.Add("number", 5.05);
            medium.Add("boolean", false);
            medium.Add("array", new[] {"test", "benchmark"});

            large.Add("base_url", "http://www.example.com/test-me");
            large.Add("total_word_count", 6743);
            large.Add("access_time", DateTime.UtcNow);
            large.Add("meta_tags",
                new Document()
                    .Add("description", "i am a long description string")
                    .Add("author", "Holly Man")
                    .Add("dynamically_created_meta_tag", "who know\n what"));
            large.Add("page_structure",
                new Document().Add("counted_tags", 3450)
                    .Add("no_of_js_attached", 10)
                    .Add("no_of_images", 6));
            var words = new[]
            {
                "10gen", "web", "open", "source", "application", "paas",
                "platform-as-a-service", "technology", "helps",
                "developers", "focus", "building", "mongodb", "mongo"
            };
            var harvestedWords = new string[words.Length*20];
            for(var i = 0; i < words.Length*20; i++)
                harvestedWords[i] = words[i%words.Length];
            large.Add("harvested_words", harvestedWords);
        }

        private static void RunInsertTest(string name, IMongoDatabase db, string col, Document doc, bool index, bool bulk)
        {
            var lowest = TimeSpan.MaxValue;
            for(var i = 0; i < trials; i++)
            {
                SetupInsert(db, "col", index);
                var ret = TimeInsert(db, col, doc, bulk);
                if(ret < lowest)
                    lowest = ret;
            }
            var opsSec = (int)(perTrial/lowest.TotalSeconds);
            Console.Out.WriteLine(String.Format("{0}{1} {2}", name + new string('.', 55 - name.Length), opsSec, lowest));
        }

        private static void SetupInsert(IMongoDatabase db, string col, bool index)
        {
            try
            {
                db.Metadata.DropCollection(col);
                if(index)
                {
                    var idx = new Document().Add("x", IndexOrder.Ascending);
                    db[col].MetaData.CreateIndex(idx, false);
                }
            }
            catch(MongoCommandException)
            {
                //swallow for now.
            }
        }

        private static TimeSpan TimeInsert(IMongoDatabase db, string col, Document doc, bool bulk)
        {
            var start = DateTime.Now;
            if(bulk)
                DoBulkInsert(db, col, doc, batchSize);
            else
                DoInsert(db, col, doc);
            var stop = DateTime.Now;
            var t = stop - start;
            return t;
        }

        private static void DoInsert(IMongoDatabase db, string col, Document doc)
        {
            for(var i = 0; i < perTrial; i++)
            {
                var ins = new Document();
                doc.CopyTo(ins);
                ins.Add("x", i);
                db[col].Insert(ins);
            }
        }

        private static void DoBulkInsert(IMongoDatabase db, string col, Document doc, int size)
        {
            for(var i = 0; i < perTrial/size; i++)
            {
                var docs = new Document[size];
                for(var f = 0; f < docs.Length; f++)
                {
                    var ins = new Document();
                    doc.CopyTo(ins);
                    docs[f] = ins;
                }
                db[col].Insert(docs);
            }
        }

        private static void RunEncodeTest(string name, Document doc)
        {
            var lowest = TimeSpan.MaxValue;
            for(var i = 0; i < trials; i++)
            {
                var ret = TimeEncode(doc);
                if(ret < lowest)
                    lowest = ret;
            }
            var opsSec = (int)(perTrial/lowest.TotalSeconds);
            Console.Out.WriteLine(String.Format("{0}{1} {2}", name + new string('.', 55 - name.Length), opsSec, lowest));
        }

        private static TimeSpan TimeEncode(Document doc)
        {
            var start = DateTime.Now;
            DoEncode(doc);
            var stop = DateTime.Now;
            var t = stop - start;
            return t;
        }

        private static void DoEncode(Document doc)
        {
            var ms = new MemoryStream();
            for(var i = 0; i < perTrial; i++)
            {
                var writer = new BsonWriter(ms, new BsonDocumentDescriptor());
                writer.WriteObject(doc);
                ms.Seek(0, SeekOrigin.Begin);
            }
        }

        private static void RunDecodeTest(string name, Document doc)
        {
            var ms = new MemoryStream();
            var writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            writer.WriteObject(doc);

            var buff = ms.ToArray();

            var lowest = TimeSpan.MaxValue;
            for(var i = 0; i < trials; i++)
            {
                var ret = TimeDecode(buff);
                if(ret < lowest)
                    lowest = ret;
            }
            var opsSec = (int)(perTrial/lowest.TotalSeconds);
            Console.Out.WriteLine(String.Format("{0}{1} {2}", name + new string('.', 55 - name.Length), opsSec, lowest));
        }

        private static TimeSpan TimeDecode(byte[] doc)
        {
            var start = DateTime.Now;
            DoDecode(doc);
            var stop = DateTime.Now;
            var t = stop - start;
            return t;
        }

        private static void DoDecode(byte[] buff)
        {
            var ms = new MemoryStream(buff);
            for(var i = 0; i < perTrial; i++)
            {
                var reader = new BsonReader(ms, new BsonDocumentBuilder());
                reader.Read();
                ms.Seek(0, SeekOrigin.Begin);
            }
        }

        private static void RunFindTest(string name, IMongoDatabase db, string col, Document spec, bool range)
        {
            var lowest = TimeSpan.MaxValue;
            for(var i = 0; i < trials; i++)
            {
                var ret = TimeFind(db, col, spec, range);
                if(ret < lowest)
                    lowest = ret;
            }
            var opsSec = (int)(perTrial/lowest.TotalSeconds);
            Console.Out.WriteLine(String.Format("{0}{1} {2}", name + new string('.', 55 - name.Length), opsSec, lowest));
        }

        private static TimeSpan TimeFind(IMongoDatabase db, string col, Document psec, bool range)
        {
            var start = DateTime.Now;
            if(range)
                DoFindOne(db, col, psec);
            else
                DoFind(db, col, psec);
            var stop = DateTime.Now;
            var t = stop - start;
            return t;
        }

        private static void DoFindOne(IMongoDatabase db, string col, Document spec)
        {
            for(var i = 0; i < perTrial; i++)
                db[col].FindOne(spec);
        }

        private static void DoFind(IMongoDatabase db, string col, Document spec)
        {
            for(var i = 0; i < perTrial; i++)
            {
                var cur = db[col].Find(spec);
                foreach(var d in cur.Documents)
                {
                }
            }
        }
    }
}