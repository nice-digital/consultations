using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace Comments.ViewModels
{
	public class DownloadUser : User
	{
		public DownloadUser() {}

		public DownloadUser(bool isAdminUser, bool isTeamUser, User user, List<string> teamNames) : base(user.IsAuthorised, user.DisplayName, user.UserId, user.OrganisationsAssignedAsLead, user.OrganisationUserId)
		{
			IsAdminUser = isAdminUser;
			IsTeamUser = isTeamUser;
			TeamNames = teamNames;
		}

		/// <summary>
		/// true if the user has one of the roles from ConsultationList.DownloadRoles.AdminRoles
		/// </summary>
		public bool IsAdminUser { get; private set; }


		/// <summary>
		/// true if the user has one of the roles from ConsultationList.DownloadRoles.TeamRoles AND is not an admin
		/// </summary>
		public bool IsTeamUser { get; private set; }

		private List<string> _teamNames;
		/// <summary>
		/// the code in the get accessor adds a space making team names like "ABCTeam" into "ABC Team".
		/// </summary>
		public List<string> TeamNames
		{
			get
			{
				return _teamNames;
			}
			private set
			{
				_teamNames = value;
				if (_teamNames != null && _teamNames.Any())
				{
					for (var index = 0; index < _teamNames.Count; index++)
					{
						var teamIndex = _teamNames[index].IndexOf("Team", StringComparison.OrdinalIgnoreCase);
						if (teamIndex != -1)
						{
							_teamNames[index] = $"{_teamNames[index].Substring(0, teamIndex)} {_teamNames[index].Substring(teamIndex)}";
						}
					}
				}
			}
		}
	}
}
