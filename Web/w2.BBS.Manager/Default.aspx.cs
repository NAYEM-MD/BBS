using System;

namespace w2.BBS.Manager
{
	public partial class Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			this.Response.Redirect("~/Login.aspx", false);
			this.Context.ApplicationInstance.CompleteRequest();
		}
	}
}
