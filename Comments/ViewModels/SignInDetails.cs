using System;

namespace Comments.ViewModels
{
    public class SignInDetails : User
    {
	    public SignInDetails(bool isAuthorised, string displayName, Guid? userId, string signInURL, string registerURL)
			: base(isAuthorised, displayName, userId)
	    {
		    SignInURL = signInURL;
		    RegisterURL = registerURL;
	    }
	    public SignInDetails(User user, string signInURL, string registerURL)
		    : base(user.IsAuthorised, user.DisplayName, user.UserId)
	    {
		    SignInURL = signInURL;
		    RegisterURL = registerURL;
	    }

		public string SignInURL { get; private set; }

	    public string RegisterURL { get; private set; }
	}
}
