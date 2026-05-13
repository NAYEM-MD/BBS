// (c) 2025 W2 Co.,Ltd.

using w2.SampleDomain.Dto.Sample;

namespace w2.SampleDomain.Domains.Sample
{
	/// <summary>
	/// サンプルモデル
	/// </summary>
	public class SampleModel
	{
		/// <summary>
		/// DTOからモデルを生成する
		/// </summary>
		/// <param name="dto">DTO</param>
		/// <returns>モデル</returns>
		internal static SampleModel CreateByDto(SampleDto dto)
		{
			return new SampleModel
			{
				Id = new SampleId(dto.Id),
				Name = new Name(dto.Name),
				Age = new Age(dto.Age)
			};
		}

		/// <summary>
		/// モデルからDTOを生成する
		/// </summary>
		/// <returns>DTO</returns>
		internal SampleDto CreateDto()
		{
			return new SampleDto
			{
				Id = this.Id.AsInt,
				Name = this.Name.AsString,
				Age = this.Age.Value,
			};
		}

		/// <summary>ID</summary>
		public SampleId Id { get; init; } = null!;
		/// <summary>名前</summary>
		public Name Name { get; init; } = null!;
		/// <summary>年齢</summary>
		public Age Age { get; init; } = null!;
	}
}
