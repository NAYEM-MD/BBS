// (c) 2026 W2 Co.,Ltd.

namespace w2.BBS.Front.ViewModels
{
	/// <summary>
	/// アカウント設定画面モデル
	/// </summary>
	public class AccountSettingsViewModel : BaseViewModel
	{
		/// <summary>ログインID（変更不可・表示のみ）</summary>
		public string LoginId { get; set; }

		/// <summary>ユーザー名</summary>
		public string UserName { get; set; }

		/// <summary>現在のパスワード</summary>
		public string CurrentPassword { get; set; }

		/// <summary>新しいパスワード</summary>
		public string NewPassword { get; set; }

		/// <summary>新しいパスワード（確認）</summary>
		public string NewPasswordConfirm { get; set; }

		/// <summary>退会用パスワード</summary>
		public string DeletePassword { get; set; }

		/// <summary>成功メッセージ</summary>
		public string SuccessMessage { get; set; }

		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}
}
