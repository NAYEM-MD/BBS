namespace w2.BBS.Front.ViewModels
{
	/// <summary>
	/// 掲示板投稿詳細画面モデル
	/// </summary>
	public class ForumDetailViewModel : BaseViewModel
	{
		/// <summary>ログインユーザーID</summary>
		public int LoginUserId { get; set; }

		/// <summary>ログインユーザー名</summary>
		public string LoginUserName { get; set; }

		/// <summary>表示する投稿ID</summary>
		public int PostId { get; set; }
	}
}
