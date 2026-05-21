// (c) 2026 W2 Co.,Ltd.

using System;

namespace w2.BBS.Front.ViewModels
{
	/// <summary>
	/// 掲示板一覧レスポンスモデル
	/// </summary>
	public class ForumListResponseViewModel
	{
		/// <summary>成功したか</summary>
		public bool IsSuccess { get; set; }

		/// <summary>メッセージ</summary>
		public string Message { get; set; }

		/// <summary>投稿一覧</summary>
		public ForumPostViewModel[] Posts { get; set; } = Array.Empty<ForumPostViewModel>();

		/// <summary>現在ページ</summary>
		public int Page { get; set; }

		/// <summary>総ページ数</summary>
		public int PageCount { get; set; }

		/// <summary>総件数</summary>
		public int TotalCount { get; set; }
	}
}
