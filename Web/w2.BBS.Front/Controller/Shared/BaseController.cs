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
				ContentType = "text/html",
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
			return Content(json, "application/json", Encoding.UTF8);
		}
	}
}
