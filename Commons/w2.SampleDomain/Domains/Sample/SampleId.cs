// (c) 2025 W2 Co.,Ltd.

using System;

namespace w2.SampleDomain.Domains.Sample
{
	/// <summary>
	/// サンプルID
	/// </summary>
	public sealed record SampleId(int AsInt)
	{
		/// <inheritdoc />
		public override string ToString()
		{
			return this.AsInt.ToString();
		}
	}
}
