// (c) 2026 W2 Co.,Ltd.

using System.Web.Mvc;
using w2.BBS.Front.Controller.Shared;

namespace w2.BBS.Front.Controller
{
	/// <summary>
	/// トップページ
	/// </summary>
	public class IndexController : BaseController
	{
		/// <summary>
		/// ルートアクセス
		/// </summary>
		[HttpGet]
		[Route("~/")]
		public ActionResult Index()
		{
			if (this.Session[FrontSession.SESSION_KEY_LOGIN_USER_ID] is not null)
			{
				return this.Redirect("~/forum");
			}

			return this.Redirect("~/auth/login");
		}
	}
}
