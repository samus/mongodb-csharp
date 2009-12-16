using System;

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
		
		static int trials = 2;
		static int perTrial = 5000;
		static int batchSize = 100;
		
		static void SetupDocuments(){
			medium.Append("integer", (int) 5);
			medium.Append("number", 5.05);
			medium.Append("boolean", false);
			medium.Append("array", new String[]{"test","benchmark"});
			
			large.Append("base_url", "http://www.example.com/test-me");
         	large.Append("total_word_count", (int)6743);
         	large.Append("access_time", DateTime.UtcNow);
         	large.Append("meta_tags", new Document()
						.Append("description", "i am a long description string")
						.Append("author", "Holly Man")
						.Append("dynamically_created_meta_tag", "who know\n what"));
			large.Append("page_structure", new Document().Append("counted_tags", 3450)
						.Append("no_of_js_attached", (int)10)
						.Append("no_of_images", (int)6));
			string[] words = new string[]{"10gen","web","open","source","application","paas",
                             "platform-as-a-service","technology","helps",
                             "developers","focus","building","mongodb","mongo"};
			string[] harvestedWords = new string[words.Length * 20];
			for(int i = 0; i < words.Length * 20; i++){
				harvestedWords[i] = words[i % words.Length];
			}
			large.Append("harvested_words", harvestedWords);
		}
		
		static void SetupInsert(Database db, string col){
			try{
				db.MetaData.DropCollection(col);
			}catch(MongoCommandException mce){
				//swallow for now.
			}
		}
		
		static void TestInsert(string name, Database db, string col, Document doc){
			TimeSpan ret = TimeSpan.MaxValue;
			for(int i = 0; i < trials; i++){
				DateTime start = DateTime.Now;
				Insert(db,col,doc);
				DateTime stop = DateTime.Now;
				TimeSpan t = stop - start;
				if(t < ret){
					ret = t;
				}
			}
			
			int opsSec = (int)(perTrial/ret.TotalSeconds);
			Console.Out.WriteLine(String.Format("{0}{1} {2}", name + new string('.', 60 - name.Length), opsSec, ret));
		}
		
		static void Insert(Database db, string col, Document doc){
			for(int i = 0; i < perTrial; i++){
				Document ins = new Document();
				doc.CopyTo(ins);
				ins.Append("x", i);
				db[col].Insert(ins);
			}
		}
		static void runInsertTest(string name, Database db, string col, Document doc, bool index){
			SetupInsert(db,"col");
			if(index){
				Document idx = new Document().Append("x", IndexOrder.Ascending);
				db[name].MetaData.CreateIndex(idx,false);
			}
			TestInsert(name, db, col,doc);
		}
		
		public static void Main (string[] args)
		{
			SetupDocuments();
			
			Mongo m = new Mongo();
			m.Connect();
			Database db = m["benchmark"];
			db.MetaData.DropDatabase();
			
			runInsertTest("insert (small, no index)", db, "small_none",small,false);
			runInsertTest("insert (medium, no index)", db, "medium_none",medium,false);
			runInsertTest("insert (large, no index)", db, "large_none",large,false);
			
			runInsertTest("insert (small, indexed)", db, "small_index",small,true);
			runInsertTest("insert (medium, indexed)", db, "medium_index",medium,true);
			runInsertTest("insert (large, indexed)", db, "large_index",large,true);
			
			
		}
	}
}
