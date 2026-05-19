// (c) 2025 W2 Co.,Ltd.

using System;
using System.Data.SqlClient;
using System.Web.UI;
using w2.Common;

namespace w2.BBS.Manager
{
	public partial class UserEdit : AdminPageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (int.TryParse(this.Request.QueryString["id"], out var userId) == false || userId < 1)
			{
				this.Response.Redirect("~/UserList.aspx", false);
				return;
			}

			this.ViewState["uid"] = userId;

			if (this.IsPostBack)
			{
				return;
			}

			this.litTitle.Text = @"<h1>ユーザー編集</h1>";
			this.litLblLoginId.Text = @"<span class=""label"">ログインID</span>";
			this.litLblUserName.Text = @"<span class=""label"">ユーザー名</span>";
			this.litLblPassword.Text = @"<span class=""label"">新パスワード</span>（空なら変更しない）";

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				using (var command = new SqlCommand(
@"SELECT login_id, user_name FROM w2_User WHERE user_id = @user_id AND del_flg = 0", connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);
					using (var reader = command.ExecuteReader())
					{
						if (reader.Read() == false)
						{
							this.litMsg.Text = @"<p class=""error"">ユーザーが見つかりません。</p>";
							return;
						}

						this.litLoginId.Text = System.Web.HttpUtility.HtmlEncode(Convert.ToString(reader["login_id"]));
						this.tbUserName.Text = Convert.ToString(reader["user_name"]);
					}
				}
			}
		}

		protected void lbSave_OnClick(object sender, EventArgs e)
		{
			this.litMsg.Text = string.Empty;
			var userId = Convert.ToInt32(this.ViewState["uid"]);
			var name = (this.tbUserName.Text ?? string.Empty).Trim();
			if (string.IsNullOrEmpty(name))
			{
				this.litMsg.Text = @"<p class=""error"">ユーザー名を入力してください。</p>";
				return;
			}

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				using (var command = new SqlCommand(
@"UPDATE w2_User SET user_name = @user_name WHERE user_id = @user_id AND del_flg = 0", connection))
				{
					command.Parameters.AddWithValue("@user_name", name);
					command.Parameters.AddWithValue("@user_id", userId);
					command.ExecuteNonQuery();
				}

				var pw = (this.tbPassword.Text ?? string.Empty).Trim();
				if (string.IsNullOrEmpty(pw) == false)
				{
					using (var command = new SqlCommand(
@"UPDATE w2_User SET password = @password WHERE user_id = @user_id AND del_flg = 0", connection))
					{
						command.Parameters.AddWithValue("@password", pw);
						command.Parameters.AddWithValue("@user_id", userId);
						command.ExecuteNonQuery();
					}
				}
			}

			this.litMsg.Text = @"<p class=""ok"">保存しました。</p>";
		}

		protected void lbDeleteUser_OnClick(object sender, EventArgs e)
		{
			var userId = Convert.ToInt32(this.ViewState["uid"]);

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				using (var tx = connection.BeginTransaction())
				{
					try
					{
						using (var c1 = new SqlCommand(
@"UPDATE w2_ForumPost SET del_flg = 1 WHERE user_id = @user_id AND del_flg = 0", connection, tx))
						{
							c1.Parameters.AddWithValue("@user_id", userId);
							c1.ExecuteNonQuery();
						}
						using (var c2 = new SqlCommand(
@"UPDATE w2_ForumReply SET del_flg = 1 WHERE user_id = @user_id AND del_flg = 0", connection, tx))
						{
							c2.Parameters.AddWithValue("@user_id", userId);
							c2.ExecuteNonQuery();
						}
						using (var c3 = new SqlCommand(
@"UPDATE w2_User SET del_flg = 1 WHERE user_id = @user_id AND del_flg = 0", connection, tx))
						{
							c3.Parameters.AddWithValue("@user_id", userId);
							c3.ExecuteNonQuery();
						}
						tx.Commit();
					}
					catch
					{
						tx.Rollback();
						throw;
					}
				}
			}

			this.Response.Redirect("~/UserList.aspx", false);
		}

		protected void lbBack_OnClick(object sender, EventArgs e)
		{
			this.Response.Redirect("~/UserList.aspx", false);
		}
	}
}
