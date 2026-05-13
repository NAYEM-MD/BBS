// (c) 2023 W2 Co.,Ltd.

using System;
using System.ComponentModel;

namespace w2.BBS.Front.Codes.Binding
{
	/// <summary>
	/// エイリアスプロパティ記述子
	/// </summary>
	public sealed class AliasedPropertyDescriptor : PropertyDescriptor
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="alias">エイリアス</param>
		/// <param name="inner">プロパティ</param>
		public AliasedPropertyDescriptor(string alias, PropertyDescriptor inner)
			: base(alias, attrs: null)
		{
			this.Inner = inner;
		}

		/// <inheritdoc />
		public override bool CanResetValue(object component)
		{
			return this.Inner.CanResetValue(component);
		}

		/// <inheritdoc />
		public override object GetValue(object component)
		{
			return this.Inner.GetValue(component);
		}

		/// <inheritdoc />
		public override void ResetValue(object component)
		{
			this.Inner.ResetValue(component);
		}

		/// <inheritdoc />
		public override void SetValue(object component, object value)
		{
			this.Inner.SetValue(component, value);
		}

		/// <inheritdoc />
		public override bool ShouldSerializeValue(object component)
		{
			return this.Inner.ShouldSerializeValue(component);
		}

		/// <inheritdoc />
		public override Type ComponentType
		{
			get { return this.Inner.ComponentType; }
		}
		/// <inheritdoc />
		public override bool IsReadOnly
		{
			get { return this.Inner.IsReadOnly; }
		}
		/// <inheritdoc />
		public override Type PropertyType
		{
			get { return this.Inner.PropertyType; }
		}
		/// <summary>プロパティ</summary>
		public PropertyDescriptor Inner { get; }
	}
}
