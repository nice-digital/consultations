using Comments.Common;
using Comments.Configuration;
using Comments.Models;
using Comments.ViewModels;
using NICE.Feeds;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using Location = Comments.Models.Location;

namespace Comments.Services
{
	public interface IExportService
	{
		(IEnumerable<Models.Comment> comment, IEnumerable<Models.Answer> answer, IEnumerable<Models.Question> question, Validate valid) GetAllDataForConsultation(int consultationId);

		(IEnumerable<Models.Comment> comment, IEnumerable<Models.Answer> answer, IEnumerable<Models.Question> question, Validate valid) GetAllDataForConsultationForCurrentUser(int consultationId);

		(IEnumerable<Models.Comment> comment, IEnumerable<Models.Answer> answer, IEnumerable<Models.Question> question, Validate valid) GetDataSubmittedToLeadForConsultation(int consultationId);
		(string ConsultationName, string DocumentName, string ChapterName) GetLocationData(Comments.Models.Location location);
		string GetConsultationName(Location location);
		IEnumerable<OrganisationUser> GetOrganisationUsersByOrganisationUserIds(IEnumerable<int> organisationUserIds);
	}

    public class ExportService : IExportService
    {
	    private readonly ConsultationsContext _context;
	    private readonly IUserService _userService;
	    private readonly IConsultationService _consultationService;
	    private readonly IFeedService _feedService;

		public ExportService(ConsultationsContext consultationsContext, IUserService userService, IConsultationService consultationService, IFeedService feedService)
	    {
		    var user = userService.GetCurrentUser();
		    if (!user.IsAuthenticatedByAnyMechanism)
		    {
			    throw new AuthenticationException("User not authenticated");
		    }

			_context = consultationsContext;
		    _userService = userService;
		    _consultationService = consultationService;
		    _feedService = feedService;
	    }

	    public (IEnumerable<Models.Comment> comment, IEnumerable<Models.Answer> answer, IEnumerable<Models.Question> question, Validate valid) GetAllDataForConsultation(int consultationId)
	    {
		    var userRoles = _userService.GetUserRoles().ToList();
		    var isAdminUser = userRoles.Any(role => AppSettings.ConsultationListConfig.DownloadRoles.AdminRoles.Contains(role));
			var consultation = _feedService.GetConsultationList().Single(c => c.ConsultationId.Equals(consultationId));
		    if (!isAdminUser && !userRoles.Contains(consultation.AllowedRole))
		    {
				return (null, null, null, new Validate(valid: false, unauthenticated: true, message: $"User does not have access to download this type of consultation."));
			}

			var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);
		    var commentsInDB = _context.GetAllSubmittedCommentsForURI(sourceURI);
		    var answersInDB = _context.GetAllSubmittedAnswersForURI(sourceURI);
		    var questionsInDB = _context.GetUnansweredQuestionsForURI(sourceURI);

			if (commentsInDB == null && answersInDB == null && questionsInDB == null)
				return (null, null, null, new Validate(valid: false, notFound: true, message: $"Consultation id:{consultationId} not found trying to get all data for consultation"));

			return (commentsInDB, answersInDB, questionsInDB, new Validate(true));
	    }

	    public (IEnumerable<Models.Comment> comment, IEnumerable<Models.Answer> answer, IEnumerable<Models.Question> question, Validate valid) GetDataSubmittedToLeadForConsultation(int consultationId)
	    {
		    var user = _userService.GetCurrentUser();
		    var isLead = user.OrganisationsAssignedAsLead.Any();
		    if (!isLead)
		    {
			    return (null, null, null, new Validate(valid: false, unauthenticated: true, message: $"User does not have access to download responses submitted to organisation leads."));
			}

		    var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);
		    var commentsInDB = _context.GetCommentsSubmittedToALeadForURI(sourceURI);
		    var answersInDB = _context.GetAnswersSubmittedToALeadForURI(sourceURI);
		    var questionsInDB = _context.GetUsersUnansweredQuestionsForURI(sourceURI); // TODO: This is wrong, but for now will pull through something rightish.

		    if (commentsInDB == null && answersInDB == null && questionsInDB == null)
			    return (null, null, null, new Validate(valid: false, notFound: true, message: $"Consultation id:{consultationId} not found trying to get all data for consultation"));

		    return (commentsInDB, answersInDB, questionsInDB, new Validate(true));
	    }

		public (IEnumerable<Models.Comment> comment, IEnumerable<Models.Answer> answer, IEnumerable<Models.Question> question, Validate valid) GetAllDataForConsultationForCurrentUser(int consultationId)
	    {

		    var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);
		    var commentsInDB = _context.GetUsersCommentsForURI(sourceURI);
		    var answersInDB = _context.GetUsersAnswersForURI(sourceURI);
		    var questionsInDB = _context.GetUsersUnansweredQuestionsForURI(sourceURI);

		    if (commentsInDB == null && answersInDB == null && questionsInDB == null)
			    return (null, null, null, new Validate(valid: false, notFound: true, message: $"Consultation id:{consultationId} not found trying to get all data for consultation"));

		    return (commentsInDB, answersInDB, questionsInDB, new Validate(true));
	    }

		public (string ConsultationName, string DocumentName, string ChapterName) GetLocationData(Location location)
	    {
		    var sourceURI = location.SourceURI;
		    ConsultationsUriElements URIElements = ConsultationsUri.ParseConsultationsUri(sourceURI);

		    var consultationDetails = _consultationService.GetConsultation(URIElements.ConsultationId, BreadcrumbType.None, useFilters:false);
		    var documents = _consultationService.GetDocuments(URIElements.ConsultationId).documents;
		    var documentDetail = documents.FirstOrDefault(x => x.DocumentId == URIElements.DocumentId);

		    Chapter chapterDetail = null;
		    if (URIElements.ChapterSlug != null)
			    chapterDetail = documentDetail?.Chapters.First(c => c.Slug == URIElements.ChapterSlug);

		    return (consultationDetails.ConsultationName, documentDetail?.Title, chapterDetail?.Slug);
	    }

	    public string GetConsultationName(Location location)
	    {
		    var sourceURI = location.SourceURI;
		    ConsultationsUriElements URIElements = ConsultationsUri.ParseConsultationsUri(sourceURI);

		    var consultationDetails = _consultationService.GetConsultation(URIElements.ConsultationId,  BreadcrumbType.None, useFilters:false);
		    return consultationDetails.ConsultationName;
	    }

	    public IEnumerable<OrganisationUser> GetOrganisationUsersByOrganisationUserIds(IEnumerable<int> organisationUserIds)
	    {
		    return _context.GetOrganisationUsersByOrganisationUserIds(organisationUserIds);
	    }
	}
}
