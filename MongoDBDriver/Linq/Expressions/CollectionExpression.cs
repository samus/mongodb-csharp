using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDB.Driver.Linq.Expressions
{
    internal class CollectionExpression : Expression
    {
        private readonly string _collectionName;
        private readonly IMongoDatabase _database;
        private readonly Type _documentType;

        public string CollectionName
        {
            get { return _collectionName; }
        }

        public IMongoDatabase Database
        {
            get { return _database; }
        }

        public Type DocumentType
        {
            get { return _documentType; }
        }

        public CollectionExpression(Type type, IMongoDatabase database, string collectionName, Type documentType)
            : base((ExpressionType)MongoExpressionType.Collection, type)
        {
            _collectionName = collectionName;
            _database = database;
            _documentType = documentType;
        }
    }
}
