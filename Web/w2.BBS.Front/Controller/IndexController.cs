using SqlKata.Compilers;
using SqlKata.Execution;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using w2.BBS.Front.Controller.Shared;
using w2.BBS.Front.ViewModels;
using w2.Common;
using w2.Common.Sql;

namespace w2.BBS.Front.Controller
{
	/// <summary>
	/// トップページコントローラ
	/// </summary>
	public class IndexController : BaseController
	{
		/// <summary>
		/// トップページ
		/// </summary>
		/// <returns>アクションリザルト</returns>
		[Route("~/")]
		public ActionResult Index()
		{
			if (this.Session["LoginUserId"] != null)
			{
				return Redirect("~/forum");
			}
			return Redirect("~/auth/login");
		}

		/// <summary>
		/// メッセージ取得
		/// </summary>
		/// <returns>アクションリザルト</returns>
		[Route("~/get-message")]
		[HttpGet]
		public ActionResult GetMessage()
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				var queryFactory = new QueryFactory(connection, new SqlServerCompiler());
				var query = queryFactory.Query("w2_Sample").Select("message_text");

				var message = query.Get<string>().FirstOrDefault() ?? "No message";

				return JsonForJs(
					new
					{
						Message = message,
					});
			}
		}

		/// <summary>
		/// メッセージ更新２
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <returns>アクションリザルト</returns>
		[Route("~/update-message")]
		[HttpPost]
		public ActionResult UpdateMessage(string message)
		{
			// クエリビルダーを利用するパターンです。

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				var queryFactory = new QueryFactory(connection, new SqlServerCompiler());
				var query = queryFactory.Query("w2_Sample").AsUpdate(
					new Dictionary<string, object>
					{
						{ "message_text", message }
					});

				queryFactory.Execute(query);

				return JsonForJs(new {});
			}
		}

		/// <summary>
		/// メッセージ更新
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <returns>アクションリザルト</returns>
		[Route("~/update-message2")]
		[HttpPost]
		public ActionResult UpdateMessage2(string message)
		{
			// XMLに記述したSQLステートメントを利用するパターンです。

			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("Sample", "UpdateMessage"))
			{
				accessor.OpenConnection();
				statement.ExecStatement(
					accessor,
					new Hashtable
					{
						{ "message_text", message }
					});

				return JsonForJs(new {});
			}
		}
	}
}
