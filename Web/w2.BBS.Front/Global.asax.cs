using System;
using System.Web.Routing;
using w2.Common;

namespace w2.BBS.Front
{
	public class Global : System.Web.HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			Constants.APPLICATION_NAME = "w2.BBS.Front";
			Constants.PHYSICALDIRPATH_LOGFILE = "C:\\Logs\\";
			RouteConfig.RegisterRoutes(RouteTable.Routes);
		}
		
		protected void Session_Start(object sender, EventArgs e)
		{
			// セッションにダミー値を格納
			// （セッションに何か格納しないとセッションクッキーが発行されず、Session.SessionIDが定まらない為）
			this.Session["__DummyValueToFixSessionID__"] = string.Empty;
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{

		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}
