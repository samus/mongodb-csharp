using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.Driver {
	public interface ICursor : IDisposable {
		long Id { get; }
		string FullCollectionName { get; set; }
		string CollName { get; set; }
		Document Spec { get; set; }
		int Limit { get; set; }
		int Skip { get; set; }
		Document Fields { get; set; }
		bool Modifiable { get; }
		IEnumerable<Document> Documents { get; }
	}
}
