// (c) 2026 W2 Co.,Ltd.

using System;
using System.Web.UI;

namespace w2.BBS.Manager.Form.Common
{
	/// <summary>
	/// 共通マスターページ
	/// </summary>
	public partial class Default : System.Web.UI.MasterPage
	{
		private const string PATH_USER_LIST = "~/UserList.aspx";
		private const string PATH_POST_LIST = "~/PostList.aspx";
		private const string PATH_LOGIN = "~/Login.aspx";

		/// <summary>
		/// ページロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// ユーザー一覧ナビ クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbNavUserList_OnClick(object sender, EventArgs e)
		{
			this.Response.Redirect(PATH_USER_LIST, false);
		}

		/// <summary>
		/// 投稿一覧ナビ クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbNavPostList_OnClick(object sender, EventArgs e)
		{
			this.Response.Redirect(PATH_POST_LIST, false);
		}

		/// <summary>
		/// ログアウト ナビ クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbNavLogout_OnClick(object sender, EventArgs e)
		{
			this.Session.Remove(AdminSession.SESSION_KEY_OPERATOR_ID);
			this.Session.Remove(AdminSession.SESSION_KEY_LOGIN_ID);
			this.Response.Redirect(PATH_LOGIN, false);
		}
	}
}
