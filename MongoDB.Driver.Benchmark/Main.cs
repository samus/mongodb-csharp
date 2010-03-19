using System;
using System.IO;
using System.Threading;

using MongoDB.Driver;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Benchmark
{
    /// <summary>
    /// This is the standard 10gen benchmark program.
    /// </summary>
    class MainClass
    {
        static Document small = new Document();
        static Document medium = new Document();
        static Document large = new Document();

        static int trials = 1;
        static int perTrial = 5000;
        static int batchSize = 100;

        public static void Main (string[] args)
        {
            SetupDocuments();

            Mongo m = new Mongo();
            m.Connect();
            MongoDatabase db = m["benchmark"];

            db.MetaData.DropDatabase();
            Console.WriteLine("Starting Tests");
            
            RunEncodeTest("encode (small)",small);
            RunEncodeTest("encode (medium)", medium);
            RunEncodeTest("encode (large)", large);
            
            RunDecodeTest("decode (small)",small);
            RunDecodeTest("decode (medium)", medium);
            RunDecodeTest("decode (large)", large);
            
            db.MetaData.DropDatabase();
            RunInsertTest("insert (small, no index)", db, "small_none",small,false,false);
            RunInsertTest("insert (medium, no index)", db, "medium_none",medium,false,false);
            RunInsertTest("insert (large, no index)", db, "large_none",large,false,false);

            RunInsertTest("insert (small, indexed)", db, "small_index",small,true,false);
            RunInsertTest("insert (medium, indexed)", db, "medium_index",medium,true,false);
            RunInsertTest("insert (large, indexed)", db, "large_index",large,true,false);

            RunInsertTest("batch insert (small, no index)", db, "small_bulk",small,false,true);
            RunInsertTest("batch insert (medium, no index)", db, "medium_bulk",medium,false,true);
            RunInsertTest("batch insert (large, no index)", db, "large_bulk",large,false,true);            
            
            Document fonespec = new Document().Add("x",perTrial/2);
            RunFindTest("find_one (small, no index)", db, "small_none",fonespec,false);
            RunFindTest("find_one (medium, no index)", db, "medium_none",fonespec,false);
            RunFindTest("find_one (large, no index)", db, "large_none",fonespec,false);

            RunFindTest("find_one (small, indexed)", db, "small_index",fonespec,false);
            RunFindTest("find_one (medium, indexed)", db, "medium_index",fonespec,false);
            RunFindTest("find_one (large, indexed)", db, "large_index",fonespec,false);

            RunFindTest("find (small, no index)", db, "small_none",fonespec,true);
            RunFindTest("find (medium, no index)", db, "medium_none",fonespec,true);
            RunFindTest("find (large, no index)", db, "large_none",fonespec,true);

            RunFindTest("find (small, indexed)", db, "small_index",fonespec,true);
            RunFindTest("find (medium, indexed)", db, "medium_index",fonespec,true);
            RunFindTest("find (large, indexed)", db, "large_index",fonespec,true);

            Document findRange = new Document().Add("x",new Document().Add("$gt",perTrial/2).Add("$lt", perTrial/2 + batchSize));
            RunFindTest("find range (small, indexed)", db, "small_index",findRange,true);
            RunFindTest("find range (medium, indexed)", db, "medium_index",findRange,true);
            RunFindTest("find range (large, indexed)", db, "large_index",findRange,true);

            System.Console.WriteLine("Press any key to continue...");
            System.Console.ReadKey();
        }

        static void SetupDocuments(){
            medium.Add("integer", (int) 5);
            medium.Add("number", 5.05);
            medium.Add("boolean", false);
            medium.Add("array", new String[]{"test","benchmark"});

            large.Add("base_url", "http://www.example.com/test-me");
            large.Add("total_word_count", (int)6743);
            large.Add("access_time", DateTime.UtcNow);
            large.Add("meta_tags", new Document()
                         .Add("description", "i am a long description string")
                         .Add("author", "Holly Man")
                         .Add("dynamically_created_meta_tag", "who know\n what"));
            large.Add("page_structure", new Document().Add("counted_tags", 3450)
                         .Add("no_of_js_attached", (int)10)
                         .Add("no_of_images", (int)6));
            string[] words = new string[]{"10gen","web","open","source","application","paas",
                "platform-as-a-service","technology","helps",
                "developers","focus","building","mongodb","mongo"};
            string[] harvestedWords = new string[words.Length * 20];
            for(int i = 0; i < words.Length * 20; i++){
                harvestedWords[i] = words[i % words.Length];
            }
            large.Add("harvested_words", harvestedWords);
        }
#region Insert Tests
        static void RunInsertTest(string name, MongoDatabase db, string col, Document doc, bool index, bool bulk){
            TimeSpan lowest = TimeSpan.MaxValue;
            for(int i = 0; i < trials; i++){
                SetupInsert(db,"col",index);
                TimeSpan ret = TimeInsert(db, col,doc, bulk);
                if(ret < lowest) lowest = ret;
            }
            int opsSec = (int)(perTrial/lowest.TotalSeconds);
            Console.Out.WriteLine(String.Format("{0}{1} {2}", name + new string('.', 55 - name.Length), opsSec, lowest));
        }

        static void SetupInsert(MongoDatabase db, string col, bool index){
            try{
                db.MetaData.DropCollection(col);
                if(index){
                    Document idx = new Document().Add("x", IndexOrder.Ascending);
                    db[col].MetaData.CreateIndex(idx,false);
                }
            }catch(MongoCommandException){
                //swallow for now.
            }
        }

        static TimeSpan TimeInsert(MongoDatabase db, string col, Document doc, bool bulk){
            DateTime start = DateTime.Now;
            if(bulk){
                DoBulkInsert(db,col,doc, batchSize);
            }else{
                DoInsert(db,col,doc);
            }
            DateTime stop = DateTime.Now;
            TimeSpan t = stop - start;
            return t;
        }

        static void DoInsert(MongoDatabase db, string col, Document doc){
            for(int i = 0; i < perTrial; i++){
                Document ins = new Document();
                doc.CopyTo(ins);
                ins.Add("x", i);
                db[col].Insert(ins);
            }
        }
        
        static void DoBulkInsert(MongoDatabase db, string col, Document doc, int size){
            for(int i = 0; i < perTrial / size; i++){
                Document[] docs = new Document[size];
                for(int f = 0; f < docs.Length; f++){
                    Document ins = new Document();
                    doc.CopyTo(ins);
                    docs[f] = ins;
                }
                db[col].Insert(docs);
            }
        }
#endregion

#region Encode Tests
        static void RunEncodeTest(string name, Document doc){
            TimeSpan lowest = TimeSpan.MaxValue;
            for(int i = 0; i < trials; i++){
                TimeSpan ret = TimeEncode(doc);
                if(ret < lowest) lowest = ret;
            }
            int opsSec = (int)(perTrial/lowest.TotalSeconds);
            Console.Out.WriteLine(String.Format("{0}{1} {2}", name + new string('.', 55 - name.Length), opsSec, lowest));            
        }
        
        static TimeSpan TimeEncode(Document doc){
            DateTime start = DateTime.Now;
            DoEncode(doc);
            DateTime stop = DateTime.Now;
            TimeSpan t = stop - start;
            return t;
        }

        static void DoEncode(Document doc){
            MemoryStream ms = new MemoryStream();
            for(int i = 0; i < perTrial; i++){
                BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
                writer.WriteObject(doc);
                ms.Seek(0,SeekOrigin.Begin);
            }
        }
#endregion        

        static void RunDecodeTest(string name, Document doc){
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            writer.WriteObject(doc);
    
            byte[] buff = ms.ToArray();
            
            TimeSpan lowest = TimeSpan.MaxValue;
            for(int i = 0; i < trials; i++){
                TimeSpan ret = TimeDecode(buff);
                if(ret < lowest) lowest = ret;
            }
            int opsSec = (int)(perTrial/lowest.TotalSeconds);
            Console.Out.WriteLine(String.Format("{0}{1} {2}", name + new string('.', 55 - name.Length), opsSec, lowest));            
        }
        
        static TimeSpan TimeDecode(byte[] doc){
            DateTime start = DateTime.Now;
            DoDecode(doc);
            DateTime stop = DateTime.Now;
            TimeSpan t = stop - start;
            return t;
        }

        static void DoDecode(byte[] buff){
            MemoryStream ms = new MemoryStream(buff);
            for(int i = 0; i < perTrial; i++){
                BsonReader reader = new BsonReader(ms,new BsonDocumentBuilder());
                reader.Read();
                ms.Seek(0,SeekOrigin.Begin);
            }
        }

        #region Find Tests
        static void RunFindTest(string name, MongoDatabase db, string col, Document spec, bool range){
            TimeSpan lowest = TimeSpan.MaxValue;
            for(int i = 0; i < trials; i++){
                TimeSpan ret = TimeFind(db, col, spec, range);
                if(ret < lowest) lowest = ret;
            }
            int opsSec = (int)(perTrial/lowest.TotalSeconds);
            Console.Out.WriteLine(String.Format("{0}{1} {2}", name + new string('.', 55 - name.Length), opsSec, lowest));
        }

        static TimeSpan TimeFind(MongoDatabase db, string col,Document psec, bool range){
            DateTime start = DateTime.Now;
            if(range){
                DoFindOne(db,col,psec);
            }else{
                DoFind(db,col,psec);
            }
            DateTime stop = DateTime.Now;
            TimeSpan t = stop - start;
            return t;
        }

        static void DoFindOne(MongoDatabase db, string col, Document spec){
            for(int i = 0; i < perTrial; i++){
                db[col].FindOne(spec);
            }
        }

        static void DoFind(MongoDatabase db, string col, Document spec){
            for(int i = 0; i < perTrial; i++){
                ICursor<Document> cur = db[col].Find(spec);
                foreach(Document d in cur.Documents){
                }
            }
        }
        #endregion

    }
}