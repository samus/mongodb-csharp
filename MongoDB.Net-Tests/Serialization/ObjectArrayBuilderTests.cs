using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver.Serialization.Builders;
using NUnit.Framework;

namespace MongoDB.Driver.Serialization
{
    [TestFixture]
    public class ObjectArrayBuilderTests
    {
        [Test]
        [ExpectedException(typeof(MongoException))]
        public void CanNotBuildNonIEnumerableObjects()
        {
            var builder = new ObjectArrayBuilder(typeof(object));
            builder.Complete();
        }

        [Test]
        [ExpectedException(typeof(MongoException))]
        public void CanNotBuildFromAnyInterface()
        {
            var builder = new ObjectArrayBuilder(typeof(IDisposable));
            builder.Complete();
        }

        [Test]
        [ExpectedException(typeof(MongoException))]
        public void CatchExceptionsWhileBuildingArrayAndThrowMongoExceptionWithTypename()
        {
            var builder = new ObjectArrayBuilder(typeof(Array));
            builder.Complete();
        }

        [Test]
        public void CanBuildArrayOfInt()
        {
            var builder = new ObjectArrayBuilder(typeof(int[]));
            var containingType1 = builder.BeginProperty(null);
            builder.EndProperty(1);
            var containingType2 = builder.BeginProperty(null);
            builder.EndProperty(2);
            var array = (int[])builder.Complete();
            Assert.IsNotNull(array);
            Assert.AreEqual(2,array.Length);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(typeof(int), containingType1);
            Assert.AreEqual(typeof(int), containingType2);
        }

        [Test]
        public void CanBuildArrayList()
        {
            var builder = new ObjectArrayBuilder(typeof(ArrayList));
            var containingType1 = builder.BeginProperty(null);
            builder.EndProperty(1);
            var containingType2 = builder.BeginProperty(null);
            builder.EndProperty(2);
            var array = (ArrayList)builder.Complete();
            Assert.IsNotNull(array);
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(typeof(object), containingType1);
            Assert.AreEqual(typeof(object), containingType2);
        }

        [Test]
        public void CanBuildListOfInt()
        {
            var builder = new ObjectArrayBuilder(typeof(List<int>));
            var containingType1 = builder.BeginProperty(null);
            builder.EndProperty(1);
            var containingType2 = builder.BeginProperty(null);
            builder.EndProperty(2);
            var array = (List<int>)builder.Complete();
            Assert.IsNotNull(array);
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(typeof(int), containingType1);
            Assert.AreEqual(typeof(int), containingType2);
        }

        [Test]
        public void CanBuildListForIEnumerable()
        {
            var builder = new ObjectArrayBuilder(typeof(IEnumerable));
            var containingType1 = builder.BeginProperty(null);
            builder.EndProperty(1);
            var containingType2 = builder.BeginProperty(null);
            builder.EndProperty(2);
            var array = new ArrayList();
            foreach(var item in (IEnumerable)builder.Complete())
                array.Add(item);
            Assert.IsNotNull(array);
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(typeof(object), containingType1);
            Assert.AreEqual(typeof(object), containingType2);
        }

        [Test]
        public void CanBuildListForIEnumerableOfInt()
        {
            var builder = new ObjectArrayBuilder(typeof(IEnumerable<int>));
            var containingType1 = builder.BeginProperty(null);
            builder.EndProperty(1);
            var containingType2 = builder.BeginProperty(null);
            builder.EndProperty(2);
            var array = new List<int>((IEnumerable<int>)builder.Complete());
            Assert.IsNotNull(array);
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(typeof(int), containingType1);
            Assert.AreEqual(typeof(int), containingType2);
        }

        [Test]
        public void CanBuildListForICollection()
        {
            var builder = new ObjectArrayBuilder(typeof(ICollection));
            var containingType1 = builder.BeginProperty(null);
            builder.EndProperty(1);
            var containingType2 = builder.BeginProperty(null);
            builder.EndProperty(2);
            var array = new ArrayList((ICollection)builder.Complete());
            Assert.IsNotNull(array);
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(typeof(object), containingType1);
            Assert.AreEqual(typeof(object), containingType2);
        }

        [Test]
        public void CanBuildListForICollectionOfInt()
        {
            var builder = new ObjectArrayBuilder(typeof(ICollection<int>));
            var containingType1 = builder.BeginProperty(null);
            builder.EndProperty(1);
            var containingType2 = builder.BeginProperty(null);
            builder.EndProperty(2);
            var array = new List<int>((ICollection<int>)builder.Complete());
            Assert.IsNotNull(array);
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(typeof(int), containingType1);
            Assert.AreEqual(typeof(int), containingType2);
        }

        [Test,Ignore("Support will be later added")]
        public void CanBuildQueue()
        {
            var builder = new ObjectArrayBuilder(typeof(Queue));
            var containingType1 = builder.BeginProperty(null);
            builder.EndProperty(1);
            var containingType2 = builder.BeginProperty(null);
            builder.EndProperty(2);
            var array = (Queue)builder.Complete();
            Assert.IsNotNull(array);
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(1, array.Dequeue());
            Assert.AreEqual(2, array.Dequeue());
            Assert.AreEqual(typeof(object), containingType1);
            Assert.AreEqual(typeof(object), containingType2);
        }

        [Test, Ignore("Support will be later added")]
        public void CanBuildQueueOfInt()
        {
            var builder = new ObjectArrayBuilder(typeof(Queue<int>));
            var containingType1 = builder.BeginProperty(null);
            builder.EndProperty(1);
            var containingType2 = builder.BeginProperty(null);
            builder.EndProperty(2);
            var array = (Queue<int>)builder.Complete();
            Assert.IsNotNull(array);
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(1, array.Dequeue());
            Assert.AreEqual(2, array.Dequeue());
            Assert.AreEqual(typeof(int), containingType1);
            Assert.AreEqual(typeof(int), containingType2);
        }

        [Test, Ignore("Support will be later added")]
        public void CanBuildStack()
        {
            var builder = new ObjectArrayBuilder(typeof(Stack));
            var containingType1 = builder.BeginProperty(null);
            builder.EndProperty(1);
            var containingType2 = builder.BeginProperty(null);
            builder.EndProperty(2);
            var array = (Stack)builder.Complete();
            Assert.IsNotNull(array);
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(2, array.Pop());
            Assert.AreEqual(1, array.Pop());
            Assert.AreEqual(typeof(object), containingType1);
            Assert.AreEqual(typeof(object), containingType2);
        }

        [Test, Ignore("Support will be later added")]
        public void CanBuildStackOfInt()
        {
            var builder = new ObjectArrayBuilder(typeof(Stack<int>));
            var containingType1 = builder.BeginProperty(null);
            builder.EndProperty(1);
            var containingType2 = builder.BeginProperty(null);
            builder.EndProperty(2);
            var array = (Stack<int>)builder.Complete();
            Assert.IsNotNull(array);
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(2, array.Pop());
            Assert.AreEqual(1, array.Pop());
            Assert.AreEqual(typeof(int), containingType1);
            Assert.AreEqual(typeof(int), containingType2);
        }
    }
}