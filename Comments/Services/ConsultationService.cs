using System;
using Comments.ViewModels;
using Microsoft.Extensions.Logging;
using NICE.Feeds;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Comments.Common;
using Comments.Configuration;
using Comments.Models;
using NICE.Feeds.Models.Indev.Chapter;
using NICE.Feeds.Models.Indev.Detail;
using NICE.Feeds.Models.Indev.List;

namespace Comments.Services
{
	public interface IConsultationService
    {
        ChapterContent GetChapterContent(int consultationId, int documentId, string chapterSlug);
        IEnumerable<Document> GetDocuments(int consultationId);
	    IEnumerable<Document> GetPreviewDraftDocuments(int consultationId, int documentId, string reference);
	    IEnumerable<Document> GetPreviewPublishedDocuments(int consultationId, int documentId);
        ViewModels.Consultation GetConsultation(int consultationId, bool isReview);
        IEnumerable<ViewModels.Consultation> GetConsultations();

	    ChapterContent GetPreviewChapterContent(int consultationId, int documentId, string chapterSlug, string reference);
	    ConsultationState GetConsultationState(string sourceURI, IEnumerable<Models.Location> locations = null, ConsultationDetail consultation = null);
	    ConsultationState GetConsultationState(int consultationId, IEnumerable<Models.Location> locations = null, ConsultationDetail consultation = null);


		bool HasSubmittedCommentsOrQuestions(string consultationSourceURI, Guid userId);
	    IEnumerable<BreadcrumbLink> GetBreadcrumbs(ConsultationDetail consultation, bool isReview);
    }

	public class ConsultationService : IConsultationService
    {
	    private readonly ConsultationsContext _context;
	    private readonly IFeedService _feedService;
        private readonly ILogger<ConsultationService> _logger;
        private readonly IUserService _userService;

		public ConsultationService(ConsultationsContext context, IFeedService feedService, ILogger<ConsultationService> logger, IUserService userService)
        {
	        _context = context;
	        _feedService = feedService;
            _logger = logger;
            _userService = userService;
		}

        public ChapterContent GetChapterContent(int consultationId, int documentId, string chapterSlug)
        {
            return new ViewModels.ChapterContent(
                _feedService.GetConsultationChapterForPublishedProject(consultationId, documentId, chapterSlug));
        }

	    public ChapterContent GetPreviewChapterContent(int consultationId, int documentId, string chapterSlug, string reference)
	    {
		    return new ViewModels.ChapterContent(
			    _feedService.GetIndevConsultationChapterForDraftProject(consultationId, documentId, chapterSlug, reference));
	    }

		public IEnumerable<Document> GetDocuments(int consultationId)
        {
            var consultationDetail = _feedService.GetIndevConsultationDetailForPublishedProject(consultationId, PreviewState.NonPreview);
            return consultationDetail.Resources.Select(r => new ViewModels.Document(consultationId, r)).ToList();
        }


	    public IEnumerable<Document> GetPreviewPublishedDocuments(int consultationId, int documentId)
	    {
		    var consultationDetail = _feedService.GetIndevConsultationDetailForPublishedProject(consultationId, PreviewState.Preview, documentId);
		    return consultationDetail.Resources.Select(r => new ViewModels.Document(consultationId, r)).ToList();
	    }

		public IEnumerable<Document> GetPreviewDraftDocuments(int consultationId, int documentId, string reference)
	    {
		    var consultationDetail = _feedService.GetIndevConsultationDetailForDraftProject(consultationId, documentId, reference);
		    return consultationDetail.Resources.Select(r => new ViewModels.Document(consultationId, r)).ToList();
	    }


        public ViewModels.Consultation GetConsultation(int consultationId, bool isReview)
        {
            var user = _userService.GetCurrentUser();
	        var consultationDetail = GetConsultationDetail(consultationId);
	        var consultationState = GetConsultationState(consultationId, null, consultationDetail);
	        var breadcrumbs = GetBreadcrumbs(consultationDetail, isReview);
	        var filters = isReview ? AppSettings.ReviewConfig.Filters : null;
            return new ViewModels.Consultation(consultationDetail, user, breadcrumbs, consultationState, filters);
        }




	    public IEnumerable<BreadcrumbLink> GetBreadcrumbs(ConsultationDetail consultation, bool isReview)
	    {
			var breadcrumbs = new List<BreadcrumbLink>{
					new BreadcrumbLink("Home", ExternalRoutes.HomePage),
					new BreadcrumbLink(consultation.Title, ExternalRoutes.ConsultationUrl(consultation))
			};

		    if (isReview)
		    {
			    var firstDocument = GetDocuments(consultation.ConsultationId).FirstOrDefault(d => d.ConvertedDocument);
			    var firstChapter = firstDocument?.Chapters.FirstOrDefault();

			    if (firstChapter != null)
				    breadcrumbs.Add(new BreadcrumbLink("Consultation documents", $"/consultations/{consultation.ConsultationId}/{firstDocument.DocumentId}/{firstChapter.Slug}"));
		    }

		    return breadcrumbs;
	    }

		public IEnumerable<ViewModels.Consultation> GetConsultations()
        {
            var user = _userService.GetCurrentUser();
            var consultations = _feedService.GetConsultationList();
            return consultations.Select(c => new ViewModels.Consultation(c, user)).ToList();
        }

	    public ConsultationState GetConsultationState(string sourceURI, IEnumerable<Models.Location> locations = null, ConsultationDetail consultation = null)
	    {
		    var consultationsUriElements = ConsultationsUri.ParseConsultationsUri(sourceURI);
		    return GetConsultationState(consultationsUriElements.ConsultationId, locations, consultation);
	    }

		public ConsultationState GetConsultationState(int consultationId, IEnumerable<Models.Location> locations = null, ConsultationDetail consultationDetail = null)
	    {
			var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);
			if (consultationDetail == null)
				consultationDetail = GetConsultationDetail(consultationId);

		    var documents = GetDocuments(consultationId).ToList();
		    var documentsWhichSupportQuestions = documents.Where(d => d.SupportsQuestions).Select(d => d.DocumentId).ToList();
		    var documentsWhichSupportComments = documents.Where(d => d.SupportsComments).Select(d => d.DocumentId).ToList();

			var currentUser = _userService.GetCurrentUser();

			if (locations == null && currentUser.IsAuthorised && currentUser.UserId.HasValue)
		    {
			    locations = _context.GetAllCommentsAndQuestionsForDocument(new[] { sourceURI }, partialMatchSourceURI: true);
		    }
			else
			{
				locations = new List<Models.Location>(0);
			}

		    var hasSubmitted = currentUser != null && currentUser.IsAuthorised && currentUser.UserId.HasValue ? HasSubmittedCommentsOrQuestions(sourceURI, currentUser.UserId.Value) : false;

		    var data = ModelConverters.ConvertLocationsToCommentsAndQuestionsViewModels(locations);

		    var consultationState = new ConsultationState(consultationDetail.StartDate, consultationDetail.EndDate,
			    data.questions.Any(), data.questions.Any(q => q.Answers.Any()), data.comments.Any(), hasSubmitted,
			    consultationDetail.SupportsQuestions, consultationDetail.SupportsComments, documentsWhichSupportQuestions, documentsWhichSupportComments);

		    return consultationState;
	    }

	    public bool HasSubmittedCommentsOrQuestions(string anySourceURI, Guid userId)
	    {
		    if (string.IsNullOrWhiteSpace(anySourceURI))
			    return false;

		    var consultationsUriElements = ConsultationsUri.ParseConsultationsUri(anySourceURI);

		    var consultationSourceURI = ConsultationsUri.CreateConsultationURI(consultationsUriElements.ConsultationId);

		    return _context.HasSubmitted(consultationSourceURI, userId);
	    }

	    /// <summary>
	    /// This is intentionally private as it gets the ConsultationDetail straight from the feed. not for external consumption outside of this class.
	    /// </summary>
	    /// <param name="consultationId"></param>
	    /// <returns></returns>
	    private ConsultationDetail GetConsultationDetail(int consultationId)
	    {
		    var consultationDetail = _feedService.GetIndevConsultationDetailForPublishedProject(consultationId, PreviewState.NonPreview);
		    return consultationDetail;
	    }
	}
}
