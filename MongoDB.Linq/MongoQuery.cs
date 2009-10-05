using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace MongoDB.Linq
{
	public class MongoQuery : IMongoQuery
	{
		private readonly IMongoQueryProvider queryProvider;
		private readonly Expression expression;
		private MongoQuerySpec querySpec;

		public MongoQuery(IMongoQueryProvider queryProvider)
		{
			this.queryProvider = queryProvider;
			expression = Expression.Constant(this);
		}

		public MongoQuery(IMongoQueryProvider queryProvider, Expression expression)
		{
			this.queryProvider = queryProvider;
			this.expression = expression;
		}

		public IEnumerator<Document> GetEnumerator()
		{
			return ((IEnumerable<Document>)queryProvider.Execute(expression)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)queryProvider.Execute(expression)).GetEnumerator();
		}

		public Expression Expression
		{
			get { return expression; }
		}

		public Type ElementType
		{
			get { return typeof(Document); }
		}

		public IQueryProvider Provider
		{
			get { return queryProvider; }
		}

		private MongoQuerySpec QuerySpec
		{
			get
			{
				if (querySpec == null)
				{
					querySpec = queryProvider.GetQuerySpec(expression);
				}
				return querySpec;
			}
		}
		public Document Query { get { return QuerySpec.Query; } }
		public int Limit { get { return QuerySpec.Limit; } }
		public int Skip { get { return QuerySpec.Skip; } }
		public Document Fields { get { return QuerySpec.Fields; } }
	}
}
