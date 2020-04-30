namespace Comments.ViewModels
{
	public class DownloadUser : User
	{
		public DownloadUser(bool isAdminUser, bool isTeamUser, User user) : base(user.IsAuthorised, user.DisplayName, user.UserId, user.OrganisationName)
		{
			IsAdminUser = isAdminUser;
			IsTeamUser = isTeamUser;
		}

		/// <summary>
		/// true if the user has one of the roles from ConsultationList.DownloadRoles.AdminRoles 
		/// </summary>
		public bool IsAdminUser { get; private set; }


		/// <summary>
		/// true if the user has one of the roles from ConsultationList.DownloadRoles.TeamRoles AND is not an admin
		/// </summary>
		public bool IsTeamUser { get; private set; }
	}
}
