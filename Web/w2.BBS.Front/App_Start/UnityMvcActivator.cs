// (c) 2026 W2 Co.,Ltd.

using System.Linq;
using System.Web.Mvc;

using Unity.AspNet.Mvc;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(w2.BBS.Front.UnityMvcActivator), nameof(w2.BBS.Front.UnityMvcActivator.Start))]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(w2.BBS.Front.UnityMvcActivator), nameof(w2.BBS.Front.UnityMvcActivator.Shutdown))]

namespace w2.BBS.Front
{
	/// <summary>
	/// Unity と ASP.NET MVC の統合
	/// </summary>
	public static class UnityMvcActivator
	{
		/// <summary>
		/// アプリケーション開始時に Unity を統合
		/// </summary>
		public static void Start()
		{
			FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
			FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(UnityConfig.Container));

			DependencyResolver.SetResolver(new UnityDependencyResolver(UnityConfig.Container));
		}

		/// <summary>
		/// アプリケーション終了時に Unity コンテナを破棄
		/// </summary>
		public static void Shutdown()
		{
			UnityConfig.Container.Dispose();
		}
	}
}
