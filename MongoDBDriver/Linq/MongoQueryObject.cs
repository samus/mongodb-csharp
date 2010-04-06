using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Linq
{
    public class MongoQueryObject
    {
        private Stack<Scope> _scopes;

        public Document Fields { get; set; }

        public int NumberToSkip { get; set; }

        public int NumberToLimit { get; set; }

        public Document Order { get; set; }

        public Document Query { get; set; }

        public MongoQueryObject()
        {
            Fields = new Document();
            Order = new Document();
            Query = new Document();

            _scopes = new Stack<Scope>();
        }

        public void AddCondition(object value)
        {
            _scopes.Peek().AddCondition(value);
        }

        public void PushConditionScope(string name)
        {
            _scopes.Push(new Scope(name));
        }

        public void PopConditionScope()
        {
            var scope = _scopes.Pop();
            var doc = Query;
            foreach (var s in _scopes.Reverse()) //as if it were a queue
            {
                var sub = doc[s.Key];
                if (sub == null)
                    doc[s.Key] = sub = new Document();
                else if (!(sub is Document))
                    throw new InvalidQueryException();

                doc = (Document)sub;
            }

            doc[scope.Key] = scope.Value;
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        private class Scope
        {
            public string Key { get; private set; }

            public object Value { get; private set; }

            public Scope(string key)
            {
                Key = key;
            }

            public void AddCondition(object value)
            {
                if (Value is Document)
                {
                    if (!(value is Document))
                        throw new InvalidQueryException();

                    ((Document)Value).Merge((Document)value);
                }
                else if (Value is Document)
                    throw new InvalidQueryException();
                else if(Value != null)
                    throw new InvalidQueryException();
                else
                    Value = value;
            }
        }
    }
}