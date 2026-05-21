// (c) 2026 W2 Co.,Ltd.

using System;
using System.Configuration;
using System.Web.Routing;
using w2.Common;

namespace w2.BBS.Front
{
	/// <summary>
	/// アプリケーション
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// アプリケーション開始
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_Start(object sender, EventArgs e)
		{
			Constants.APPLICATION_NAME = "w2.BBS.Front";
			Constants.PHYSICALDIRPATH_LOGFILE = "C:\\Logs\\";
			Constants.STRING_SQL_CONNECTION = ConfigurationManager.ConnectionStrings["w2mssql"].ConnectionString;
			RouteConfig.RegisterRoutes(RouteTable.Routes);
		}

		/// <summary>
		/// セッション開始
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Session_Start(object sender, EventArgs e)
		{
			// セッションにダミー値を格納
			// （セッションに何か格納しないとセッションクッキーが発行されず、Session.SessionIDが定まらない為）
			this.Session[FrontSession.SESSION_KEY_DUMMY] = string.Empty;
		}

		/// <summary>
		/// リクエスト開始
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_BeginRequest(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// 認証リクエスト
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// エラー発生
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_Error(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// セッション終了
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Session_End(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// アプリケーション終了
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_End(object sender, EventArgs e)
		{
		}
	}
}
