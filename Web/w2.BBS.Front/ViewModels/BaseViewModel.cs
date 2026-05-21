// (c) 2026 W2 Co.,Ltd.

using w2.FoundationDomain.Configurations;

namespace w2.BBS.Front.ViewModels
{
	/// <summary>
	/// ビューモデル基底クラス
	/// </summary>
	public class BaseViewModel
	{
		/// <summary>サイトのルートURL</summary>
		public string RootUrl => EnvironmentConfig.FrontRootPath;
	}
}
