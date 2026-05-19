// (c) 2025 W2 Co.,Ltd.

using System;
using System.Web.UI;

namespace w2.BBS.Manager.Form.Common
{
	public partial class Default : System.Web.UI.MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.IsPostBack == false)
			{
				this.litBrand.Text = @"<span class=""brand"">BBS 管理</span>";
			}
		}

		protected void lbNavUserList_OnClick(object sender, EventArgs e)
		{
			this.Response.Redirect("~/UserList.aspx", false);
		}

		protected void lbNavPostList_OnClick(object sender, EventArgs e)
		{
			this.Response.Redirect("~/PostList.aspx", false);
		}

		protected void lbNavLogout_OnClick(object sender, EventArgs e)
		{
			this.Session.Remove(AdminSession.SessionKeyOperatorId);
			this.Session.Remove(AdminSession.SessionKeyLoginId);
			this.Response.Redirect("~/Login.aspx", false);
		}
	}
}
