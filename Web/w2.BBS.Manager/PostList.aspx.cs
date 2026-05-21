// (c) 2026 W2 Co.,Ltd.

using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using w2.Common;

namespace w2.BBS.Manager
{
	/// <summary>
	/// 投稿・返信一覧
	/// </summary>
	public partial class PostList : AdminPageBase
	{
		

		private const string SQL_SELECT_POSTS =
			@"SELECT p.post_id, u.login_id, u.user_name, p.title, p.body
			FROM w2_ForumPost p
			INNER JOIN w2_User u ON
			(
				p.user_id = u.user_id
				AND u.del_flg = 0
			)
			WHERE p.del_flg = 0
			AND
			(
				@keyword = N''
				OR u.login_id LIKE N'%' + @keyword + N'%'
				OR u.user_name LIKE N'%' + @keyword + N'%'
				OR p.title LIKE N'%' + @keyword + N'%'
				OR p.body LIKE N'%' + @keyword + N'%'
				OR CAST(p.post_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
			)
			ORDER BY p.post_id DESC";

		private const string SQL_SELECT_REPLIES =
			@"SELECT r.reply_id, r.post_id, u.login_id, u.user_name, r.body
			FROM w2_ForumReply r
			INNER JOIN w2_User u ON
			(
				r.user_id = u.user_id
				AND u.del_flg = 0
			)
			WHERE r.del_flg = 0
			AND
			(
				@keyword = N''
				OR u.login_id LIKE N'%' + @keyword + N'%'
				OR u.user_name LIKE N'%' + @keyword + N'%'
				OR r.body LIKE N'%' + @keyword + N'%'
				OR CAST(r.reply_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
				OR CAST(r.post_id AS NVARCHAR(20)) LIKE N'%' + @keyword + N'%'
			)
			ORDER BY r.reply_id DESC";

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
			this.LoadPosts(keyword);
			this.LoadReplies(keyword);
		}

		/// <summary>
		/// 検索ボタン クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbSearch_OnClick(object sender, EventArgs e)
		{
			var keyword = (this.tbSearchKeyword.Text ?? string.Empty).Trim();
			this.LoadPosts(keyword);
			this.LoadReplies(keyword);
		}

		/// <summary>
		/// クリアボタン クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbSearchClear_OnClick(object sender, EventArgs e)
		{
			this.tbSearchKeyword.Text = string.Empty;
			this.LoadPosts(string.Empty);
			this.LoadReplies(string.Empty);
		}

		/// <summary>
		/// 投稿リピータ コマンド
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void rPosts_OnItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName != ManagerPageConstants.COMMAND_DEL)
			{
				return;
			}

			var postId = Convert.ToInt32(e.CommandArgument);
			this.SoftDeletePostAndReplies(postId);

			var keyword = (this.tbSearchKeyword.Text ?? string.Empty).Trim();
			this.LoadPosts(keyword);
			this.LoadReplies(keyword);
		}

		/// <summary>
		/// 返信リピータ コマンド
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void rReplies_OnItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName != ManagerPageConstants.COMMAND_DEL)
			{
				return;
			}

			var replyId = Convert.ToInt32(e.CommandArgument);
			this.SoftDeleteReply(replyId);

			var keyword = (this.tbSearchKeyword.Text ?? string.Empty).Trim();
			this.LoadPosts(keyword);
			this.LoadReplies(keyword);
		}

		/// <summary>
		/// 投稿一覧読み込み
		/// </summary>
		/// <param name="keyword">検索キーワード</param>
		private void LoadPosts(string keyword)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_POSTS, connection))
				{
					command.Parameters.AddWithValue("@keyword", keyword ?? string.Empty);

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
		/// <param name="keyword">検索キーワード</param>
		private void LoadReplies(string keyword)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_REPLIES, connection))
				{
					command.Parameters.AddWithValue("@keyword", keyword ?? string.Empty);

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

				using (var tx = connection.BeginTransaction())
				{
					try
					{
						using (var command = new SqlCommand(BbsSql.SOFT_DELETE_POST_BY_ID, connection, tx))
						{
							command.Parameters.AddWithValue("@post_id", postId);
							command.ExecuteNonQuery();
						}

						using (var command = new SqlCommand(BbsSql.SOFT_DELETE_REPLIES_BY_POST, connection, tx))
						{
							command.Parameters.AddWithValue("@post_id", postId);
							command.ExecuteNonQuery();
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
		}

		/// <summary>
		/// 返信の論理削除
		/// </summary>
		/// <param name="replyId">返信ID</param>
		private int SoftDeleteReply(int replyId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(BbsSql.SOFT_DELETE_REPLY, connection))
				{
					command.Parameters.AddWithValue("@reply_id", replyId);
					return command.ExecuteNonQuery();
				}
			}
		}
	}
}

