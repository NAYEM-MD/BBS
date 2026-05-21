// (c) 2026 W2 Co.,Ltd.

using System;

namespace w2.BBS.Front.ViewModels
{
	/// <summary>
	/// 掲示板投稿モデル
	/// </summary>
	public class ForumPostViewModel
	{
		/// <summary>投稿ID</summary>
		public int PostId { get; set; }

		/// <summary>会員ID</summary>
		public int UserId { get; set; }

		/// <summary>会員名</summary>
		public string UserName { get; set; }

		/// <summary>タイトル</summary>
		public string Title { get; set; }

		/// <summary>本文</summary>
		public string Body { get; set; }

		/// <summary>編集可能か</summary>
		public bool CanEdit { get; set; }

		/// <summary>返信一覧</summary>
		public ForumReplyViewModel[] Replies { get; set; } = Array.Empty<ForumReplyViewModel>();
	}
}
