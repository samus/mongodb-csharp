using System;
using System.Collections.Generic;

using MongoDB.Driver.Bson;
using MongoDB.Driver.IO;

namespace MongoDB.Driver
{
	
	
	public class Cursor : IDisposable
	{
		private Connection connection;
		
		private long id = -1;
		public long Id{
			get {return id;}
		}		
		
		private String fullCollectionName;
		public string FullCollectionName {
			get {return fullCollectionName;}
			set {fullCollectionName = value;}
		}
		
		private String collName;
		public string CollName {
			get {return collName;}
			set {collName = value;}
		}
		
		private Document spec;
		public Document Spec{
			get {return spec;}
			set {spec = value;}
		}
		
		private int limit;
		public int Limit{
			get {return limit;}
			set {limit = value;}
		}
		
		private int skip;
		public int Skip{
			get {return skip;}
			set {skip = value;}
		}

		private Document fields;
		public Document Fields{
			get {return fields;}
			set {fields = value;}
		}
		
		private bool modifiable = true;
		public bool Modifiable{
			get {return modifiable;}
		}
		
		private ReplyMessage reply;
		
		public Cursor(Connection conn, String fullCollectionName, Document spec, int limit, int skip, Document fields){
			this.connection = conn;
			this.FullCollectionName = fullCollectionName;
			if(spec == null)spec = new Document();
			this.Spec = spec;
			this.Limit = limit;
			this.Skip = skip;
			this.Fields = fields;
		}
		
		public IEnumerable<Document> Documents{
			get{
				if(this.reply == null){
					RetrieveData();
				}
				int docsReturned = 0;
				BsonDocument[] bdocs = this.reply.Documents;
				Boolean shouldBreak = false;
				while(!shouldBreak){
					foreach(BsonDocument bdoc in bdocs){
						if((this.Limit == 0) || (this.Limit != 0 && docsReturned < this.Limit)){
							docsReturned++;
							yield return (Document)bdoc.ToNative();
						}else{
							shouldBreak = true;
							yield break;
						}
					}
					if(this.Id != 0 && shouldBreak == false){
						RetrieveMoreData();					
						bdocs = this.reply.Documents;
						if(bdocs == null){
							shouldBreak = true;	
						}
					}else{
						shouldBreak = true;
					}
				}
			}			
		}
		
		private void RetrieveData(){
			QueryMessage query = new QueryMessage();
			query.FullCollectionName = this.FullCollectionName;
			query.Query = BsonConvert.From(this.Spec);
			query.NumberToReturn = this.Limit;
			query.NumberToSkip = this.Skip;
			if(this.Fields != null){
				query.ReturnFieldSelector = BsonConvert.From(this.Fields);
			}
			
			this.reply = connection.SendTwoWayMessage(query);
			this.id = this.reply.CursorID;
			if(this.Limit < 0)this.Limit = this.Limit * -1;
		}
		
		private void RetrieveMoreData(){
			GetMoreMessage gmm = new GetMoreMessage(this.FullCollectionName, this.Id, this.Limit);

			this.reply = connection.SendTwoWayMessage(gmm);
			this.reply.Documents = null;
			this.id = this.reply.CursorID;
		}
		
		
		public void Dispose(){
			if(this.Id == 0) return; //All server side resources disposed of.
			KillCursorsMessage kcm = new KillCursorsMessage(this.Id);			
			connection.SendMessage(kcm);
			this.id = 0;
		}
	}
}
