using System;

namespace w2.BBS.Front.ViewModels
{
	/// <summary>
	/// 掲示板1件取得APIレスポンス
	/// </summary>
	public class ForumPostDetailResponseViewModel
	{
		/// <summary>成功したか</summary>
		public bool IsSuccess { get; set; }

		/// <summary>メッセージ</summary>
		public string Message { get; set; }

		/// <summary>投稿（返信付き）</summary>
		public ForumPostViewModel Post { get; set; }
	}
}
