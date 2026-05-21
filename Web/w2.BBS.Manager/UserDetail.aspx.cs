// (c) 2026 W2 Co.,Ltd.

using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using w2.Common;

namespace w2.BBS.Manager
{
	/// <summary>
	/// ユーザー詳細
	/// </summary>
	public partial class UserDetail : AdminPageBase
	{
		private const string PATH_USER_LIST = "~/UserList.aspx";
		
		
		

		private const string SQL_SELECT_USER =
			@"SELECT user_id, login_id, user_name
			FROM w2_User
			WHERE user_id = @user_id
			AND del_flg = 0";

		private const string SQL_SELECT_POSTS =
			@"SELECT post_id, title, body
			FROM w2_ForumPost
			WHERE user_id = @user_id
			AND del_flg = 0
			ORDER BY post_id DESC";

		private const string SQL_SELECT_REPLIES =
			@"SELECT reply_id, post_id, body
			FROM w2_ForumReply
			WHERE user_id = @user_id
			AND del_flg = 0
			ORDER BY reply_id DESC";

		/// <summary>
		/// ページロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (int.TryParse(this.Request.QueryString["id"], out var userId) is false || userId < 1)
			{
				this.Response.Redirect(PATH_USER_LIST, false);
				return;
			}

			this.ViewState[ManagerPageConstants.VIEWSTATE_KEY_USER_ID] = userId;

			if (this.IsPostBack)
			{
				return;
			}

			this.LoadProfile(userId);
			this.LoadPosts(userId);
			this.LoadReplies(userId);
		}

		/// <summary>
		/// 投稿リピータ コマンド
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void rPosts_OnItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName != ManagerPageConstants.COMMAND_DEL_POST)
			{
				return;
			}

			var postId = Convert.ToInt32(e.CommandArgument);
			var userId = Convert.ToInt32(this.ViewState[ManagerPageConstants.VIEWSTATE_KEY_USER_ID]);

			this.SoftDeletePostAndReplies(postId);
			this.LoadPosts(userId);
		}

		/// <summary>
		/// 返信リピータ コマンド
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void rReplies_OnItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName != ManagerPageConstants.COMMAND_DEL_REPLY)
			{
				return;
			}

			var replyId = Convert.ToInt32(e.CommandArgument);
			var userId = Convert.ToInt32(this.ViewState[ManagerPageConstants.VIEWSTATE_KEY_USER_ID]);

			this.SoftDeleteReply(replyId);
			this.LoadReplies(userId);
		}

		/// <summary>
		/// 戻るボタン クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbBack_OnClick(object sender, EventArgs e)
		{
			this.Response.Redirect(PATH_USER_LIST, false);
		}

		/// <summary>
		/// プロフィール読み込み
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		private void LoadProfile(int userId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_USER, connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read() is false)
						{
							this.pnlNotFound.Visible = true;
							this.pnlProfile.Visible = false;
							return;
						}

						this.lUserId.Text = System.Web.HttpUtility.HtmlEncode(Convert.ToString(reader["user_id"]));
						this.lLoginId.Text = System.Web.HttpUtility.HtmlEncode(Convert.ToString(reader["login_id"]));
						this.lUserName.Text = System.Web.HttpUtility.HtmlEncode(Convert.ToString(reader["user_name"]));
						this.pnlProfile.Visible = true;
						this.pnlNotFound.Visible = false;
					}
				}
			}
		}

		/// <summary>
		/// 投稿一覧読み込み
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		private void LoadPosts(int userId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_POSTS, connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);

					using (var adapter = new SqlDataAdapter(command))
					{
						var table = new DataTable();
						adapter.Fill(table);
						this.rPosts.DataSource = table;
						this.rPosts.DataBind();
					}
				}
			}
		}

		/// <summary>
		/// 返信一覧読み込み
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		private void LoadReplies(int userId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_REPLIES, connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);

					using (var adapter = new SqlDataAdapter(command))
					{
						var table = new DataTable();
						adapter.Fill(table);
						this.rReplies.DataSource = table;
						this.rReplies.DataBind();
					}
				}
			}
		}

		/// <summary>
		/// 投稿と紐づく返信の論理削除
		/// </summary>
		/// <param name="postId">投稿ID</param>
		private void SoftDeletePostAndReplies(int postId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(BbsSql.SOFT_DELETE_POST_BY_ID, connection))
				{
					command.Parameters.AddWithValue("@post_id", postId);
					command.ExecuteNonQuery();
				}

				using (var command = new SqlCommand(BbsSql.SOFT_DELETE_REPLIES_BY_POST, connection))
				{
					command.Parameters.AddWithValue("@post_id", postId);
					command.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// 返信の論理削除
		/// </summary>
		/// <param name="replyId">返信ID</param>
		private void SoftDeleteReply(int replyId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(BbsSql.SOFT_DELETE_REPLY, connection))
				{
					command.Parameters.AddWithValue("@reply_id", replyId);
					command.ExecuteNonQuery();
				}
			}
		}
	}
}

