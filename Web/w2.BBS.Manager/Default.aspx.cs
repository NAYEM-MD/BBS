// (c) 2026 W2 Co.,Ltd.

using System;

namespace w2.BBS.Manager
{
	/// <summary>
	/// 既定ページ（ログイン画面へリダイレクト）
	/// </summary>
	public partial class Default : System.Web.UI.Page
	{
		private const string PATH_LOGIN = "~/Login.aspx";

		/// <summary>
		/// ページロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			this.Response.Redirect(PATH_LOGIN, false);
			this.Context.ApplicationInstance.CompleteRequest();
		}
	}
}
