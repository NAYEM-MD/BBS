using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using w2.BBS.Front.Controller.Shared;
using w2.BBS.Front.ViewModels;
using w2.Common;

namespace w2.BBS.Front.Controller
{
	/// <summary>
	/// 認証
	/// </summary>
	public class AuthController : BaseController
	{
		private const string VIEW_LOGIN = "login.liquid";
		private const string VIEW_REGISTER = "register.liquid";

		private const string SESSION_KEY_LOGIN_USER_ID = "LoginUserId";
		private const string SESSION_KEY_LOGIN_USER_NAME = "LoginUserName";

		private const string QUERY_KEY_REGISTERED = "registered";

		private const string MESSAGE_REGISTER_COMPLETED = "会員登録が完了しました。ログインしてください。";
		private const string MESSAGE_LOGIN_REQUIRED = "ログインIDとパスワードを入力してください。";
		private const string MESSAGE_LOGIN_FAILED = "ログインIDまたはパスワードが正しくありません。";
		private const string MESSAGE_REGISTER_REQUIRED = "すべて入力してください。";
		private const string MESSAGE_LOGIN_ID_DUPLICATE = "そのログインIDは既に使われています。";

		private const string SQL_SELECT_LOGIN_USER =
@"SELECT TOP 1
	user_id,
	user_name
FROM w2_User
WHERE login_id = @login_id
AND password = @password
AND del_flg = 0";

		private const string SQL_SELECT_LOGIN_ID_COUNT =
@"SELECT COUNT(1)
FROM w2_User
WHERE login_id = @login_id
AND del_flg = 0";

		private const string SQL_INSERT_USER =
@"INSERT w2_User
(
	login_id,
	password,
	user_name,
	del_flg
)
VALUES
(
	@login_id,
	@password,
	@user_name,
	0
)";

		/// <summary>
		/// ログイン画面
		/// </summary>
		[HttpGet]
		[Route("~/auth/login")]
		public ActionResult Login()
		{
			var model = new AuthLoginViewModel();

			this.SetLoginInfoMessage(model);

			return base.View(VIEW_LOGIN, model);
		}

		/// <summary>
		/// ログイン
		/// </summary>
		/// <param name="model">ログイン画面モデル</param>
		[HttpPost]
		[Route("~/auth/login")]
		public ActionResult Login(AuthLoginViewModel model)
		{
			var loginModel = model ?? new AuthLoginViewModel();

			if (this.ValidateLogin(loginModel) == false)
			{
				return base.View(VIEW_LOGIN, loginModel);
			}

			if (this.TryLogin(loginModel) == false)
			{
				loginModel.ErrorMessage = MESSAGE_LOGIN_FAILED;
				return base.View(VIEW_LOGIN, loginModel);
			}

			return this.Redirect("~/forum");
		}
		

		/// <summary>
		/// 会員登録画面
		/// </summary>
		[HttpGet]
		[Route("~/auth/register")]
		public ActionResult Register()
		{
			var model = new AuthRegisterViewModel();

			return base.View(VIEW_REGISTER, model);
		}

		/// <summary>
		/// 会員登録
		/// </summary>
		/// <param name="model">会員登録画面モデル</param>
		[HttpPost]
		[Route("~/auth/register")]
		public ActionResult Register(AuthRegisterViewModel model)
		{
			var registerModel = model ?? new AuthRegisterViewModel();

			if (this.ValidateRegister(registerModel) == false)
			{
				return base.View(VIEW_REGISTER, registerModel);
			}

			if (this.ExistsActiveLoginId(registerModel.LoginId))
			{
				registerModel.ErrorMessage = MESSAGE_LOGIN_ID_DUPLICATE;
				return base.View(VIEW_REGISTER, registerModel);
			}

			this.InsertUser(registerModel);

			return this.Redirect("~/auth/login?registered=1");
		}

		/// <summary>
		/// ログアウト
		/// </summary>
		[HttpPost]
		[Route("~/auth/logout")]
		public ActionResult Logout()
		{
			this.ClearLoginSession();

			return this.Redirect("~/auth/login");
		}

		/// <summary>
		/// ログイン完了メッセージ設定
		/// </summary>
		/// <param name="model">ログイン画面モデル</param>
		private void SetLoginInfoMessage(AuthLoginViewModel model)
		{
			if (model is null)
			{
				return;
			}

			var isRegistered = (this.Request.QueryString[QUERY_KEY_REGISTERED] == "1");
			if (isRegistered == false)
			{
				return;
			}

			model.InfoMessage = MESSAGE_REGISTER_COMPLETED;
		}

		/// <summary>
		/// ログイン入力チェック
		/// </summary>
		/// <param name="model">ログイン画面モデル</param>
		/// <returns>チェック結果</returns>
		private bool ValidateLogin(AuthLoginViewModel model)
		{
			if (model is null)
			{
				return false;
			}

			var hasLoginId = string.IsNullOrWhiteSpace(model.LoginId) == false;
			var hasPassword = string.IsNullOrWhiteSpace(model.Password) == false;
			if (hasLoginId && hasPassword)
			{
				return true;
			}

			model.ErrorMessage = MESSAGE_LOGIN_REQUIRED;
			return false;
		}

		/// <summary>
		/// 会員登録入力チェック
		/// </summary>
		/// <param name="model">会員登録画面モデル</param>
		/// <returns>チェック結果</returns>
		private bool ValidateRegister(AuthRegisterViewModel model)
		{
			if (model is null)
			{
				return false;
			}

			var hasLoginId = string.IsNullOrWhiteSpace(model.LoginId) == false;
			var hasPassword = string.IsNullOrWhiteSpace(model.Password) == false;
			var hasUserName = string.IsNullOrWhiteSpace(model.UserName) == false;
	

			if (hasLoginId
				&& hasPassword
				&& hasUserName)
			{
				return true;
			}

			model.ErrorMessage = MESSAGE_REGISTER_REQUIRED;
			return false;
		}

		/// <summary>
		/// ログイン
		/// </summary>
		/// <param name="model">ログイン画面モデル</param>
		/// <returns>ログインできたか</returns>
		private bool TryLogin(AuthLoginViewModel model)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_LOGIN_USER, connection))
				{
					command.Parameters.AddWithValue("@login_id", model.LoginId.Trim());
					command.Parameters.AddWithValue("@password", model.Password.Trim());

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read() == false)
						{
							return false;
						}

						this.Session[SESSION_KEY_LOGIN_USER_ID] = Convert.ToInt32(reader["user_id"]);
						this.Session[SESSION_KEY_LOGIN_USER_NAME] = Convert.ToString(reader["user_name"]);

						return true;
					}
				}
			}
		}

		/// <summary>
		/// 有効なログインID存在チェック
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <returns>存在するか</returns>
		private bool ExistsActiveLoginId(string loginId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_LOGIN_ID_COUNT, connection))
				{
					command.Parameters.AddWithValue("@login_id", loginId.Trim());

					var count = Convert.ToInt32(command.ExecuteScalar());
					return (count > 0);
				}
			}
		}

		/// <summary>
		/// 会員登録
		/// </summary>
		/// <param name="model">会員登録画面モデル</param>
		private void InsertUser(AuthRegisterViewModel model)
		{

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_INSERT_USER, connection))
				{
					command.Parameters.AddWithValue("@login_id", model.LoginId.Trim());
					command.Parameters.AddWithValue("@password", model.Password.Trim());
					command.Parameters.AddWithValue("@user_name", model.UserName.Trim());

					command.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// ログインセッションクリア
		/// </summary>
		private void ClearLoginSession()
		{
			this.Session.Remove(SESSION_KEY_LOGIN_USER_ID);
			this.Session.Remove(SESSION_KEY_LOGIN_USER_NAME);
		}
	}
}

