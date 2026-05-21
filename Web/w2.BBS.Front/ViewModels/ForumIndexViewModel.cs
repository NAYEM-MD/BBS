// (c) 2026 W2 Co.,Ltd.

namespace w2.BBS.Front.ViewModels
{
	/// <summary>
	/// 掲示板トップ画面モデル
	/// </summary>
	public class ForumIndexViewModel : BaseViewModel
	{
		/// <summary>ログインユーザーID</summary>
		public int LoginUserId { get; set; }

		/// <summary>ログインユーザー名</summary>
		public string LoginUserName { get; set; }
	}
}
