using System;
using System.Collections.Generic;
using System.Threading;

using NUnit.Framework;

namespace MongoDB.Driver
{
    
    [TestFixture()]
    public class TestConcurrency :MongoTestBase
    {
        /*
         * Having all of these tests enabled will slow the test suite down a lot.  In the future it may be better
         * to have them ifdef'ed so that the long running tests can be executed by just setting a compile flag.
         */
        Mongo db = new Mongo();
        public override string TestCollections {
            get {
                return "threadinserts,threadreadinserts,threadsmallreads";
            }
        }
        
        public override void OnInit (){
            var col = (IMongoCollection)DB["threadsmallreads"];
            for(int j = 0; j < 4; j++){
                col.Insert(new Document(){{"x", 4},{"j", j}});
            }
        }


        //[Test]
        public void TestMultiThreadedWrites (){
            Mongo db = new Mongo();
            db.Connect();

            IMongoCollection col = DB["threadinserts"];
            
            List<string> identifiers = new List<string>{"A", "B", "C", "D"};
            List<Thread> threads = new List<Thread>();
            int iterations = 100;
            
            foreach(string identifier in identifiers){
                Inserter ins = new Inserter {Iterations = iterations, Identifier = identifier, Collection = col};
                ThreadStart ts = new ThreadStart(ins.DoInserts);
                Thread thread = new Thread(ts);                
                threads.Add(thread);
            }
            
            RunAndWait(threads);
            
            try{
                Assert.AreEqual(identifiers.Count * iterations, col.Count());
            }catch(Exception e){
                Assert.Fail(e.Message);
            }
        }

        //[Test]
        public void TestMultiThreadedReads(){
            Mongo db = new Mongo();
            db.Connect();               
            
            List<string> colnames = new List<string>{"threadsmallreads", "threadsmallreads",
                                                    "threadsmallreads", "threadsmallreads"};
            List<Thread> threads = new List<Thread>();
            List<Reader> readers = new List<Reader>();
            int iterations = 50;
            foreach(string colname in colnames){
                Reader r = new Reader{Iterations = iterations, Collection = DB[colname]};
                readers.Add(r);
                ThreadStart ts = new ThreadStart(r.DoReads);
                Thread thread = new Thread(ts);                
                threads.Add(thread);
            }
            RunAndWait(threads);
            
            try{
                //Connection still alive?
                DB["smallreads"].Count();
            }catch(Exception e){
                Assert.Fail(e.Message);
            }
            foreach(Reader r in readers){
                Assert.AreEqual(iterations, r.Count, "A reader did not read everytime.");
            }
        }
        
        [Test]
        public void TestMultiThreadedReadsAndWrites(){
            Mongo db = new Mongo();
            db.Connect();

            IMongoCollection col = DB["threadreadinserts"];
            
            List<string> identifiers = new List<string>{"A", "B", "C", "D"};
            List<string> colnames = new List<string>{"threadsmallreads", "threadsmallreads",
                                                    "threadsmallreads", "threadsmallreads"};
            List<Thread> threads = new List<Thread>();
            List<Reader> readers = new List<Reader>();
            int writeiterations = 100;
            int readiterations = 50;
            foreach(string identifier in identifiers){
                Inserter ins = new Inserter {Iterations = writeiterations, Identifier = identifier, Collection = col};
                ThreadStart ts = new ThreadStart(ins.DoInserts);
                Thread thread = new Thread(ts);                
                threads.Add(thread);
            }            
            foreach(string colname in colnames){
                Reader r = new Reader{Iterations = readiterations, Collection = DB[colname]};
                readers.Add(r);
                ThreadStart ts = new ThreadStart(r.DoReads);
                Thread thread = new Thread(ts);                
                threads.Add(thread);
            }            
            
            RunAndWait(threads);
            try{
                Assert.AreEqual(identifiers.Count * writeiterations, col.Count());
            }catch(Exception e){
                Assert.Fail(e.Message);
            }
            foreach(Reader r in readers){
                Assert.AreEqual(readiterations, r.Count, "A reader did not read everytime.");
            }
        }
                
        protected void RunAndWait(List<Thread> threads){
           foreach(Thread t in threads){
                t.Start();
            }
            //wait for the threads to finish.
            bool running = true;
            while(running){
                running = false;
                foreach(Thread t in threads){
                    if(t.ThreadState != ThreadState.Stopped){
                        running = true;
                        break;
                    }
                }                   
            }            
        }       
    }
    
    public class Inserter{
        public int Iterations{get; set;}
        public int Count{get;set;}
        public String Identifier{get; set;}
        public IMongoCollection Collection { get; set; }
        
        public void DoInserts(){
            for(int x = 0; x < this.Iterations; x++){
                try{
                    Document doc = new Document(){{"x",x},{"identifier", this.Identifier}};
                    this.Collection.Insert(doc);
                    this.Count++;
                }catch(Exception){
                    break;
                }
            }
        }
    }
    
    public class Reader{
        public int Iterations{get; set;}
        public int Count{get;set;}
        public IMongoCollection Collection { get; set; }
        
        public void DoReads(){
            for(int x = 0; x < this.Iterations; x++){
                try{
                    using(ICursor c = this.Collection.FindAll())
                    {
                        //Just read one and do nothing with the Document.
                        foreach(Document d in c.Documents){
                            d["works"] = true;
                            break;
                        }
                    }
                    this.Count++;
                }catch(Exception){
                    break;
                }
            }
        }        
    }

}
