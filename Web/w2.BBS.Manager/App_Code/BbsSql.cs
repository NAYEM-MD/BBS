// (c) 2026 W2 Co.,Ltd.

namespace w2.BBS.Manager
{
	/// <summary>
	/// BBS 共通 SQL 文（管理画面）
	/// </summary>
	public static class BbsSql
	{
		/// <summary>ユーザー投稿の論理削除</summary>
		public const string SOFT_DELETE_USER_POSTS =
			@"UPDATE w2_ForumPost
			SET del_flg = 1
			WHERE user_id = @user_id
			AND del_flg = 0";

		/// <summary>ユーザー返信の論理削除</summary>
		public const string SOFT_DELETE_USER_REPLIES =
			@"UPDATE w2_ForumReply
			SET del_flg = 1
			WHERE user_id = @user_id
			AND del_flg = 0";

		/// <summary>ユーザーの論理削除</summary>
		public const string SOFT_DELETE_USER =
			@"UPDATE w2_User
			SET del_flg = 1
			WHERE user_id = @user_id
			AND del_flg = 0";

		/// <summary>投稿の論理削除</summary>
		public const string SOFT_DELETE_POST_BY_ID =
			@"UPDATE w2_ForumPost
			SET del_flg = 1
			WHERE post_id = @post_id
			AND del_flg = 0";

		/// <summary>投稿に紐づく返信の論理削除</summary>
		public const string SOFT_DELETE_REPLIES_BY_POST =
			@"UPDATE w2_ForumReply
			SET del_flg = 1
			WHERE post_id = @post_id
			AND del_flg = 0";

		/// <summary>返信の論理削除</summary>
		public const string SOFT_DELETE_REPLY =
			@"UPDATE w2_ForumReply
			SET del_flg = 1
			WHERE reply_id = @reply_id
			AND del_flg = 0";
	}
}
