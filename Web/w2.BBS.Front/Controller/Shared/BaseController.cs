// (c) 2026 W2 Co.,Ltd.

using Newtonsoft.Json;
using System;
using System.Text;
using System.Web.Mvc;
using w2.BBS.Front.Codes.Helper;
using w2.BBS.Front.ViewModels;
using w2.FoundationDomain.Domains.DateStrings;
using w2.FoundationDomain.Domains.Numeric;
using w2.TemplateEngine.TemplateEngines;
using w2.TemplateEngine.TemplateEngines.Fluid;
using w2.TemplateEngine.TemplateEngines.PhysicalPathRoutes;

namespace w2.BBS.Front.Controller.Shared
{
	/// <summary>
	/// MVC基底コントローラ
	/// </summary>
	public abstract class BaseController : System.Web.Mvc.Controller
	{
		private const string CONTENT_TYPE_HTML = "text/html";
		private const string CONTENT_TYPE_JSON = "application/json";

		/// <summary>
		/// セッションからログインユーザーIDを取得
		/// </summary>
		/// <returns>ログインユーザーID（未ログインなら null）</returns>
		protected int? GetLoginUserId()
		{
			var loginUserId = this.Session[FrontSession.SESSION_KEY_LOGIN_USER_ID];
			if (loginUserId is null)
			{
				return null;
			}

			return Convert.ToInt32(loginUserId);
		}

		/// <summary>
		/// ログインセッションクリア
		/// </summary>
		protected void ClearLoginSession()
		{
			this.Session.Remove(FrontSession.SESSION_KEY_LOGIN_USER_ID);
			this.Session.Remove(FrontSession.SESSION_KEY_LOGIN_USER_NAME);
		}

		/// <summary>
		/// ViewをレンダリングしたActionResultを返す
		/// </summary>
		/// <param name="viewFileVirtualPath">ビューファイルのパス</param>
		/// <param name="model">ViewModel</param>
		/// <returns>ActionResult</returns>
		protected new ActionResult View(string viewFileVirtualPath, object model = null)
		{
			this.Response.ContentEncoding = Encoding.UTF8;
			ITemplateRenderer templateRenderer = new FluidRenderer(new ThemeTemplatePhysicalPathRoute());

			var optionData = FluidOptionData.Create(
				NumericFormat.GetJpDefault(),
				NumericFormat.GetAll(),
				DateStringFormat.GetJpDefault(),
				DateStringFormat.GetAll(),
				DateTime.Now,
				this.TempData.Get<string>(TempDataKey.AntiCsrfFormToken));

			return new ContentResult
			{
				Content = templateRenderer.RenderByFile(viewFileVirtualPath, model ?? new BaseViewModel(), optionData),
				ContentEncoding = Encoding.UTF8,
				ContentType = CONTENT_TYPE_HTML,
			};
		}

		/// <summary>
		/// JSONをレンダリングしたActionResultを返す
		/// </summary>
		/// <param name="obj">オブジェクト</param>
		/// <returns>ActionResult</returns>
		protected ActionResult JsonForJs(object obj)
		{
			var json = JsonConvert.SerializeObject(obj);
			return base.Content(json, CONTENT_TYPE_JSON, Encoding.UTF8);
		}
	}
}
