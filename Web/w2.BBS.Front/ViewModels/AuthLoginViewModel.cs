namespace w2.BBS.Front.ViewModels
{
	public class AuthLoginViewModel : BaseViewModel
	{
		public string LoginId { get; set; }
		public string Password { get; set; }
		public string ErrorMessage { get; set; }
		public string InfoMessage { get; set; }
	}
}
