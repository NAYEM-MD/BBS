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

		private const string SESSION_KEY_LOGIN_USER_ID = "LoginUserId";
		private const string SESSION_KEY_LOGIN_USER_NAME = "LoginUserName";

		private const string MESSAGE_LOGIN_REQUIRED = "ログインしてください。";
		private const string MESSAGE_USER_NAME_REQUIRED = "ユーザー名を入力してください。";
		private const string MESSAGE_CURRENT_PASSWORD_REQUIRED = "現在のパスワードを入力してください。";
		private const string MESSAGE_PASSWORD_WRONG = "現在のパスワードが正しくありません。";
		private const string MESSAGE_NEW_PASSWORD_MISMATCH = "新しいパスワードが一致しません。";
		private const string MESSAGE_NEW_PASSWORD_REQUIRED_BOTH = "新しいパスワードと確認を両方入力してください。";
		private const string MESSAGE_SETTINGS_UPDATED = "設定を更新しました。";
		private const string MESSAGE_DELETE_PASSWORD_REQUIRED = "退会するにはパスワードを入力してください。";
		private const string MESSAGE_ACCOUNT_DELETED = "アカウントを削除しました。";

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

		private const string SQL_SOFT_DELETE_USER_POSTS =
@"UPDATE w2_ForumPost
SET del_flg = 1
WHERE user_id = @user_id
	AND del_flg = 0";

		private const string SQL_SOFT_DELETE_USER_REPLIES =
@"UPDATE w2_ForumReply
SET del_flg = 1
WHERE user_id = @user_id
	AND del_flg = 0";

		private const string SQL_SOFT_DELETE_USER =
@"UPDATE w2_User
SET del_flg = 1
WHERE user_id = @user_id
	AND password = @password
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

			var vm = model ?? new AccountSettingsViewModel();
			vm.LoginId = this.GetLoginIdFromDb(loginUserId.Value);
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

			if (this.VerifyPassword(loginUserId.Value, currentPassword) == false)
			{
				vm.ErrorMessage = MESSAGE_PASSWORD_WRONG;
				vm.CurrentPassword = string.Empty;
				vm.NewPassword = string.Empty;
				vm.NewPasswordConfirm = string.Empty;
				return base.View(VIEW_SETTINGS, vm);
			}

			this.UpdateUserName(loginUserId.Value, vm.UserName);
			this.Session[SESSION_KEY_LOGIN_USER_NAME] = vm.UserName;

			var newPw = (vm.NewPassword ?? string.Empty).Trim();
			var newPwConfirm = (vm.NewPasswordConfirm ?? string.Empty).Trim();

			var hasNew = string.IsNullOrEmpty(newPw) == false;
			var hasConfirm = string.IsNullOrEmpty(newPwConfirm) == false;

			if (hasNew || hasConfirm)
			{
				if (hasNew == false || hasConfirm == false)
				{
					vm.ErrorMessage = MESSAGE_NEW_PASSWORD_REQUIRED_BOTH;
					vm.CurrentPassword = string.Empty;
					vm.NewPassword = string.Empty;
					vm.NewPasswordConfirm = string.Empty;
					return base.View(VIEW_SETTINGS, vm);
				}

				if (newPw != newPwConfirm)
				{
					vm.ErrorMessage = MESSAGE_NEW_PASSWORD_MISMATCH;
					vm.CurrentPassword = string.Empty;
					vm.NewPassword = string.Empty;
					vm.NewPasswordConfirm = string.Empty;
					return base.View(VIEW_SETTINGS, vm);
				}

				this.UpdatePassword(loginUserId.Value, newPw);
			}

			vm.SuccessMessage = MESSAGE_SETTINGS_UPDATED;
			vm.CurrentPassword = string.Empty;
			vm.NewPassword = string.Empty;
			vm.NewPasswordConfirm = string.Empty;
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

			var vm = model ?? new AccountSettingsViewModel();
			vm.LoginId = this.GetLoginIdFromDb(loginUserId.Value);
			vm.UserName = this.GetUserNameFromDb(loginUserId.Value);

			var deletePassword = (vm.DeletePassword ?? string.Empty).Trim();
			if (string.IsNullOrEmpty(deletePassword))
			{
				vm.ErrorMessage = MESSAGE_DELETE_PASSWORD_REQUIRED;
				return base.View(VIEW_SETTINGS, vm);
			}

			if (this.VerifyPassword(loginUserId.Value, deletePassword) == false)
			{
				vm.ErrorMessage = MESSAGE_PASSWORD_WRONG;
				vm.DeletePassword = string.Empty;
				return base.View(VIEW_SETTINGS, vm);
			}

			this.SoftDeleteUserContent(loginUserId.Value, deletePassword);
			this.ClearLoginSession();

			return this.Redirect("~/auth/login");
		}

		private int? GetLoginUserId()
		{
			var loginUserId = this.Session[SESSION_KEY_LOGIN_USER_ID];
			if (loginUserId is null)
			{
				return null;
			}

			return Convert.ToInt32(loginUserId);
		}

		private void ClearLoginSession()
		{
			this.Session.Remove(SESSION_KEY_LOGIN_USER_ID);
			this.Session.Remove(SESSION_KEY_LOGIN_USER_NAME);
		}

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
						if (reader.Read() == false)
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

		private string GetLoginIdFromDb(int userId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_PROFILE, connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read() == false)
						{
							return string.Empty;
						}

						return Convert.ToString(reader["login_id"]);
					}
				}
			}
		}

		private string GetUserNameFromDb(int userId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_PROFILE, connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read() == false)
						{
							return string.Empty;
						}

						return Convert.ToString(reader["user_name"]);
					}
				}
			}
		}

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

		private void UpdateUserName(int userId, string userName)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_UPDATE_USER_NAME, connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);
					command.Parameters.AddWithValue("@user_name", userName);

					command.ExecuteNonQuery();
				}
			}
		}

		private void UpdatePassword(int userId, string newPassword)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_UPDATE_PASSWORD, connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);
					command.Parameters.AddWithValue("@new_password", newPassword);

					command.ExecuteNonQuery();
				}
			}
		}

		private void SoftDeleteUserContent(int userId, string password)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var tx = connection.BeginTransaction())
				{
					try
					{
						using (var cmdPosts = new SqlCommand(SQL_SOFT_DELETE_USER_POSTS, connection, tx))
						{
							cmdPosts.Parameters.AddWithValue("@user_id", userId);
							cmdPosts.ExecuteNonQuery();
						}

						using (var cmdReplies = new SqlCommand(SQL_SOFT_DELETE_USER_REPLIES, connection, tx))
						{
							cmdReplies.Parameters.AddWithValue("@user_id", userId);
							cmdReplies.ExecuteNonQuery();
						}

						using (var cmdUser = new SqlCommand(SQL_SOFT_DELETE_USER, connection, tx))
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
