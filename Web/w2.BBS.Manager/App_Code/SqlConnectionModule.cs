// (c) 2026 W2 Co.,Ltd.

using System.Configuration;
using System.Web;

namespace w2.BBS.Manager
{
	/// <summary>
	/// SQL接続文字列初期化モジュール
	/// </summary>
	public class SqlConnectionModule : IHttpModule
	{
		private static bool s_initialized;

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="context">HTTPアプリケーション</param>
		public void Init(HttpApplication context)
		{
			if (s_initialized)
			{
				return;
			}

			s_initialized = true;
			w2.Common.Constants.STRING_SQL_CONNECTION = ConfigurationManager.ConnectionStrings["w2mssql"].ConnectionString;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
		}
	}
}
