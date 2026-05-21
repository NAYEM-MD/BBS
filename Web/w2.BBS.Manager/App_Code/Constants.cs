// (c) 2026 W2 Co.,Ltd.

using System.Configuration;

namespace w2.BBS.Manager
{
	/// <summary>
	/// 管理画面定数
	/// </summary>
	public class Constants : w2.Common.Constants
	{
		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static Constants()
		{
			var connectionString = ConfigurationManager.ConnectionStrings["w2mssql"];
			if (connectionString is not null && string.IsNullOrEmpty(connectionString.ConnectionString) is false)
			{
				STRING_SQL_CONNECTION = connectionString.ConnectionString;
			}
		}
	}
}
