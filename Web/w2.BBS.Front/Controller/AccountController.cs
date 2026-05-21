// (c) 2026 W2 Co.,Ltd.

using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using w2.BBS.Front.Controller.Shared;
using w2.BBS.Front.ViewModels;
using w2.Common;

namespace w2.BBS.Front.Controller
{
	/// <summary>
	/// アカウント設定・退会
	/// </summary>
	public class AccountController : BaseController
	{
		private const string VIEW_SETTINGS = "settings.liquid";

		private const string MESSAGE_USER_NAME_REQUIRED = "ユーザー名を入力してください。";
		private const string MESSAGE_CURRENT_PASSWORD_REQUIRED = "現在のパスワードを入力してください。";
		private const string MESSAGE_PASSWORD_WRONG = "現在のパスワードが正しくありません。";
		private const string MESSAGE_NEW_PASSWORD_MISMATCH = "新しいパスワードが一致しません。";
		private const string MESSAGE_NEW_PASSWORD_REQUIRED_BOTH = "新しいパスワードと確認を両方入力してください。";
		private const string MESSAGE_SETTINGS_UPDATED = "設定を更新しました。";
		private const string MESSAGE_DELETE_PASSWORD_REQUIRED = "退会するにはパスワードを入力してください。";

		private const string SQL_SELECT_PROFILE =
			@"SELECT login_id, user_name
			FROM w2_User
			WHERE user_id = @user_id
			AND del_flg = 0";

		private const string SQL_VERIFY_PASSWORD =
			@"SELECT COUNT(1)
			FROM w2_User
			WHERE user_id = @user_id
			AND password = @password
			AND del_flg = 0";

		private const string SQL_UPDATE_USER_NAME =
			@"UPDATE w2_User
			SET user_name = @user_name
			WHERE user_id = @user_id
			AND del_flg = 0";

		private const string SQL_UPDATE_PASSWORD =
			@"UPDATE w2_User
			SET password = @new_password
			WHERE user_id = @user_id
			AND del_flg = 0";

		/// <summary>
		/// 設定画面
		/// </summary>
		[HttpGet]
		[Route("~/account/settings")]
		public ActionResult Settings()
		{
			var loginUserId = this.GetLoginUserId();
			if (loginUserId is null)
			{
				return this.Redirect("~/auth/login");
			}

			var model = this.LoadSettingsModel(loginUserId.Value);
			if (model is null)
			{
				this.ClearLoginSession();
				return this.Redirect("~/auth/login");
			}

			return base.View(VIEW_SETTINGS, model);
		}

		/// <summary>
		/// プロフィール・パスワード更新
		/// </summary>
		[HttpPost]
		[Route("~/account/update")]
		public ActionResult Update(AccountSettingsViewModel model)
		{
			var loginUserId = this.GetLoginUserId();
			if (loginUserId is null)
			{
				return this.Redirect("~/auth/login");
			}

			var profile = this.LoadSettingsModel(loginUserId.Value);
			if (profile is null)
			{
				this.ClearLoginSession();
				return this.Redirect("~/auth/login");
			}

			var vm = model ?? new AccountSettingsViewModel();
			vm.LoginId = profile.LoginId;
			vm.UserName = (vm.UserName ?? string.Empty).Trim();

			if (string.IsNullOrEmpty(vm.UserName))
			{
				vm.ErrorMessage = MESSAGE_USER_NAME_REQUIRED;
				return base.View(VIEW_SETTINGS, vm);
			}

			var currentPassword = (vm.CurrentPassword ?? string.Empty).Trim();
			if (string.IsNullOrEmpty(currentPassword))
			{
				vm.ErrorMessage = MESSAGE_CURRENT_PASSWORD_REQUIRED;
				return base.View(VIEW_SETTINGS, vm);
			}

			if (this.VerifyPassword(loginUserId.Value, currentPassword) is false)
			{
				vm.ErrorMessage = MESSAGE_PASSWORD_WRONG;
				this.ClearPasswordFields(vm);
				return base.View(VIEW_SETTINGS, vm);
			}

			this.UpdateUserName(loginUserId.Value, vm.UserName);
			this.Session[FrontSession.SESSION_KEY_LOGIN_USER_NAME] = vm.UserName;

			var newPw = (vm.NewPassword ?? string.Empty).Trim();
			var newPwConfirm = (vm.NewPasswordConfirm ?? string.Empty).Trim();

			var hasNew = string.IsNullOrEmpty(newPw) is false;
			var hasConfirm = string.IsNullOrEmpty(newPwConfirm) is false;

			if (hasNew || hasConfirm)
			{
				if (hasNew is false || hasConfirm is false)
				{
					vm.ErrorMessage = MESSAGE_NEW_PASSWORD_REQUIRED_BOTH;
					this.ClearPasswordFields(vm);
					return base.View(VIEW_SETTINGS, vm);
				}

				if (newPw != newPwConfirm)
				{
					vm.ErrorMessage = MESSAGE_NEW_PASSWORD_MISMATCH;
					this.ClearPasswordFields(vm);
					return base.View(VIEW_SETTINGS, vm);
				}

				this.UpdatePassword(loginUserId.Value, newPw);
			}

			vm.SuccessMessage = MESSAGE_SETTINGS_UPDATED;
			this.ClearPasswordFields(vm);
			vm.DeletePassword = string.Empty;

			return base.View(VIEW_SETTINGS, vm);
		}

		/// <summary>
		/// アカウント削除（論理削除）
		/// </summary>
		[HttpPost]
		[Route("~/account/delete")]
		public ActionResult Delete(AccountSettingsViewModel model)
		{
			var loginUserId = this.GetLoginUserId();
			if (loginUserId is null)
			{
				return this.Redirect("~/auth/login");
			}

			var profile = this.LoadSettingsModel(loginUserId.Value);
			if (profile is null)
			{
				this.ClearLoginSession();
				return this.Redirect("~/auth/login");
			}

			var vm = model ?? new AccountSettingsViewModel();
			vm.LoginId = profile.LoginId;
			vm.UserName = profile.UserName;

			var deletePassword = (vm.DeletePassword ?? string.Empty).Trim();
			if (string.IsNullOrEmpty(deletePassword))
			{
				vm.ErrorMessage = MESSAGE_DELETE_PASSWORD_REQUIRED;
				return base.View(VIEW_SETTINGS, vm);
			}

			if (this.VerifyPassword(loginUserId.Value, deletePassword) is false)
			{
				vm.ErrorMessage = MESSAGE_PASSWORD_WRONG;
				vm.DeletePassword = string.Empty;
				return base.View(VIEW_SETTINGS, vm);
			}

			this.SoftDeleteUserContent(loginUserId.Value, deletePassword);
			this.ClearLoginSession();

			return this.Redirect("~/auth/login");
		}

		/// <summary>
		/// パスワード入力欄クリア
		/// </summary>
		/// <param name="vm">アカウント設定画面モデル</param>
		private void ClearPasswordFields(AccountSettingsViewModel vm)
		{
			vm.CurrentPassword = string.Empty;
			vm.NewPassword = string.Empty;
			vm.NewPasswordConfirm = string.Empty;
		}

		/// <summary>
		/// 設定画面モデル取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>設定画面モデル（該当ユーザー無しなら null）</returns>
		private AccountSettingsViewModel LoadSettingsModel(int userId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_PROFILE, connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read() is false)
						{
							return null;
						}

						return new AccountSettingsViewModel
						{
							LoginId = Convert.ToString(reader["login_id"]),
							UserName = Convert.ToString(reader["user_name"]),
						};
					}
				}
			}
		}

		/// <summary>
		/// パスワード照合
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="password">パスワード</param>
		/// <returns>正しいか</returns>
		private bool VerifyPassword(int userId, string password)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_VERIFY_PASSWORD, connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);
					command.Parameters.AddWithValue("@password", password);

					var count = Convert.ToInt32(command.ExecuteScalar());

					return count > 0;
				}
			}
		}

		/// <summary>
		/// ユーザー名更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userName">新ユーザー名</param>
		/// <returns>更新件数</returns>
		private int UpdateUserName(int userId, string userName)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_UPDATE_USER_NAME, connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);
					command.Parameters.AddWithValue("@user_name", userName);

					return command.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// パスワード更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="newPassword">新パスワード</param>
		/// <returns>更新件数</returns>
		private int UpdatePassword(int userId, string newPassword)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_UPDATE_PASSWORD, connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);
					command.Parameters.AddWithValue("@new_password", newPassword);

					return command.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// ユーザーと関連投稿・返信の論理削除
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="password">パスワード（再確認用）</param>
		private void SoftDeleteUserContent(int userId, string password)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var tx = connection.BeginTransaction())
				{
					try
					{
						using (var cmdPosts = new SqlCommand(BbsSql.SOFT_DELETE_USER_POSTS, connection, tx))
						{
							cmdPosts.Parameters.AddWithValue("@user_id", userId);
							cmdPosts.ExecuteNonQuery();
						}

						using (var cmdReplies = new SqlCommand(BbsSql.SOFT_DELETE_USER_REPLIES, connection, tx))
						{
							cmdReplies.Parameters.AddWithValue("@user_id", userId);
							cmdReplies.ExecuteNonQuery();
						}

						using (var cmdUser = new SqlCommand(BbsSql.SOFT_DELETE_USER_WITH_PASSWORD, connection, tx))
						{
							cmdUser.Parameters.AddWithValue("@user_id", userId);
							cmdUser.Parameters.AddWithValue("@password", password);
							cmdUser.ExecuteNonQuery();
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

