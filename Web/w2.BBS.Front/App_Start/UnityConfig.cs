// (c) 2026 W2 Co.,Ltd.

using System;

using Unity;
using w2.FoundationDomain.DependencyInjections;

namespace w2.BBS.Front
{
	/// <summary>
	/// Unity DI 設定
	/// </summary>
	public static class UnityConfig
	{
		private static readonly Lazy<IUnityContainer> s_container = new(
			() =>
			{
				var container = new UnityContainer();
				RegisterTypes(container);
				return container;
			});

		/// <summary>設定済み Unity コンテナ</summary>
		public static IUnityContainer Container => s_container.Value;

		/// <summary>
		/// Unity コンテナへ型マッピングを登録
		/// </summary>
		/// <param name="container">Unity コンテナ</param>
		public static void RegisterTypes(UnityContainer container)
		{
			new DiContainerConfiguratorEntryPoint().Configure(container);
		}
	}
}
