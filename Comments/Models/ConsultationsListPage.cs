using System;
using System.Collections.Generic;
using System.Linq;
using Comments.ViewModels;

namespace Comments.Models
{
	public class ConsultationListRow
	{
		public ConsultationListRow(string title, DateTime startDate, DateTime endDate, int? responses, int consultationId, int? documentId, string chapterSlug, string gidReference,
			string productTypeName, bool hasCurrentUserEnteredCommentsOrAnsweredQuestions, bool hasCurrentUserSubmittedCommentsOrAnswers, string allowedRole,
			IList<OrganisationCode> organisationCodes, bool currentUserIsAuthorisedToViewOrganisationCodes)
		{
			Title = title;
			StartDate = startDate;
			EndDate = endDate;
			SubmissionCount = responses;
			ConsultationId = consultationId;
			DocumentId = documentId;
			ChapterSlug = chapterSlug;
			GidReference = gidReference;
			ProductTypeName = productTypeName;
			HasCurrentUserEnteredCommentsOrAnsweredQuestions = hasCurrentUserEnteredCommentsOrAnsweredQuestions;
			HasCurrentUserSubmittedCommentsOrAnswers = hasCurrentUserSubmittedCommentsOrAnswers;
			AllowedRole = allowedRole;
			OrganisationCodes = organisationCodes;
			CurrentUserIsAuthorisedToViewOrganisationCodes = currentUserIsAuthorisedToViewOrganisationCodes;
		}

		public string Title { get; private set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }

		public int? SubmissionCount { get; private set; }

		public int ConsultationId { get; private set; }
		public int? DocumentId { get; private set; }
		public string ChapterSlug { get; private set; }

		public string GidReference { get; private set; }
		public string ProductTypeName { get; private set; }

		/// <summary>
		/// Has the current user submitted comments or answers on this consultation.
		/// </summary>
		public bool HasCurrentUserEnteredCommentsOrAnsweredQuestions { get; private set; }

		/// <summary>
		/// if this is true then HasCurrentUserEnteredCommentsOrAnsweredQuestions must be true.
		/// </summary>
		public bool HasCurrentUserSubmittedCommentsOrAnswers { get; private set; }

		/// <summary>
		/// AllowedRole is in the indev feed. It tracks which teams work on which consultatations. If supplied, the Allowed role should match one from here: AppSettings.ConsultationListConfig.DownloadRoles.TeamRoles
		/// If supplied, then the download page will allow filtering for that role.
		/// </summary>
		public string AllowedRole { get; private set; }

		public bool IsOpen => StartDate <= DateTime.UtcNow && EndDate > DateTime.UtcNow;
		public bool IsClosed => EndDate < DateTime.UtcNow;
		public bool IsUpcoming => StartDate > DateTime.UtcNow;
		public bool Show { get; set; } = true;

		public IList<OrganisationCode> OrganisationCodes { get; private set; }

		private readonly bool CurrentUserIsAuthorisedToViewOrganisationCodes;
		public bool ShowShareWithOrganisationButton => IsOpen && CurrentUserIsAuthorisedToViewOrganisationCodes;
	}
}
