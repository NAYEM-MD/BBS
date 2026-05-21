// (c) 2026 W2 Co.,Ltd.

namespace w2.BBS.Front.ViewModels
{
	/// <summary>
	/// 掲示板返信モデル
	/// </summary>
	public class ForumReplyViewModel
	{
		/// <summary>返信ID</summary>
		public int ReplyId { get; set; }

		/// <summary>投稿ID</summary>
		public int PostId { get; set; }

		/// <summary>会員ID</summary>
		public int UserId { get; set; }

		/// <summary>会員名</summary>
		public string UserName { get; set; }

		/// <summary>本文</summary>
		public string Body { get; set; }

		/// <summary>編集可能か（将来用）</summary>
		public bool CanEdit { get; set; }
	}
}
