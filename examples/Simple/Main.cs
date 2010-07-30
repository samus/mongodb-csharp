using System;
using System.Configuration;
using System.Linq;
using MongoDB;
using MongoDB.Configuration;
using MongoDB.Linq;

namespace Simple
{
    /// <summary>
    ///   Illustrates some simple operations on the database.
    ///   Creating a database connection.
    ///   Remove some documents.
    ///   Insert some documents
    ///   Find one document
    ///   Find several documents and iterate through them.
    /// </summary>
    internal class MainClass
    {
        private IMongoCollection<Document> categories;
        private Mongo mongo;
        private IMongoDatabase simple;

        private class MyClass
        {
            public string Name { get; set; }
            public  int Corners { get; set; }
        }

        private class SubClass : MyClass
        {
            public double Ratio { get; set; }
        }

        public static void Main(string[] args)
        {
            var config = new MongoConfigurationBuilder();

            // COMMENT OUT FROM HERE
            config.Mapping(mapping =>
            {
                mapping.DefaultProfile(profile =>
                {
                    profile.SubClassesAre(t => t.IsSubclassOf(typeof(MyClass)));
                });
                mapping.Map<MyClass>();
                mapping.Map<SubClass>();
            });
            // TO HERE

            config.ConnectionString("Server=127.0.0.1");

            using (Mongo mongo = new Mongo(config.BuildConfiguration()))
            {
                mongo.Connect();
                try
                {
                    var db = mongo.GetDatabase("TestDb");
                    var collection = db.GetCollection<MyClass>();

                    MyClass square = new MyClass()
                    {
                        Corners = 4,
                        Name = "Square"
                    };

                    MyClass circle = new MyClass()
                    {
                        Corners = 0,
                        Name = "Circle"
                    };

                    SubClass sub = new SubClass()
                    {
                        Name = "SubClass",
                        Corners = 6,
                        Ratio = 3.43
                    };

                    collection.Save(square);
                    collection.Save(circle);
                    collection.Save(sub);

                    var superclass = (from item in db.GetCollection<MyClass>("MyClass").Linq()
                                where item.Corners > 1
                                select item.Corners).ToList();

                    var subclass = (from item in db.GetCollection<SubClass>("MyClass").Linq()
                                    where item.Ratio > 1
                                    select item.Corners).ToList();

                    Console.WriteLine("Count by LINQ on typed collection: {0}", collection.Linq().Count(x => x.Corners > 1));
                    Console.WriteLine("Count by LINQ on typed collection2: {0}", db.GetCollection<SubClass>().Linq().Count(x => x.Corners > 1));
                    //Console.WriteLine("Count by LINQ on typed collection3: {0}", db.GetCollection<SubClass>().Count(new { Corners = Op.GreaterThan(1) }));
                    Console.WriteLine("Count on typed collection: {0}", collection.Count(new { Corners = Op.GreaterThan(1) }));

                    var coll = db.GetCollection("MyClass");
                    var count = coll.Count(new Document("Corners", Op.GreaterThan(1)));
                    Console.WriteLine("Count: {0}", count);
                    Console.ReadKey();
                }
                finally
                {
                    mongo.Disconnect();
                }
            }

            //var main = new MainClass();
            //main.Setup();
            //main.Run();
        }

        /// <summary>
        ///   Setup the collection and insert some data into it.
        /// </summary>
        public void Setup()
        {
            var connstr = ConfigurationManager.AppSettings["simple"];
            if(String.IsNullOrEmpty(connstr))
                throw new ArgumentNullException("Connection string not found.");
            mongo = new Mongo(connstr);
            mongo.Connect();
            simple = mongo["simple"];
            categories = simple.GetCollection<Document>("categories");
            Clean();

            var names = new[] {"Bluez", "Jazz", "Classical", "Rock", "Oldies", "Heavy Metal"};
            foreach(var name in names)
                categories.Insert(new Document {{"name", name}});
        }

        public void Clean()
        {
            categories.Remove(new Document {{"name", "Jazz"}}); //remove documents with the name Jazz.
            categories.Remove(new Document()); //remove everything from the categories collection.
        }

        public void Run()
        {
            var category = categories.FindOne(new Document {{"name", "Bluez"}});

            Console.WriteLine("The id findOne" + category["_id"]);

            var selector = new Document {{"_id", category["_id"]}};

            category["name"] = "Bluess";
            //The following will do the same thing.
            categories.Save(category);

            Console.WriteLine("Category after one update " + categories.FindOne(selector));

            category["name"] = "Blues";
            categories.Update(category, selector);

            Console.WriteLine("Category after two updates " + categories.FindOne(selector));

            //Find it by _id that has been converted to a string now.
            var id = (category["_id"]).ToString();

            Console.WriteLine("Found by string id converted back to Oid");
            Console.WriteLine(categories.FindOne(new Document {{"_id", id.ToOid()}}));

            //Find(new Document()) is equivalent to FindAll();
            //Specifying the cursor in a using block will close it on the server if we decide not
            //to iterate through the whole thing.
            using(var all = categories.Find(new Document()))
            {
                foreach(var doc in all.Documents)
                    Console.WriteLine(doc.ToString());
            }

            mongo.Disconnect();
        }
    }
}

public static class OidExtensions
{
    public static Oid ToOid(this string str)
    {
        if(str.Length == 24)
            return new Oid(str);

        return new Oid(str.Replace("\"", ""));
    }
}