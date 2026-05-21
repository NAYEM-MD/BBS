// (c) 2026 W2 Co.,Ltd.

using System.Configuration;
using System.Web;
using w2.Common;

namespace w2.BBS.Front
{
	/// <summary>
	/// SQL接続文字列をリクエスト開始時に設定する HTTP モジュール
	/// </summary>
	public class SqlConnectionModule : IHttpModule
	{
		private static bool s_initialized;

		public void Init(HttpApplication context)
		{
			if (s_initialized)
			{
				return;
			}

			s_initialized = true;
			w2.Common.Constants.STRING_SQL_CONNECTION = ConfigurationManager.ConnectionStrings["w2mssql"].ConnectionString;
		}

		public void Dispose()
		{
		}
	}
}
