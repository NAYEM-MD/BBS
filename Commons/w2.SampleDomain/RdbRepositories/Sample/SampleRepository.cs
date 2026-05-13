// (c) 2025 W2 Co.,Ltd.

using System.Linq;
using w2.FoundationDomain.Repositories;
using w2.SampleDomain.Domains.Sample;
using w2.SampleDomain.Dto.Sample;
using w2.SampleDomain.RepositoryInterfaces.Sample;

namespace w2.SampleDomain.RdbRepositories.Sample
{
	/// <summary>
	/// サンプルリポジトリ
	/// </summary>
	internal class SampleRepository : ISampleRepository
	{
		private readonly ISqlRepository _repository;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SampleRepository(ISqlRepository repository)
		{
			_repository = repository;
		}

		/// <summary>
		/// IDで取得
		/// </summary>
		/// <param name="id">ID</param>
		/// <returns>モデル</returns>
		public SampleModel? Get(SampleId id)
		{
			var dto = _repository.GetWithBuilder<SampleDto>(
				f => f.Query("w2_SampleTable").Where("id", id.AsInt)).FirstOrDefault();
			return dto is not null ? SampleModel.CreateByDto(dto) : null;
		}
	}
}
