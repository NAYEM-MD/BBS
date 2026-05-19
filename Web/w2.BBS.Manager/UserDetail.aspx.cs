using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Common;

namespace w2.BBS.Manager
{
	public partial class UserDetail : AdminPageBase
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

			this.litTitle.Text = @"<h1>ユーザー詳細</h1>";
			this.litPostsTitle.Text = @"<h2>このユーザーの投稿</h2>";
			this.litRepliesTitle.Text = @"<h2>このユーザーの返信</h2>";
			this.litPc1.Text = "投稿ID";
			this.litPc2.Text = "タイトル";
			this.litPc3.Text = "";
			this.litRc1.Text = "返信ID";
			this.litRc2.Text = "投稿ID";
			this.litRc3.Text = "本文";
			this.litRc4.Text = "";

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				using (var command = new SqlCommand(
@"SELECT user_id, login_id, user_name
FROM w2_User
WHERE user_id = @user_id AND del_flg = 0", connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);
					using (var reader = command.ExecuteReader())
					{
						if (reader.Read() == false)
						{
							this.litProfile.Text = @"<p class=""error"">ユーザーが見つかりません。</p>";
							return;
						}

						var loginId = System.Web.HttpUtility.HtmlEncode(Convert.ToString(reader["login_id"]));
						var userName = System.Web.HttpUtility.HtmlEncode(Convert.ToString(reader["user_name"]));
						this.litProfile.Text =
							$@"<p><span class=""label"">ユーザーID</span>{reader["user_id"]}</p>
<p><span class=""label"">ログインID</span>{loginId}</p>
<p><span class=""label"">ユーザー名</span>{userName}</p>";
					}
				}
			}

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				using (var command = new SqlCommand(
@"SELECT post_id, title, body
FROM w2_ForumPost
WHERE user_id = @user_id AND del_flg = 0
ORDER BY post_id DESC", connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);
					using (var adapter = new SqlDataAdapter(command))
					{
						var table = new DataTable();
						adapter.Fill(table);
						this.rptPosts.DataSource = table;
						this.rptPosts.DataBind();
					}
				}
			}

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				using (var command = new SqlCommand(
@"SELECT reply_id, post_id, body
FROM w2_ForumReply
WHERE user_id = @user_id AND del_flg = 0
ORDER BY reply_id DESC", connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);
					using (var adapter = new SqlDataAdapter(command))
					{
						var table = new DataTable();
						adapter.Fill(table);
						this.rptReplies.DataSource = table;
						this.rptReplies.DataBind();
					}
				}
			}
		}

		protected void rptPosts_OnItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName != "DelPost")
			{
				return;
			}

			var postId = Convert.ToInt32(e.CommandArgument);
			var userId = Convert.ToInt32(this.ViewState["uid"]);

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				using (var command = new SqlCommand(
@"UPDATE w2_ForumPost SET del_flg = 1 WHERE post_id = @post_id AND del_flg = 0", connection))
				{
					command.Parameters.AddWithValue("@post_id", postId);
					command.ExecuteNonQuery();
				}
			}

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				using (var command = new SqlCommand(
@"SELECT post_id, title, body
FROM w2_ForumPost
WHERE user_id = @user_id AND del_flg = 0
ORDER BY post_id DESC", connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);
					using (var adapter = new SqlDataAdapter(command))
					{
						var table = new DataTable();
						adapter.Fill(table);
						this.rptPosts.DataSource = table;
						this.rptPosts.DataBind();
					}
				}
			}
		}

		protected void rptReplies_OnItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName != "DelReply")
			{
				return;
			}

			var replyId = Convert.ToInt32(e.CommandArgument);
			var userId = Convert.ToInt32(this.ViewState["uid"]);

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				using (var command = new SqlCommand(
@"UPDATE w2_ForumReply SET del_flg = 1 WHERE reply_id = @reply_id AND del_flg = 0", connection))
				{
					command.Parameters.AddWithValue("@reply_id", replyId);
					command.ExecuteNonQuery();
				}
			}

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				using (var command = new SqlCommand(
@"SELECT reply_id, post_id, body
FROM w2_ForumReply
WHERE user_id = @user_id AND del_flg = 0
ORDER BY reply_id DESC", connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);
					using (var adapter = new SqlDataAdapter(command))
					{
						var table = new DataTable();
						adapter.Fill(table);
						this.rptReplies.DataSource = table;
						this.rptReplies.DataBind();
					}
				}
			}
		}

		protected void lbBack_OnClick(object sender, EventArgs e)
		{
			this.Response.Redirect("~/UserList.aspx", false);
		}
	}
}
