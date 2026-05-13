using System;

using Unity;
using w2.FoundationDomain.DependencyInjections;

namespace w2.BBS.Front
{
	/// <summary>
	/// Specifies the Unity configuration for the main container.
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

		/// <summary>
		/// Configured Unity Container.
		/// </summary>
		public static IUnityContainer Container => s_container.Value;

		/// <summary>
		/// Registers the type mappings with the Unity container.
		/// </summary>
		/// <param name="container">The unity container to configure.</param>
		/// <remarks>
		/// There is no need to register concrete types such as controllers or
		/// API controllers (unless you want to change the defaults), as Unity
		/// allows resolving a concrete type even if it was not previously
		/// registered.
		/// </remarks>
		public static void RegisterTypes(UnityContainer container)
		{
			new DiContainerConfiguratorEntryPoint().Configure(container);
		}
	}
}
