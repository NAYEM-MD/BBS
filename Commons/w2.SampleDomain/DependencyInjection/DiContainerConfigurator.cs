// (c) 2025 W2 Co.,Ltd.

using Unity;
using w2.FoundationDomain.DependencyInjections;
using w2.SampleDomain.RdbRepositories.Sample;
using w2.SampleDomain.RepositoryInterfaces.Sample;

namespace w2.SampleDomain.DependencyInjection
{
	/// <summary>
	/// DIコンテナコンフィギュレータ
	/// </summary>
	public class DiContainerConfigurator : IDiContainerConfigurator
	{
		/// <inheritdoc />
		public UnityContainer Configure(UnityContainer container)
		{
			container.RegisterType<ISampleRepository, SampleRepository>();
			return container;
		}
	}
}
