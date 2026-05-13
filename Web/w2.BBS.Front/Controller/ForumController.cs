using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using w2.BBS.Front.Controller.Shared;
using w2.BBS.Front.ViewModels;
using w2.Common;

namespace w2.BBS.Front.Controller
{
	/// <summary>
	/// 掲示板
	/// </summary>
	public class ForumController : BaseController
	{
		private const string VIEW_FORUM = "forum.liquid";

		private const string SESSION_KEY_LOGIN_USER_ID = "LoginUserId";
		private const string SESSION_KEY_LOGIN_USER_NAME = "LoginUserName";

		private const int PAGE_SIZE = 20;

		private const string MESSAGE_LOGIN_REQUIRED = "ログインしてください。";
		private const string MESSAGE_POST_INPUT_REQUIRED = "タイトルと本文を入力してください。";
		private const string MESSAGE_POST_CREATED = "投稿しました。";
		private const string MESSAGE_POST_UPDATED = "投稿を更新しました。";
		private const string MESSAGE_POST_DELETED = "投稿を削除しました。";
		private const string MESSAGE_POST_FORBIDDEN = "この投稿は編集または削除できません。";

		private const string MESSAGE_REPLY_BODY_REQUIRED = "返信本文を入力してください。";
		private const string MESSAGE_REPLY_CREATED = "返信しました。";
		private const string MESSAGE_POST_NOT_FOUND = "対象の投稿が見つかりません。";

		private const string SQL_SELECT_POST_COUNT =
@"SELECT COUNT(1)
FROM w2_ForumPost
WHERE del_flg = 0";

		private const string SQL_SELECT_POST_LIST =
@"SELECT
	p.post_id,
	p.user_id,
	u.user_name,
	p.title,
	p.body
FROM w2_ForumPost p
INNER JOIN w2_User u ON
(
	p.user_id = u.user_id
	AND u.del_flg = 0
)
WHERE p.del_flg = 0
ORDER BY
	p.post_id DESC
OFFSET @offset ROWS
FETCH NEXT @page_size ROWS ONLY";

		private const string SQL_INSERT_POST =
@"INSERT w2_ForumPost
(
	user_id,
	title,
	body,
	del_flg
)
VALUES
(
	@user_id,
	@title,
	@body,
	0
)";

		private const string SQL_UPDATE_OWN_POST =
@"UPDATE w2_ForumPost
SET title = @title,
	body = @body
WHERE post_id = @post_id
	AND user_id = @user_id
	AND del_flg = 0";

		private const string SQL_SOFT_DELETE_OWN_POST =
@"UPDATE w2_ForumPost
SET del_flg = 1
WHERE post_id = @post_id
	AND user_id = @user_id
	AND del_flg = 0";

		private const string SQL_POST_EXISTS =
@"SELECT COUNT(1)
FROM w2_ForumPost
WHERE post_id = @post_id
	AND del_flg = 0";

		private const string SQL_INSERT_REPLY =
@"INSERT w2_ForumReply
(
	post_id,
	user_id,
	body,
	del_flg
)
VALUES
(
	@post_id,
	@user_id,
	@body,
	0
)";

		/// <summary>
		/// 掲示板トップ
		/// </summary>
		[HttpGet]
		[Route("~/forum")]
		public ActionResult Index()
		{
			var loginUserId = this.GetLoginUserId();
			if (loginUserId is null)
			{
				return this.Redirect("~/auth/login");
			}

			var model = new ForumIndexViewModel
			{
				LoginUserId = loginUserId.Value,
				LoginUserName = Convert.ToString(this.Session[SESSION_KEY_LOGIN_USER_NAME]),
			};

			return base.View(VIEW_FORUM, model);
		}

		/// <summary>
		/// 投稿一覧取得
		/// </summary>
		/// <param name="page">ページ番号</param>
		[HttpGet]
		[Route("~/api/forum/posts")]
		public ActionResult Posts(int page = 1)
		{
			var loginUserId = this.GetLoginUserId();
			if (loginUserId is null)
			{
				return this.JsonForJs(new ForumListResponseViewModel
				{
					IsSuccess = false,
					Message = MESSAGE_LOGIN_REQUIRED,
				});
			}

			var currentPage = (page < 1) ? 1 : page;
			var totalCount = this.GetPostCount();
			var pageCount = this.GetPageCount(totalCount);

			if (currentPage > pageCount)
			{
				currentPage = pageCount;
			}

			var posts = this.GetPostList(currentPage, loginUserId.Value);
			this.AttachRepliesToPosts(posts, loginUserId.Value);

			return this.JsonForJs(new ForumListResponseViewModel
			{
				IsSuccess = true,
				Posts = posts,
				Page = currentPage,
				PageCount = pageCount,
				TotalCount = totalCount,
			});
		}

		/// <summary>
		/// 投稿
		/// </summary>
		[HttpPost]
		[Route("~/api/forum/create")]
		public ActionResult Create(string title, string body)
		{
			var loginUserId = this.GetLoginUserId();
			if (loginUserId is null)
			{
				return this.JsonForJs(new ForumActionResponseViewModel
				{
					IsSuccess = false,
					Message = MESSAGE_LOGIN_REQUIRED,
				});
			}

			var inputTitle = (title ?? string.Empty).Trim();
			var inputBody = (body ?? string.Empty).Trim();

			if (string.IsNullOrEmpty(inputTitle)
				|| string.IsNullOrEmpty(inputBody))
			{
				return this.JsonForJs(new ForumActionResponseViewModel
				{
					IsSuccess = false,
					Message = MESSAGE_POST_INPUT_REQUIRED,
				});
			}

			this.InsertPost(loginUserId.Value, inputTitle, inputBody);

			return this.JsonForJs(new ForumActionResponseViewModel
			{
				IsSuccess = true,
				Message = MESSAGE_POST_CREATED,
			});
		}

		/// <summary>
		/// 投稿更新
		/// </summary>
		[HttpPost]
		[Route("~/api/forum/update")]
		public ActionResult Update(int postId, string title, string body)
		{
			var loginUserId = this.GetLoginUserId();
			if (loginUserId is null)
			{
				return this.JsonForJs(new ForumActionResponseViewModel { IsSuccess = false, Message = MESSAGE_LOGIN_REQUIRED });
			}

			var inputTitle = (title ?? string.Empty).Trim();
			var inputBody = (body ?? string.Empty).Trim();
			if (string.IsNullOrEmpty(inputTitle) || string.IsNullOrEmpty(inputBody))
			{
				return this.JsonForJs(new ForumActionResponseViewModel { IsSuccess = false, Message = MESSAGE_POST_INPUT_REQUIRED });
			}

			var updated = this.UpdateOwnPost(loginUserId.Value, postId, inputTitle, inputBody);
			if (updated == false)
			{
				return this.JsonForJs(new ForumActionResponseViewModel { IsSuccess = false, Message = MESSAGE_POST_FORBIDDEN });
			}

			return this.JsonForJs(new ForumActionResponseViewModel { IsSuccess = true, Message = MESSAGE_POST_UPDATED });
		}

		/// <summary>
		/// 投稿削除（論理削除）
		/// </summary>
		[HttpPost]
		[Route("~/api/forum/delete")]
		public ActionResult Delete(int postId)
		{
			var loginUserId = this.GetLoginUserId();
			if (loginUserId is null)
			{
				return this.JsonForJs(new ForumActionResponseViewModel { IsSuccess = false, Message = MESSAGE_LOGIN_REQUIRED });
			}

			var deleted = this.DeleteOwnPost(loginUserId.Value, postId);
			if (deleted == false)
			{
				return this.JsonForJs(new ForumActionResponseViewModel { IsSuccess = false, Message = MESSAGE_POST_FORBIDDEN });
			}

			return this.JsonForJs(new ForumActionResponseViewModel { IsSuccess = true, Message = MESSAGE_POST_DELETED });
		}

		/// <summary>
		/// 返信投稿
		/// </summary>
		[HttpPost]
		[Route("~/api/forum/reply")]
		public ActionResult CreateReply(int postId, string body)
		{
			var loginUserId = this.GetLoginUserId();
			if (loginUserId is null)
			{
				return this.JsonForJs(new ForumActionResponseViewModel { IsSuccess = false, Message = MESSAGE_LOGIN_REQUIRED });
			}

			var inputBody = (body ?? string.Empty).Trim();
			if (string.IsNullOrEmpty(inputBody))
			{
				return this.JsonForJs(new ForumActionResponseViewModel { IsSuccess = false, Message = MESSAGE_REPLY_BODY_REQUIRED });
			}

			if (this.PostExists(postId) == false)
			{
				return this.JsonForJs(new ForumActionResponseViewModel { IsSuccess = false, Message = MESSAGE_POST_NOT_FOUND });
			}

			this.InsertReply(postId, loginUserId.Value, inputBody);

			return this.JsonForJs(new ForumActionResponseViewModel { IsSuccess = true, Message = MESSAGE_REPLY_CREATED });
		}

		private int? GetLoginUserId()
		{
			var loginUserId = this.Session[SESSION_KEY_LOGIN_USER_ID];
			if (loginUserId is null)
			{
				return null;
			}

			return Convert.ToInt32(loginUserId);
		}

		private int GetPostCount()
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_POST_COUNT, connection))
				{
					return Convert.ToInt32(command.ExecuteScalar());
				}
			}
		}

		private ForumPostViewModel[] GetPostList(int page, int loginUserId)
		{
			var postList = new List<ForumPostViewModel>();
			var offset = (page - 1) * PAGE_SIZE;

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_POST_LIST, connection))
				{
					command.Parameters.AddWithValue("@offset", offset);
					command.Parameters.AddWithValue("@page_size", PAGE_SIZE);

					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var postUserId = Convert.ToInt32(reader["user_id"]);

							postList.Add(new ForumPostViewModel
							{
								PostId = Convert.ToInt32(reader["post_id"]),
								UserId = postUserId,
								UserName = Convert.ToString(reader["user_name"]),
								Title = Convert.ToString(reader["title"]),
								Body = Convert.ToString(reader["body"]),
								CanEdit = (postUserId == loginUserId),
								Replies = Array.Empty<ForumReplyViewModel>(),
							});
						}
					}
				}
			}

			return postList.ToArray();
		}

		private void InsertPost(int loginUserId, string title, string body)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_INSERT_POST, connection))
				{
					command.Parameters.AddWithValue("@user_id", loginUserId);
					command.Parameters.AddWithValue("@title", title);
					command.Parameters.AddWithValue("@body", body);

					command.ExecuteNonQuery();
				}
			}
		}

		private bool UpdateOwnPost(int loginUserId, int postId, string title, string body)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_UPDATE_OWN_POST, connection))
				{
					command.Parameters.AddWithValue("@post_id", postId);
					command.Parameters.AddWithValue("@user_id", loginUserId);
					command.Parameters.AddWithValue("@title", title);
					command.Parameters.AddWithValue("@body", body);
					return (command.ExecuteNonQuery() > 0);
				}
			}
		}

		private bool DeleteOwnPost(int loginUserId, int postId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SOFT_DELETE_OWN_POST, connection))
				{
					command.Parameters.AddWithValue("@post_id", postId);
					command.Parameters.AddWithValue("@user_id", loginUserId);

					return (command.ExecuteNonQuery() > 0);
				}
			}
		}

		private bool PostExists(int postId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_POST_EXISTS, connection))
				{
					command.Parameters.AddWithValue("@post_id", postId);

					return Convert.ToInt32(command.ExecuteScalar()) > 0;
				}
			}
		}

		private void InsertReply(int postId, int loginUserId, string body)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_INSERT_REPLY, connection))
				{
					command.Parameters.AddWithValue("@post_id", postId);
					command.Parameters.AddWithValue("@user_id", loginUserId);
					command.Parameters.AddWithValue("@body", body);

					command.ExecuteNonQuery();
				}
			}
		}

		private void AttachRepliesToPosts(ForumPostViewModel[] posts, int loginUserId)
		{
			if (posts is null || posts.Length == 0)
			{
				return;
			}

			var paramNames = new List<string>();

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand())
				{
					command.Connection = connection;

					for (var i = 0; i < posts.Length; i++)
					{
						var name = "@p" + i;
						paramNames.Add(name);
						command.Parameters.AddWithValue(name, posts[i].PostId);
					}

					command.CommandText =
						@"SELECT r.reply_id, r.post_id, r.user_id, u.user_name, r.body
FROM w2_ForumReply r
INNER JOIN w2_User u ON
(
	r.user_id = u.user_id
	AND u.del_flg = 0
)
WHERE r.del_flg = 0
	AND r.post_id IN (" + string.Join(",", paramNames) + @")
ORDER BY
	r.reply_id ASC";

					var byPost = new Dictionary<int, List<ForumReplyViewModel>>();

					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var pid = Convert.ToInt32(reader["post_id"]);
							var uid = Convert.ToInt32(reader["user_id"]);

							if (byPost.ContainsKey(pid) == false)
							{
								byPost[pid] = new List<ForumReplyViewModel>();
							}

							byPost[pid].Add(new ForumReplyViewModel
							{
								ReplyId = Convert.ToInt32(reader["reply_id"]),
								PostId = pid,
								UserId = uid,
								UserName = Convert.ToString(reader["user_name"]),
								Body = Convert.ToString(reader["body"]),
								CanEdit = (uid == loginUserId),
							});
						}
					}

					for (var i = 0; i < posts.Length; i++)
					{
						var p = posts[i];
						if (byPost.TryGetValue(p.PostId, out var list))
						{
							p.Replies = list.ToArray();
						}
						else
						{
							p.Replies = Array.Empty<ForumReplyViewModel>();
						}
					}
				}
			}
		}

		private int GetPageCount(int totalCount)
		{
			if (totalCount == 0)
			{
				return 1;
			}

			return (int)Math.Ceiling((decimal)totalCount / PAGE_SIZE);
		}
	}
}
