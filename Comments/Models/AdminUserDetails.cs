using System;

namespace Comments.Models
{
	/// <summary>
	/// This class is just here to support the data export for idam. it's to be removed after idam has been integrated.
	/// </summary>
	public class AdminUserDetails
	{
		public Guid UserId { get; }
		public string DisplayName { get; }
		public string EmailAddress { get; }

		public AdminUserDetails(Guid userId, string displayName, string emailAddress)
		{
			UserId = userId;
			DisplayName = displayName;
			EmailAddress = emailAddress;
		}
	}
}
