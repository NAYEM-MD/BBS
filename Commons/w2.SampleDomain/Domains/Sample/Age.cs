// (c) 2025 W2 Co.,Ltd.

using System;

namespace w2.SampleDomain.Domains.Sample
{
	/// <summary>
	/// 年齢
	/// </summary>
	public class Age
	{
		private int _value;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="value">年齢</param>
		public Age(int value)
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(value), "年齢は0以上でなければなりません。");
			}

			_value = value;
		}

		/// <summary>
		/// 成人済みか
		/// </summary>
		/// <returns>結果</returns>
		public bool IsAdult()
		{
			return _value >= 20;
		}

		/// <summary>
		/// 高齢者か
		/// </summary>
		/// <returns>結果</returns>
		public bool IsSenior()
		{
			return _value >= 65;
		}

		/// <summary>
		/// 子どもか
		/// </summary>
		/// <returns>結果</returns>
		public bool IsChild()
		{
			return _value < 20;
		}

		/// <summary>int値を取得</summary>
		public int Value => _value;
	}
}
