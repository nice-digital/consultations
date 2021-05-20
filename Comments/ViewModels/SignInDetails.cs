namespace Comments.ViewModels
{
	public class SignInDetails : User
    {
	    public SignInDetails(User user, string signInURL, string registerURL) : base(user.AuthenticatedBy, user.AuthenticationType, user.DisplayName, user.UserId, user.OrganisationsAssignedAsLead, user.ValidatedSessions)
	    {
		    SignInURL = signInURL;
		    RegisterURL = registerURL;
	    }

		public string SignInURL { get; private set; }

	    public string RegisterURL { get; private set; }
	}
}
