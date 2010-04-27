using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MongoDB.Linq
{
    internal class ProjectionReader<T, TResult> : IEnumerable<TResult>
    {
        private Enumerator _enumerator;

        public ProjectionReader(ICursor<T> cursor, Func<T, TResult> projector)
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

        private class Enumerator : IEnumerator<TResult>, IDisposable
        {
            private IEnumerator<T> _cursorEnumerator;
            private Func<T, TResult> _projector;

            public TResult Current
            {
                get { return _projector(_cursorEnumerator.Current); }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public Enumerator(IEnumerator<T> enumerator, Func<T, TResult> projector)
            {
                _cursorEnumerator = enumerator;
                _projector = projector;
            }

            public void Dispose()
            {
                _cursorEnumerator.Dispose();
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
