// (c) 2026 W2 Co.,Ltd.

namespace w2.BBS.Front
{
	/// <summary>
	/// BBS 共通 SQL 文（フロント）
	/// </summary>
	public static class BbsSql
	{
		/// <summary>ログインユーザー取得</summary>
		public const string SELECT_LOGIN_USER =
			@"SELECT TOP 1
				user_id,
				user_name
			FROM w2_User
			WHERE login_id = @login_id
			AND password = @password
			AND del_flg = 0";

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

		/// <summary>ユーザー論理削除（パスワード確認付き）</summary>
		public const string SOFT_DELETE_USER_WITH_PASSWORD =
			@"UPDATE w2_User
			SET del_flg = 1
			WHERE user_id = @user_id
			AND password = @password
			AND del_flg = 0";

		/// <summary>投稿の論理削除（管理者・投稿ID）</summary>
		public const string SOFT_DELETE_POST_BY_ID =
			@"UPDATE w2_ForumPost
			SET del_flg = 1
			WHERE post_id = @post_id
			AND del_flg = 0";

		/// <summary>自分の投稿の論理削除</summary>
		public const string SOFT_DELETE_OWN_POST =
			@"UPDATE w2_ForumPost
			SET del_flg = 1
			WHERE post_id = @post_id
			AND user_id = @user_id
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
