using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB
{
    /// <summary>
    ///   Encapsulates and provides access to the serverside javascript stored in db.system.js.
    /// </summary>
    public class DatabaseJavascript : ICollection<Document>
    {
        private readonly IMongoCollection _collection;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DatabaseJavascript" /> class.
        /// </summary>
        /// <param name = "database">The database.</param>
        internal DatabaseJavascript(IMongoDatabase database)
        {
            _collection = database["system.js"];
            //Needed for some versions of the db to retrieve the functions.
            _collection.MetaData.CreateIndex(new Document().Add("_id", 1), true);
        }

        /// <summary>
        ///   Gets or sets the <see cref = "MongoDB.Document" /> with the specified name.
        /// </summary>
        /// <value></value>
        public Document this[String name]
        {
            get { return GetFunction(name); }
            set { Add(value); }
        }

        /// <summary>
        /// Stores a function in the database.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Add(Document item)
        {
            if(_collection.FindOne(new Document("_id", item["_id"])) != null)
                throw new ArgumentException(String.Format("Function {0} already exists in the database.", item["_id"]));
            _collection.Insert(item);
        }

        /// <summary>
        /// Removes every function in the database.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Clear()
        {
            _collection.Remove(new Document());
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(Document item)
        {
            return Contains((string)item["_id"]);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.
        /// -or-
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// -or-
        /// Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(Document[] array, int arrayIndex)
        {
            using(var cursor = _collection.FindAll().Limit(array.Length - 1).Skip(arrayIndex).Sort("_id"))
            {
                var index = arrayIndex;
                foreach(var document in cursor.Documents)
                {
                    array[index] = document;
                    index++;
                }
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public bool Remove(Document item)
        {
            return Remove((string)item["_id"]);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get
            {
                var cnt = _collection.Count();
                if(cnt > int.MaxValue)
                    return int.MaxValue; //lots of functions.
                return (int)cnt;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Document> GetEnumerator()
        {
            foreach(var doc in _collection.FindAll().Documents)
                yield return doc;
            yield break;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///   Gets the document representing the function in the database.
        /// </summary>
        /// <param name = "name">
        ///   A <see cref = "System.String" />
        /// </param>
        /// <returns>
        ///   A <see cref = "Document" />
        /// </returns>
        public Document GetFunction(string name)
        {
            return _collection.FindOne(new Document().Add("_id", name));
        }

        /// <summary>
        ///   Returns a listing of the names of all the functions in the database
        /// </summary>
        public List<string> GetFunctionNames()
        {
            var list = new List<string>();
            foreach(var document in _collection.FindAll().Documents)
                list.Add((String)document["_id"]);
            return list;
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="func">The func.</param>
        public void Add(string name, string func)
        {
            Add(name, new Code(func));
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="func">The func.</param>
        public void Add(string name, Code func)
        {
            Add(new Document().Add("_id", name).Add("value", func));
        }

        /// <summary>
        ///   Store a function in the database with an extended attribute called version.
        /// </summary>
        /// <remarks>
        ///   Version attributes are an extension to the spec.  Function names must be unique
        ///   to the database so only one version can be stored at a time.  This is most useful for libraries
        ///   that store function in the database to make sure that the function they are using is the most
        ///   up to date.
        /// </remarks>
        public void Add(string name, Code func, float version)
        {
            Add(new Document().Add("_id", name).Add("value", func).Add("version", version));
        }

        /// <summary>
        ///   Checks to see if a function named name is stored in the database.
        /// </summary>
        /// <param name = "name">
        ///   A <see cref = "System.String" />
        /// </param>
        /// <returns>
        ///   A <see cref = "System.Boolean" />
        /// </returns>
        public bool Contains(string name)
        {
            return GetFunction(name) != null;
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Update(Document item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public bool Remove(string name)
        {
            _collection.Remove(new Document().Add("_id", name));
            return true;
        }
    }
}