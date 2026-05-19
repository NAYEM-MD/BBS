// (c) 2025 W2 Co.,Ltd.

using System;
using System.Configuration;
using w2.Common;

namespace w2.BBS.Manager
{
	public class Constants : w2.Common.Constants
	{
		static Constants()
		{
			var connectionString = ConfigurationManager.ConnectionStrings["w2mssql"];
			if (connectionString != null && string.IsNullOrEmpty(connectionString.ConnectionString) == false)
			{
				STRING_SQL_CONNECTION = connectionString.ConnectionString;
			}
		}
	}
}
