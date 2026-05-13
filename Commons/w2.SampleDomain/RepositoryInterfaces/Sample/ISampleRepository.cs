using w2.SampleDomain.Domains.Sample;

namespace w2.SampleDomain.RepositoryInterfaces.Sample
{
	/// <summary>
	/// サンプルリポジトリ インターフェース
	/// </summary>
	public interface ISampleRepository
	{
		/// <summary>
		/// IDで取得
		/// </summary>
		/// <param name="id">ID</param>
		/// <returns>モデル</returns>
		public SampleModel? Get(SampleId id);
	}
}
