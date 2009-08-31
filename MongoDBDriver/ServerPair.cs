using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.Driver
{
	public class ServerPair
	{
		private Connection _left;
		private Connection _right;

		public ServerPair(Connection left, Connection right){
			this._left = left;
			this._right = right;
		}


	}
}
