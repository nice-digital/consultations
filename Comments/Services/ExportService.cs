using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using Comments.Common;
using Comments.Models;
using Comments.ViewModels;
using Microsoft.AspNetCore.Http;
using Location = Comments.Models.Location;

namespace Comments.Services
{
	public interface IExportService
	{
		(IEnumerable<Models.Comment> comment, IEnumerable<Models.Answer> answer, IEnumerable<Models.Question> question, Validate valid) GetAllDataForConsulation(int consultationId);

		(IEnumerable<Models.Comment> comment, IEnumerable<Models.Answer> answer, IEnumerable<Models.Question> question, Validate valid) GetAllDataForConsulationForCurrentUser(int consultationId);
		(string ConsultationName, string DocumentName, string ChapterName) GetLocationData(Comments.Models.Location location);
	}

    public class ExportService : IExportService
    {
	    private readonly ConsultationsContext _context;
	    private readonly IConsultationService _consultationService;
	    private readonly ClaimsPrincipal _niceUser;

		public ExportService(ConsultationsContext consultationsContext, IUserService userService, IConsultationService consultationService, IHttpContextAccessor httpContextAccessor)
	    {
		    var user = userService.GetCurrentUser();
		    if (!user.IsAuthorised)
		    {
			    throw new AuthenticationException("GetCurrentUser returned null");
		    }
		    _niceUser = httpContextAccessor.HttpContext.User;
		    if (!_niceUser.Identity.IsAuthenticated)
		    {
			    throw new AuthenticationException("NICE user is not authenticated");
		    }
		    
			_context = consultationsContext;
		    _consultationService = consultationService;
		}

	    public (IEnumerable<Models.Comment> comment, IEnumerable<Models.Answer> answer, IEnumerable<Models.Question> question, Validate valid) GetAllDataForConsulation(int consultationId)
	    {
			var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);
		    var commentsInDB = _context.GetAllSubmittedCommentsForURI(sourceURI);
		    var answersInDB = _context.GetAllSubmittedAnswersForURI(sourceURI);
		    var questionsInDB = _context.GetUnansweredQuestionsForURI(sourceURI);

			if (commentsInDB == null && answersInDB == null && questionsInDB == null)
				return (null, null, null, new Validate(valid: false, notFound: true, message: $"Consultation id:{consultationId} not found trying to get all data for consultation"));

			return (commentsInDB, answersInDB, questionsInDB, null);
	    }

	    public (IEnumerable<Models.Comment> comment, IEnumerable<Models.Answer> answer, IEnumerable<Models.Question> question, Validate valid) GetAllDataForConsulationForCurrentUser(int consultationId)
	    {

		    var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);
		    var commentsInDB = _context.GetUsersCommentsForURI(sourceURI);
		    var answersInDB = _context.GetUsersAnswersForURI(sourceURI);
		    var questionsInDB = _context.GetUsersUnansweredQuestionsForURI(sourceURI);

		    if (commentsInDB == null && answersInDB == null && questionsInDB == null)
			    return (null, null, null, new Validate(valid: false, notFound: true, message: $"Consultation id:{consultationId} not found trying to get all data for consultation"));

		    return (commentsInDB, answersInDB, questionsInDB, null);
	    }

		public (string ConsultationName, string DocumentName, string ChapterName) GetLocationData(Location location)
	    {
		    var sourceURI = location.SourceURI;
		    ConsultationsUriElements URIElements = ConsultationsUri.ParseConsultationsUri(sourceURI);

		    var consultationDetails = _consultationService.GetConsultation(URIElements.ConsultationId, Page.QuestionAdmin, false);
		    var documents = _consultationService.GetDocuments(URIElements.ConsultationId);
		    var documentDetail = documents.FirstOrDefault(x => x.DocumentId == URIElements.DocumentId);

		    Chapter chapterDetail = null;
		    if (URIElements.ChapterSlug != null)
			    chapterDetail = documentDetail?.Chapters.First(c => c.Slug == URIElements.ChapterSlug);

		    return (consultationDetails.ConsultationName, documentDetail?.Title, chapterDetail?.Slug);
	    }
	}
}
