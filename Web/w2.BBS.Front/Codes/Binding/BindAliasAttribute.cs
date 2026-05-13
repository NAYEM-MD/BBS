// (c) 2023 W2 Co.,Ltd.

using System;

namespace w2.BBS.Front.Codes.Binding
{
	/// <summary>
	/// 別名バインド属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public sealed class BindAliasAttribute : Attribute
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="alias">エイリアス</param>
		public BindAliasAttribute(string alias)
		{
			this.Alias = alias;
		}

		/// <summary>エイリアス</summary>
		public string Alias { get; }
	}
}
