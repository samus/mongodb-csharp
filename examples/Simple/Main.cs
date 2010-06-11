using System;
using System.Configuration;
using MongoDB;

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
        private IMongoCollection categories;
        private Mongo mongo;
        private IMongoDatabase simple;

        public static void Main(string[] args)
        {
            var main = new MainClass();
            main.Setup();
            main.Run();
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
            categories = simple["categories"];
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