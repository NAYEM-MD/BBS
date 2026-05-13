// (c) 2023 W2 Co.,Ltd.

using System.Web.Mvc;
using w2.Common.Helper;
using w2.Common.Helper.Attribute;

namespace w2.BBS.Front.Codes.Helper
{
	/// <summary>
	/// コントローラー内 テンポラリーデータキー
	/// </summary>
	public enum TempDataKey
	{
		/// <summary>CSRF対策トークン</summary>
		[EnumTextName("antiCsrfFormToken")]
		AntiCsrfFormToken,
	}

	/// <summary>
	/// コントローラー内 テンポラリーデータキー 拡張クラス
	/// </summary>
	public static class TempDataDictionaryExtension
	{
		/// <summary>
		/// 存在チェック
		/// </summary>
		/// <param name="temp">対象</param>
		/// <param name="tempDataKey">キー</param>
		/// <returns>結果</returns>
		public static bool ExistCheck(this TempDataDictionary temp, TempDataKey tempDataKey)
		{
			return temp.ContainsKey(tempDataKey.ToText());
		}

		/// <summary>
		/// 取得
		/// </summary>
		/// <typeparam name="T">取得する対象の型</typeparam>
		/// <param name="temp">対象</param>
		/// <param name="tempDataKey">キー</param>
		/// <returns>内容</returns>
		public static T Get<T>(this TempDataDictionary temp, TempDataKey tempDataKey)
		{
			return (T)temp[key: tempDataKey.ToText()];
		}

		/// <summary>
		/// 設定
		/// </summary>
		/// <typeparam name="T">設定する対象の型</typeparam>
		/// <param name="temp">対象</param>
		/// <param name="tempDataKey">キー</param>
		/// <param name="value">内容</param>
		public static void Set<T>(this TempDataDictionary temp, TempDataKey tempDataKey, T value)
		{
			temp[key: tempDataKey.ToText()] = value;
		}
	}
}
