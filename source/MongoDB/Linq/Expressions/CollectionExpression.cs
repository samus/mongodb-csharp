using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDB.Linq.Expressions
{
    internal class CollectionExpression : Expression
    {
        private readonly string _alias;
        private readonly string _collectionName;
        private readonly IMongoDatabase _database;
        private readonly Type _documentType;

        public string Alias
        {
            get { return _alias; }
        }

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

        public CollectionExpression(Type type, string alias, IMongoDatabase database, string collectionName, Type documentType)
            : base((ExpressionType)MongoExpressionType.Collection, type)
        {
            _alias = alias;
            _collectionName = collectionName;
            _database = database;
            _documentType = documentType;
        }
    }
}