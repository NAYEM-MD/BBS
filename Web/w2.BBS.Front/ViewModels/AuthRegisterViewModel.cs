// (c) 2026 W2 Co.,Ltd.

namespace w2.BBS.Front.ViewModels
{
	/// <summary>
	/// 会員登録画面モデル
	/// </summary>
	public class AuthRegisterViewModel : BaseViewModel
	{
		/// <summary>ログインID</summary>
		public string LoginId { get; set; }

		/// <summary>パスワード</summary>
		public string Password { get; set; }

		/// <summary>ユーザー名</summary>
		public string UserName { get; set; }

		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}
}
