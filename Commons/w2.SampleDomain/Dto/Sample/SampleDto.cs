// (c) 2025 W2 Co.,Ltd.

using w2.FoundationDomain.Helpers;

namespace w2.SampleDomain.Dto.Sample
{
	/// <summary>
	/// サンプルDTO
	/// </summary>
	internal class SampleDto : IHashtableGeneratable
	{
		/// <summary>ID</summary>
		[HashtableAlias("id")]
		public int Id { get; set; } = int.MinValue;
		/// <summary>名前</summary>
		[HashtableAlias("name")]
		public string Name { get; set; } = null!;
		/// <summary>年齢</summary>
		[HashtableAlias("age")]
		public int Age { get; set; } = int.MinValue;
	}
}
