// (c) 2026 W2 Co.,Ltd.

using System;
using System.Data.SqlClient;
using System.Web.UI;
using w2.Common;

namespace w2.BBS.Manager
{
	/// <summary>
	/// 管理者ログイン
	/// </summary>
	public partial class Login : Page
	{
		private const string PATH_AFTER_LOGIN = "~/UserList.aspx";

		private const string MESSAGE_INPUT_REQUIRED = "ログインIDとパスワードを入力してください。";
		private const string MESSAGE_LOGIN_FAILED = "ログインIDまたはパスワードが正しくありません。";

		private const string SQL_SELECT_OPERATOR =
			@"SELECT TOP 1
				operator_id,
				login_id
			FROM w2_Operator
			WHERE login_id = @login_id
			AND password = @password";

		/// <summary>
		/// ページロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// ログインボタン クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbLogin_OnClick(object sender, EventArgs e)
		{
			this.HideError();

			var loginId = (this.tbLoginId.Text ?? string.Empty).Trim();
			var password = (this.tbPassword.Text ?? string.Empty).Trim();
			if (string.IsNullOrEmpty(loginId) || string.IsNullOrEmpty(password))
			{
				this.ShowError(MESSAGE_INPUT_REQUIRED);
				return;
			}

			if (this.TryLogin(loginId, password) is false)
			{
				this.ShowError(MESSAGE_LOGIN_FAILED);
				return;
			}

			this.Response.Redirect(PATH_AFTER_LOGIN, false);
		}

		/// <summary>
		/// エラーメッセージ表示
		/// </summary>
		/// <param name="message">メッセージ</param>
		private void ShowError(string message)
		{
			this.lError.Text = System.Web.HttpUtility.HtmlEncode(message);
			this.pnlError.Visible = true;
		}

		/// <summary>
		/// エラーメッセージ非表示
		/// </summary>
		private void HideError()
		{
			this.lError.Text = string.Empty;
			this.pnlError.Visible = false;
		}

		/// <summary>
		/// ログイン処理
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="password">パスワード</param>
		/// <returns>ログインできたか</returns>
		private bool TryLogin(string loginId, string password)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_OPERATOR, connection))
				{
					command.Parameters.AddWithValue("@login_id", loginId);
					command.Parameters.AddWithValue("@password", password);

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read() is false)
						{
							return false;
						}

						this.Session[AdminSession.SESSION_KEY_OPERATOR_ID] = Convert.ToInt32(reader["operator_id"]);
						this.Session[AdminSession.SESSION_KEY_LOGIN_ID] = Convert.ToString(reader["login_id"]);
						return true;
					}
				}
			}
		}
	}
}
