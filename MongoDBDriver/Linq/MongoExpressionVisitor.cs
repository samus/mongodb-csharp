using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace MongoDB.Driver.Linq
{

    internal class MongoExpressionVisitor : ExpressionVisitor
    {
        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
                return null;
            switch ((MongoExpressionType)exp.NodeType)
            {
                case MongoExpressionType.Collection:
                    return VisitCollection((CollectionExpression)exp);
                case MongoExpressionType.Field:
                    return VisitField((FieldExpression)exp);
                case MongoExpressionType.Projection:
                    return VisitProjection((ProjectionExpression)exp);
                case MongoExpressionType.Select:
                    return VisitSelect((SelectExpression)exp);
                default:
                return base.Visit(exp);
            }
        }

        protected virtual Expression VisitCollection(CollectionExpression c)
        {
            return c;
        }

        protected virtual Expression VisitField(FieldExpression f)
        {
            return f;
        }

        protected virtual Expression VisitProjection(ProjectionExpression p)
        {
            var source = (SelectExpression)Visit(p.Source);
            var projector = Visit(p.Projector);
            if (source != p.Source || projector != p.Projector)
                return new ProjectionExpression(source, projector);
            return p;
        }

        protected virtual Expression VisitSelect(SelectExpression s)
        {
            var from = VisitSource(s.From);
            var where = Visit(s.Where);
            var fields = VisitFieldDeclarations(s.Fields);
            if (from != s.From || where != s.Where || fields != s.Fields)
                return new SelectExpression(s.Type, fields, from, where);
            return s;
        }

        protected virtual Expression VisitSource(Expression source)
        {
            return Visit(source);
        }

        protected ReadOnlyCollection<FieldDeclaration> VisitFieldDeclarations(ReadOnlyCollection<FieldDeclaration> fields)
        {
            List<FieldDeclaration> alternate = null;
            for (int i = 0, n = fields.Count; i < n; i++)
            {
                var field = fields[i];
                var e = Visit(field.Expression);
                if (alternate == null && e != field.Expression)
                    alternate = fields.Take(i).ToList();
                if (alternate != null)
                    alternate.Add(new FieldDeclaration(field.Name, e));
            }
            if (alternate != null)
                return alternate.AsReadOnly();
            return fields;
        }
    }

    internal enum MongoExpressionType
    {
        Collection = 1000,
        Field,
        Select,
        Projection
    }

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

    internal class FieldExpression : Expression
    {
        private readonly string _name;

        public string Name
        {
            get { return _name; }
        }

        public FieldExpression(Type type, string name)
            : base((ExpressionType)MongoExpressionType.Field, type)
        {
            _name = name;
        }
    }

    internal class FieldDeclaration
    {
        private readonly string _name;
        private readonly Expression _expression;

        public string Name
        {
            get { return _name; }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public FieldDeclaration(string name, Expression expression)
        {
            _name = name;
            _expression = expression;
        }
    }

    internal class SelectExpression : Expression
    {
        private readonly ReadOnlyCollection<FieldDeclaration> _fields;
        private readonly Expression _from;
        private readonly Expression _where;

        public ReadOnlyCollection<FieldDeclaration> Fields
        {
            get { return _fields; }
        }

        public Expression From
        {
            get { return _from; }
        }

        public Expression Where
        {
            get { return _where; }
        }

        public SelectExpression(Type type, IEnumerable<FieldDeclaration> fields, Expression from, Expression where)
            : base((ExpressionType)MongoExpressionType.Select, type)
        {
            _fields = fields as ReadOnlyCollection<FieldDeclaration>;
            if (_fields == null)
                _fields = new List<FieldDeclaration>(fields).AsReadOnly();

            _from = from;
            _where = where;
        }
    }

    internal class ProjectionExpression : Expression
    {
        private readonly SelectExpression _source;
        private readonly Expression _projector;

        public SelectExpression Source
        {
            get { return _source; }
        }

        public Expression Projector
        {
            get { return _projector; }
        }

        public ProjectionExpression(SelectExpression source, Expression projector)
            : base((ExpressionType)MongoExpressionType.Projection, projector.Type)
        {
            _source = source;
            _projector = projector;
        }
    }
}