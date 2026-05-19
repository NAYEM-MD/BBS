// (c) 2025 W2 Co.,Ltd.

using System;
using System.Web.UI;

namespace w2.BBS.Manager
{
	public class AdminPageBase : Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			var path = (this.Request.AppRelativeCurrentExecutionFilePath ?? string.Empty).ToLowerInvariant();
			if (path.Contains("~/login.aspx"))
			{
				return;
			}

			if (this.Session[AdminSession.SessionKeyOperatorId] is null)
			{
				this.Response.Redirect("~/Login.aspx", false);
				this.Context.ApplicationInstance.CompleteRequest();
			}
		}
	}
}
