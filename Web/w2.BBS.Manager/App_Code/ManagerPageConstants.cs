// (c) 2026 W2 Co.,Ltd.

namespace w2.BBS.Manager
{
	/// <summary>
	/// 管理画面ページ定数
	/// </summary>
	public static class ManagerPageConstants
	{
		/// <summary>セッションキー：セッションID固定用ダミー</summary>
		public const string SESSION_KEY_DUMMY = "SESSION_KEY_DUMMY";

		/// <summary>ViewStateキー：ユーザーID</summary>
		public const string VIEWSTATE_KEY_USER_ID = "VIEWSTATE_KEY_USER_ID";

		/// <summary>コマンド名：削除（投稿・返信一覧）</summary>
		public const string COMMAND_DEL = "Del";

		/// <summary>コマンド名：投稿削除（ユーザー詳細）</summary>
		public const string COMMAND_DEL_POST = "DelPost";

		/// <summary>コマンド名：返信削除（ユーザー詳細）</summary>
		public const string COMMAND_DEL_REPLY = "DelReply";
	}
}
