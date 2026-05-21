// (c) 2026 W2 Co.,Ltd.

using System.Web.Mvc;

namespace w2.BBS.Front
{
	/// <summary>
	/// ルーティング設定
	/// </summary>
	public class RouteConfig
	{
		/// <summary>
		/// ルーティングの登録
		/// </summary>
		/// <param name="routes">RouteCollection</param>
		public static void RegisterRoutes(System.Web.Routing.RouteCollection routes)
		{
			routes.RouteExistingFiles = false;

			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("Scripts/{*pathInfo}");
			routes.IgnoreRoute("{resource}.ashx");
			routes.IgnoreRoute("Theme/{*pathInfo}");

			routes.MapMvcAttributeRoutes();

			routes.MapRoute(
				name: "CatchAll",
				url: "{*url}",
				defaults: new
				{
					controller = "Error",
					action = "RedirectShortUrlOrDisplay404",
				}
			);
		}
	}
}
