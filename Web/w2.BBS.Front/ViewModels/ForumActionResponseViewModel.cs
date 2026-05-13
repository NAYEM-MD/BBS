namespace w2.BBS.Front.ViewModels
{
	/// <summary>
	/// 掲示板処理レスポンスモデル
	/// </summary>
	public class ForumActionResponseViewModel
	{
		/// <summary>成功したか</summary>
		public bool IsSuccess { get; set; }

		/// <summary>メッセージ</summary>
		public string Message { get; set; }
	}
}
