// (c) 2026 W2 Co.,Ltd.

using System;
using System.Web.UI;

namespace w2.BBS.Manager
{
	/// <summary>
	/// 管理画面共通基底ページ
	/// </summary>
	public class AdminPageBase : Page
	{
		private const string PATH_LOGIN = "~/Login.aspx";
		private const string PATH_LOGIN_LOWER = "~/login.aspx";

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="e">イベント引数</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			var path = (this.Request.AppRelativeCurrentExecutionFilePath ?? string.Empty).ToLowerInvariant();
			if (path.Contains(PATH_LOGIN_LOWER))
			{
				return;
			}

			if (this.Session[AdminSession.SESSION_KEY_OPERATOR_ID] is null)
			{
				this.Response.Redirect(PATH_LOGIN, false);
				this.Context.ApplicationInstance.CompleteRequest();
			}
		}
	}
}
