using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MongoDB.Driver.Linq
{
    internal abstract class MongoProjectionDocument
    {
        public abstract object GetValue(string memberName);
    }

    internal class MongoFieldProjection
    {
        private List<string> _fields;

        public IEnumerable<string> Fields
        {
            get { return _fields; }
        }

        public LambdaExpression Projector { get; set; }

        public MongoFieldProjection()
        {
            _fields = new List<string>();
        }

        public void AddField(string field)
        {
            _fields.Add(field);
        }

        public Document CreateDocument()
        {
            var doc = new Document();
            foreach (var field in _fields)
                doc.Add(field, 1);
            return doc;
        }
    }

    internal class MongoFieldProjector : ExpressionVisitor
    {
        private static MethodInfo miGetValue;

        private MongoFieldProjection _projection;
        private ParameterExpression _document;

        public MongoFieldProjector()
        {
            if (miGetValue == null)
                miGetValue = typeof(MongoProjectionDocument).GetMethod("GetValue");
        }

        public MongoFieldProjection ProjectFields(Expression expression, ParameterExpression document)
        {
            _document = document;
            _projection = new MongoFieldProjection();
            _projection.Projector = Expression.Lambda(Visit(expression), _document);

            return _projection;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                _projection.AddField(m.Member.Name);
                return Expression.Convert(Expression.Call(_document, miGetValue, Expression.Constant(m.Member.Name)), m.Type);
            }
            else
                return base.VisitMemberAccess(m);
        }
    }

    internal class MongoProjectionReader<T, TResult> : IEnumerable<TResult>
    {
        private Enumerator _enumerator;

        public MongoProjectionReader(ICursor<T> cursor, Func<MongoProjectionDocument, TResult> projector)
        {
            _enumerator = new Enumerator(cursor.Documents.GetEnumerator(), projector);
        }

        public IEnumerator<TResult> GetEnumerator()
        {
            var e = _enumerator;
            if (e == null)
                throw new InvalidOperationException("Cannot enumerate more than once.");
            _enumerator = null;
            return e;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Enumerator : MongoProjectionDocument, IEnumerator<TResult>, IDisposable
        {
            private TResult _current;
            private IEnumerator<T> _cursorEnumerator;
            private Func<MongoProjectionDocument, TResult> _projector;

            public TResult Current
            {
                get { return _projector(this); }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return _current; }
            }

            public Enumerator(IEnumerator<T> enumerator, Func<MongoProjectionDocument, TResult> projector)
            {
                _cursorEnumerator = enumerator;
                _projector = projector;
            }

            public void Dispose()
            {
                _cursorEnumerator.Dispose();
            }

            public override object GetValue(string memberName)
            {
                var currentObj = _cursorEnumerator.Current;
                return currentObj.GetType().GetProperty(memberName).GetValue(currentObj, null);
            }

            public bool MoveNext()
            {
                return _cursorEnumerator.MoveNext();
            }

            public void Reset()
            {
                _cursorEnumerator.Reset();
            }
        }
    }
}