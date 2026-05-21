// (c) 2026 W2 Co.,Ltd.

namespace w2.BBS.Front.ViewModels
{
	/// <summary>
	/// ログイン画面モデル
	/// </summary>
	public class AuthLoginViewModel : BaseViewModel
	{
		/// <summary>ログインID</summary>
		public string LoginId { get; set; }

		/// <summary>パスワード</summary>
		public string Password { get; set; }

		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }

		/// <summary>案内メッセージ</summary>
		public string InfoMessage { get; set; }
	}
}
