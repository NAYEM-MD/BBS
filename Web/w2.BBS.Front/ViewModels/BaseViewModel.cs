using w2.FoundationDomain.Configurations;

namespace w2.BBS.Front.ViewModels
{
	public class BaseViewModel
	{
		public string RootUrl => EnvironmentConfig.FrontRootPath;
	}
}
