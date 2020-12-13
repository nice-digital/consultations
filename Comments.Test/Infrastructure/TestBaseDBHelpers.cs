using Comments.Common;
using Comments.Models;
using Comments.ViewModels;
using System;
using System.Linq;
using Comment = Comments.Models.Comment;
using Location = Comments.Models.Location;

namespace Comments.Test.Infrastructure
{
	/// <summary>
	/// These functions come from the DB Helpers region of TestBase. The main difference is they're all static, and don't support not passing context in.
	/// </summary>
	public static class TestBaseDBHelpers
	{
		public static int AddLocation(ConsultationsContext passedInContext, string sourceURI, string order = "0")
		{
			var location = new Location(sourceURI, null, null, null, null, null, null, order, null, null, null);
			passedInContext.Location.Add(location);
			passedInContext.SaveChanges();
			return location.LocationId;
		}

		public static int AddComment(ConsultationsContext passedInContext, int locationId, string commentText, string createdByUserId, int status = (int)StatusName.Draft, int? organisationUserId = null, int? parentCommentId = null)
		{
			var comment = new Comment(locationId, createdByUserId, commentText, Guid.Empty.ToString(), location: null, statusId: status, status: null, organisationUserId, parentCommentId);
			passedInContext.Comment.Add(comment);
			passedInContext.SaveChanges();
			if (!passedInContext.Status.Any(s => s.StatusId.Equals(status)))
			{
				AddStatus(passedInContext, "adding status: " + status, status);
			}
			return comment.CommentId;
		}

		public static int AddStatus(ConsultationsContext passedInContext, string statusName, int statusId = (int)StatusName.Draft)
		{
			var statusModel = new Models.Status(statusName ?? "Draft", null, null){ StatusId = statusId};
			passedInContext.Status.Add(statusModel);
			passedInContext.SaveChanges();
			return statusModel.StatusId;
		}

		public static int AddOrganisationAuthorisationWithLocation(int organisationId, int consultationId, ConsultationsContext passedInContext, string userId = "someUserId", string collationCode = null)
		{
			var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);
			var locationId = AddLocation(passedInContext, sourceURI);
			passedInContext.SaveChanges();
			var organisationAuthorisation = new OrganisationAuthorisation(userId, DateTime.Now, organisationId, locationId, collationCode);
			passedInContext.OrganisationAuthorisation.Add(organisationAuthorisation);
			passedInContext.SaveChanges();
			return organisationAuthorisation.OrganisationAuthorisationId;
		}

		public static int AddOrganisationUser(ConsultationsContext passedInContext, int organisationAuthorisationId, Guid authorisationSession, DateTime? expirationDate, int organisationUserId = 0)
		{
			var organisationUser = new OrganisationUser(organisationAuthorisationId, authorisationSession, expirationDate ?? DateTime.Now.AddDays(28))
				{OrganisationUserId = organisationUserId};

			passedInContext.OrganisationUser.Add(organisationUser);
			passedInContext.SaveChanges();
			return organisationUser.OrganisationUserId;
		}
	}
}
