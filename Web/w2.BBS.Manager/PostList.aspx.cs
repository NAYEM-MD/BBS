// (c) 2025 W2 Co.,Ltd.

using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Common;

namespace w2.BBS.Manager
{
	public partial class PostList : AdminPageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.IsPostBack)
			{
				return;
			}

			this.litTitle.Text = @"<h1>投稿・返信一覧</h1>";
			this.litSearchLbl.Text = @"<span class=""label"">検索（ログインID・ユーザー名・本文・タイトル・ID）</span>";
			this.litPostsSection.Text = @"<h2>投稿</h2>";
			this.litP1.Text = "投稿ID";
			this.litP2.Text = "ログインID";
			this.litP3.Text = "ユーザー名";
			this.litP4.Text = "タイトル";
			this.litP5.Text = "本文";
			this.litP6.Text = "";
			this.litRepliesSection.Text = @"<h2>返信</h2>";
			this.litR1.Text = "返信ID";
			this.litR2.Text = "投稿ID";
			this.litR3.Text = "ログインID";
			this.litR4.Text = "ユーザー名";
			this.litR5.Text = "本文";
			this.litR6.Text = "";

			var kw = (this.tbSearchKeyword.Text ?? string.Empty).Trim();

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				using (var command = new SqlCommand(
@"SELECT p.post_id, u.login_id, u.user_name, p.title, p.body
FROM w2_ForumPost p
INNER JOIN w2_User u ON p.user_id = u.user_id AND u.del_flg = 0
WHERE p.del_flg = 0
  AND (
        @keyword = N''
        OR u.login_id LIKE N'%' + @keyword + N'%'
        OR u.user_name LIKE N'%' + @keyword + N'%'
        OR p.title LIKE N'%' + @keyword + N'%'
        OR p.body LIKE N'%' + @keyword + N'%'
        OR CAST(p.post_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
      )
ORDER BY p.post_id DESC", connection))
				{
					command.Parameters.AddWithValue("@keyword", kw);
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
@"SELECT r.reply_id, r.post_id, u.login_id, u.user_name, r.body
FROM w2_ForumReply r
INNER JOIN w2_User u ON r.user_id = u.user_id AND u.del_flg = 0
WHERE r.del_flg = 0
  AND (
        @keyword = N''
        OR u.login_id LIKE N'%' + @keyword + N'%'
        OR u.user_name LIKE N'%' + @keyword + N'%'
        OR r.body LIKE N'%' + @keyword + N'%'
        OR CAST(r.reply_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
        OR CAST(r.post_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
      )
ORDER BY r.reply_id DESC", connection))
				{
					command.Parameters.AddWithValue("@keyword", kw);
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

		protected void lbSearch_OnClick(object sender, EventArgs e)
		{
			var kw = (this.tbSearchKeyword.Text ?? string.Empty).Trim();

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				using (var command = new SqlCommand(
@"SELECT p.post_id, u.login_id, u.user_name, p.title, p.body
FROM w2_ForumPost p
INNER JOIN w2_User u ON p.user_id = u.user_id AND u.del_flg = 0
WHERE p.del_flg = 0
  AND (
        @keyword = N''
        OR u.login_id LIKE N'%' + @keyword + N'%'
        OR u.user_name LIKE N'%' + @keyword + N'%'
        OR p.title LIKE N'%' + @keyword + N'%'
        OR p.body LIKE N'%' + @keyword + N'%'
        OR CAST(p.post_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
      )
ORDER BY p.post_id DESC", connection))
				{
					command.Parameters.AddWithValue("@keyword", kw);
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
@"SELECT r.reply_id, r.post_id, u.login_id, u.user_name, r.body
FROM w2_ForumReply r
INNER JOIN w2_User u ON r.user_id = u.user_id AND u.del_flg = 0
WHERE r.del_flg = 0
  AND (
        @keyword = N''
        OR u.login_id LIKE N'%' + @keyword + N'%'
        OR u.user_name LIKE N'%' + @keyword + N'%'
        OR r.body LIKE N'%' + @keyword + N'%'
        OR CAST(r.reply_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
        OR CAST(r.post_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
      )
ORDER BY r.reply_id DESC", connection))
				{
					command.Parameters.AddWithValue("@keyword", kw);
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

		protected void lbSearchClear_OnClick(object sender, EventArgs e)
		{
			this.tbSearchKeyword.Text = string.Empty;

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				using (var command = new SqlCommand(
@"SELECT p.post_id, u.login_id, u.user_name, p.title, p.body
FROM w2_ForumPost p
INNER JOIN w2_User u ON p.user_id = u.user_id AND u.del_flg = 0
WHERE p.del_flg = 0
  AND (
        @keyword = N''
        OR u.login_id LIKE N'%' + @keyword + N'%'
        OR u.user_name LIKE N'%' + @keyword + N'%'
        OR p.title LIKE N'%' + @keyword + N'%'
        OR p.body LIKE N'%' + @keyword + N'%'
        OR CAST(p.post_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
      )
ORDER BY p.post_id DESC", connection))
				{
					command.Parameters.AddWithValue("@keyword", string.Empty);
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
@"SELECT r.reply_id, r.post_id, u.login_id, u.user_name, r.body
FROM w2_ForumReply r
INNER JOIN w2_User u ON r.user_id = u.user_id AND u.del_flg = 0
WHERE r.del_flg = 0
  AND (
        @keyword = N''
        OR u.login_id LIKE N'%' + @keyword + N'%'
        OR u.user_name LIKE N'%' + @keyword + N'%'
        OR r.body LIKE N'%' + @keyword + N'%'
        OR CAST(r.reply_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
        OR CAST(r.post_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
      )
ORDER BY r.reply_id DESC", connection))
				{
					command.Parameters.AddWithValue("@keyword", string.Empty);
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
			if (e.CommandName != "Del")
			{
				return;
			}

			var postId = Convert.ToInt32(e.CommandArgument);
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

			this.lbSearch_OnClick(this.lbSearch, EventArgs.Empty);
		}

		protected void rptReplies_OnItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName != "Del")
			{
				return;
			}

			var replyId = Convert.ToInt32(e.CommandArgument);
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

			this.lbSearch_OnClick(this.lbSearch, EventArgs.Empty);
		}
	}
}
