// (c) 2026 W2 Co.,Ltd.

using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using w2.Common;

namespace w2.BBS.Manager
{
	/// <summary>
	/// ユーザー一覧
	/// </summary>
	public partial class UserList : AdminPageBase
	{
		private const string PATH_USER_DETAIL = "~/UserDetail.aspx?id=";
		private const string PATH_USER_EDIT = "~/UserEdit.aspx?id=";

		private const string SQL_SELECT_USERS =
			@"SELECT user_id, login_id, user_name
			FROM w2_User
			WHERE del_flg = 0
			AND
			(
				@keyword = N''
				OR login_id LIKE N'%' + @keyword + N'%'
				OR user_name LIKE N'%' + @keyword + N'%'
				OR CAST(user_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
			)
			ORDER BY user_id DESC";

		/// <summary>
		/// ページロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.IsPostBack)
			{
				return;
			}

			var keyword = (this.tbSearchKeyword.Text ?? string.Empty).Trim();
			this.LoadUsers(keyword);
		}

		/// <summary>
		/// 検索ボタン クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbSearch_OnClick(object sender, EventArgs e)
		{
			var keyword = (this.tbSearchKeyword.Text ?? string.Empty).Trim();
			this.LoadUsers(keyword);
		}

		/// <summary>
		/// クリアボタン クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbSearchClear_OnClick(object sender, EventArgs e)
		{
			this.tbSearchKeyword.Text = string.Empty;
			this.LoadUsers(string.Empty);
		}

		/// <summary>
		/// 詳細リンク クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbUserDetail_OnCommand(object sender, CommandEventArgs e)
		{
			var id = Convert.ToInt32(e.CommandArgument);
			this.Response.Redirect(PATH_USER_DETAIL + id, false);
		}

		/// <summary>
		/// 編集リンク クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbUserEdit_OnCommand(object sender, CommandEventArgs e)
		{
			var id = Convert.ToInt32(e.CommandArgument);
			this.Response.Redirect(PATH_USER_EDIT + id, false);
		}

		/// <summary>
		/// ユーザー一覧読み込み
		/// </summary>
		/// <param name="keyword">検索キーワード</param>
		private void LoadUsers(string keyword)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_USERS, connection))
				{
					command.Parameters.AddWithValue("@keyword", keyword ?? string.Empty);

					using (var adapter = new SqlDataAdapter(command))
					{
						var table = new DataTable();
						adapter.Fill(table);
						this.rUsers.DataSource = table;
						this.rUsers.DataBind();
					}
				}
			}
		}
	}
}
