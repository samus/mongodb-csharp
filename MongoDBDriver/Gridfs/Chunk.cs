using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.Driver.GridFS
{
	public class Chunk
	{
        Int32 id;
        Int64 files_id;
        Int32 chunk_number = 0;
        Byte[] data;
	}
}
