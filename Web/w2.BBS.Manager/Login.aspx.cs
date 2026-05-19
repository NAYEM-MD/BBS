using System;
using System.Data.SqlClient;
using System.Web.UI;
using w2.Common;

namespace w2.BBS.Manager
{
	public partial class Login : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.IsPostBack)
			{
				return;
			}

			this.litTitle.Text = @"<h1>管理者ログイン</h1>";
			this.litLblLoginId.Text = @"<span class=""label"">ログインID</span>";
			this.litLblPassword.Text = @"<span class=""label"">パスワード</span>";
		}

		protected void lbLogin_OnClick(object sender, EventArgs e)
		{
			this.litError.Text = string.Empty;

			var loginId = (this.tbLoginId.Text ?? string.Empty).Trim();
			var password = (this.tbPassword.Text ?? string.Empty).Trim();
			if (string.IsNullOrEmpty(loginId) || string.IsNullOrEmpty(password))
			{
				this.litError.Text = @"<p class=""error"">ログインIDとパスワードを入力してください。</p>";
				return;
			}

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				var sql =
@"SELECT TOP 1 operator_id, login_id
FROM w2_Operator
WHERE login_id = @login_id
  AND password = @password";
				using (var command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@login_id", loginId);
					command.Parameters.AddWithValue("@password", password);
					using (var reader = command.ExecuteReader())
					{
						if (reader.Read() == false)
						{
							this.litError.Text = @"<p class=""error"">ログインIDまたはパスワードが正しくありません。</p>";
							return;
						}

						this.Session[AdminSession.SessionKeyOperatorId] = Convert.ToInt32(reader["operator_id"]);
						this.Session[AdminSession.SessionKeyLoginId] = Convert.ToString(reader["login_id"]);
					}
				}
			}

			this.Response.Redirect("~/UserList.aspx", false);
		}
	}
}
