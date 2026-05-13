// (c) 2025 W2 Co.,Ltd.

using w2.SampleDomain.Domains.Sample;
using w2.SampleDomain.RepositoryInterfaces.Sample;

namespace w2.SampleDomain.Services.Sample
{
	/// <summary>
	/// サンプルサービス
	/// </summary>
	public class SampleService
	{
		private readonly ISampleRepository _sampleRepository;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SampleService(ISampleRepository sampleRepository)
		{
			_sampleRepository = sampleRepository;
		}

		/// <summary>
		/// IDで取得
		/// </summary>
		/// <param name="id">ID</param>
		/// <returns>モデル</returns>
		public SampleModel? Get(SampleId id)
		{
			return _sampleRepository.Get(id);
		}
	}
}
