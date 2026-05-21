// (c) 2026 W2 Co.,Ltd.

using System;
using System.Data.SqlClient;
using w2.Common;

namespace w2.BBS.Manager
{
	/// <summary>
	/// ユーザー編集
	/// </summary>
	public partial class UserEdit : AdminPageBase
	{
		private const string PATH_USER_LIST = "~/UserList.aspx";
		
		private const string CSS_CLASS_ERROR = "error";
		private const string CSS_CLASS_OK = "ok";

		private const string MESSAGE_NOT_FOUND = "ユーザーが見つかりません。";
		private const string MESSAGE_USER_NAME_REQUIRED = "ユーザー名を入力してください。";
		private const string MESSAGE_SAVED = "保存しました。";

		private const string SQL_SELECT_USER =
			@"SELECT login_id, user_name
			FROM w2_User
			WHERE user_id = @user_id
			AND del_flg = 0";

		private const string SQL_UPDATE_USER_NAME =
			@"UPDATE w2_User
			SET user_name = @user_name
			WHERE user_id = @user_id
			AND del_flg = 0";

		private const string SQL_UPDATE_PASSWORD =
			@"UPDATE w2_User
			SET password = @password
			WHERE user_id = @user_id
			AND del_flg = 0";

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

			this.LoadUser(userId);
		}

		/// <summary>
		/// 保存ボタン クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbSave_OnClick(object sender, EventArgs e)
		{
			this.HideMessage();

			var userId = Convert.ToInt32(this.ViewState[ManagerPageConstants.VIEWSTATE_KEY_USER_ID]);
			var name = (this.tbUserName.Text ?? string.Empty).Trim();
			if (string.IsNullOrEmpty(name))
			{
				this.ShowMessage(MESSAGE_USER_NAME_REQUIRED, CSS_CLASS_ERROR);
				return;
			}

			this.UpdateUserName(userId, name);

			var pw = (this.tbPassword.Text ?? string.Empty).Trim();
			if (string.IsNullOrEmpty(pw) is false)
			{
				this.UpdatePassword(userId, pw);
			}

			this.ShowMessage(MESSAGE_SAVED, CSS_CLASS_OK);
		}

		/// <summary>
		/// ユーザー削除ボタン クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbDeleteUser_OnClick(object sender, EventArgs e)
		{
			var userId = Convert.ToInt32(this.ViewState[ManagerPageConstants.VIEWSTATE_KEY_USER_ID]);
			this.SoftDeleteUserContent(userId);

			this.Response.Redirect(PATH_USER_LIST, false);
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
		/// メッセージ表示
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="cssClass">CSSクラス</param>
		private void ShowMessage(string message, string cssClass)
		{
			this.lMsg.Text = System.Web.HttpUtility.HtmlEncode(message);
			this.pnlMsg.CssClass = cssClass;
			this.pnlMsg.Visible = true;
		}

		/// <summary>
		/// メッセージ非表示
		/// </summary>
		private void HideMessage()
		{
			this.lMsg.Text = string.Empty;
			this.pnlMsg.Visible = false;
		}

		/// <summary>
		/// ユーザー情報読み込み
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		private void LoadUser(int userId)
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
							this.ShowMessage(MESSAGE_NOT_FOUND, CSS_CLASS_ERROR);
							return;
						}

						this.lLoginId.Text = System.Web.HttpUtility.HtmlEncode(Convert.ToString(reader["login_id"]));
						this.tbUserName.Text = Convert.ToString(reader["user_name"]);
					}
				}
			}
		}

		/// <summary>
		/// ユーザー名更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userName">ユーザー名</param>
		/// <returns>更新件数</returns>
		private int UpdateUserName(int userId, string userName)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_UPDATE_USER_NAME, connection))
				{
					command.Parameters.AddWithValue("@user_name", userName);
					command.Parameters.AddWithValue("@user_id", userId);
					return command.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// パスワード更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="password">パスワード</param>
		/// <returns>更新件数</returns>
		private int UpdatePassword(int userId, string password)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_UPDATE_PASSWORD, connection))
				{
					command.Parameters.AddWithValue("@password", password);
					command.Parameters.AddWithValue("@user_id", userId);
					return command.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// ユーザーと紐づく投稿・返信の論理削除
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		private void SoftDeleteUserContent(int userId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var tx = connection.BeginTransaction())
				{
					try
					{
						using (var command = new SqlCommand(BbsSql.SOFT_DELETE_USER_POSTS, connection, tx))
						{
							command.Parameters.AddWithValue("@user_id", userId);
							command.ExecuteNonQuery();
						}

						using (var command = new SqlCommand(BbsSql.SOFT_DELETE_USER_REPLIES, connection, tx))
						{
							command.Parameters.AddWithValue("@user_id", userId);
							command.ExecuteNonQuery();
						}

						using (var command = new SqlCommand(BbsSql.SOFT_DELETE_USER, connection, tx))
						{
							command.Parameters.AddWithValue("@user_id", userId);
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
	}
}

