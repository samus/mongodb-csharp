using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace MongoDB.Driver.Serialization
{
    [TestFixture]
    public class ArrayFactoryTests
    {
        readonly ArrayFactory _factory = new ArrayFactory();

        [Test]
        [ExpectedException(typeof(MongoException))]
        public void CanNotCreateNonIEnumerableObjects(){
            Type containingType;
            _factory.Create(typeof(object), out containingType);
        }

        [Test]
        [ExpectedException(typeof(MongoException))]
        public void CanNotCreateObjectsForAnyInterface()
        {
            Type containingType;
            _factory.Create(typeof(IDisposable), out containingType);
        }

        [Test]
        [ExpectedException(typeof(MongoException))]
        public void CatchExceptionsWhileCreatingArrayAndThrowMongoExceptionWithTypename()
        {
            Type containingType;
            _factory.Create(typeof(Array), out containingType);
        }

        [Test]
        public void CanCreateArrayOfInt(){
            Type containingType;
            var instance = _factory.Create(typeof(int[]), out containingType);
            Assert.IsInstanceOfType(typeof(int[]), instance);
            Assert.AreEqual(typeof(int), containingType);
        }

        [Test]
        public void CanCreateArrayList(){
            Type containingType;
            var instance = _factory.Create(typeof(ArrayList), out containingType);
            Assert.IsInstanceOfType(typeof(ArrayList),instance);
            Assert.AreEqual(typeof(object),containingType);
        }

        [Test]
        public void CanCreateListOfInt(){
            Type containingType;
            var instance = _factory.Create(typeof(List<int>), out containingType);
            Assert.IsInstanceOfType(typeof(List<int>), instance);
            Assert.AreEqual(typeof(int), containingType);
        }

        [Test]
        public void CanCreateListForIEnumerable(){
            Type containingType;
            var instance = _factory.Create(typeof(IEnumerable), out containingType);
            Assert.IsInstanceOfType(typeof(List<object>), instance);
            Assert.AreEqual(typeof(object), containingType);
        }

        [Test]
        public void CanCreateListForIEnumerableOfInt(){
            Type containingType;
            var instance = _factory.Create(typeof(IEnumerable<int>), out containingType);
            Assert.IsInstanceOfType(typeof(List<int>), instance);
            Assert.AreEqual(typeof(int), containingType);
        }

        [Test]
        public void CanCreateListForICollection(){
            Type containingType;
            var instance = _factory.Create(typeof(ICollection), out containingType);
            Assert.IsInstanceOfType(typeof(List<object>), instance);
            Assert.AreEqual(typeof(object), containingType);
        }

        [Test]
        public void CanCreateListForICollectionOfType()
        {
            Type containingType;
            var instance = _factory.Create(typeof(ICollection<int>), out containingType);
            Assert.IsInstanceOfType(typeof(List<int>), instance);
            Assert.AreEqual(typeof(int), containingType);
        }

        [Test]
        public void CanCreateQueue()
        {
            Type containingType;
            var instance = _factory.Create(typeof(Queue), out containingType);
            Assert.IsInstanceOfType(typeof(Queue), instance);
            Assert.AreEqual(typeof(object), containingType);
        }

        [Test]
        public void CanCreateQueueOfInt()
        {
            Type containingType;
            var instance = _factory.Create(typeof(Queue<int>), out containingType);
            Assert.IsInstanceOfType(typeof(Queue<int>), instance);
            Assert.AreEqual(typeof(int), containingType);
        }

        [Test]
        public void CanCreateStack()
        {
            Type containingType;
            var instance = _factory.Create(typeof(Stack), out containingType);
            Assert.IsInstanceOfType(typeof(Stack), instance);
            Assert.AreEqual(typeof(object), containingType);
        }

        [Test]
        public void CanCreateStackOfInt()
        {
            Type containingType;
            var instance = _factory.Create(typeof(Stack<int>), out containingType);
            Assert.IsInstanceOfType(typeof(Stack<int>), instance);
            Assert.AreEqual(typeof(int), containingType);
        }
    }
}