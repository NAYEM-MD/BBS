using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Common;

namespace w2.BBS.Manager
{
	public partial class UserList : AdminPageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.IsPostBack)
			{
				return;
			}

			this.litTitle.Text = @"<h1>ユーザー一覧</h1>";
			this.litSearchLbl.Text = @"<span class=""label"">検索（ログインID・ユーザー名・ユーザーID）</span>";
			this.litTh1.Text = "ユーザーID";
			this.litTh2.Text = "ログインID";
			this.litTh3.Text = "ユーザー名";
			this.litTh4.Text = "詳細";
			this.litTh5.Text = "編集";

			var kw = (this.tbSearchKeyword.Text ?? string.Empty).Trim();
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();
				var sql =
@"SELECT user_id, login_id, user_name
FROM w2_User
WHERE del_flg = 0
  AND (
        @keyword = N''
        OR login_id LIKE N'%' + @keyword + N'%'
        OR user_name LIKE N'%' + @keyword + N'%'
        OR CAST(user_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
      )
ORDER BY user_id DESC";
				using (var command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@keyword", kw);
					using (var adapter = new SqlDataAdapter(command))
					{
						var table = new DataTable();
						adapter.Fill(table);
						this.rptUsers.DataSource = table;
						this.rptUsers.DataBind();
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
				var sql =
@"SELECT user_id, login_id, user_name
FROM w2_User
WHERE del_flg = 0
  AND (
        @keyword = N''
        OR login_id LIKE N'%' + @keyword + N'%'
        OR user_name LIKE N'%' + @keyword + N'%'
        OR CAST(user_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
      )
ORDER BY user_id DESC";
				using (var command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@keyword", kw);
					using (var adapter = new SqlDataAdapter(command))
					{
						var table = new DataTable();
						adapter.Fill(table);
						this.rptUsers.DataSource = table;
						this.rptUsers.DataBind();
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
				var sql =
@"SELECT user_id, login_id, user_name
FROM w2_User
WHERE del_flg = 0
  AND (
        @keyword = N''
        OR login_id LIKE N'%' + @keyword + N'%'
        OR user_name LIKE N'%' + @keyword + N'%'
        OR CAST(user_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
      )
ORDER BY user_id DESC";
				using (var command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@keyword", string.Empty);
					using (var adapter = new SqlDataAdapter(command))
					{
						var table = new DataTable();
						adapter.Fill(table);
						this.rptUsers.DataSource = table;
						this.rptUsers.DataBind();
					}
				}
			}
		}

		protected void lbUserDetail_OnCommand(object sender, CommandEventArgs e)
		{
			var id = Convert.ToInt32(e.CommandArgument);
			this.Response.Redirect("~/UserDetail.aspx?id=" + id, false);
		}

		protected void lbUserEdit_OnCommand(object sender, CommandEventArgs e)
		{
			var id = Convert.ToInt32(e.CommandArgument);
			this.Response.Redirect("~/UserEdit.aspx?id=" + id, false);
		}
	}
}
