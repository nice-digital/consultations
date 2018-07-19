using System;
using Comments.ViewModels;
using Microsoft.Extensions.Logging;
using NICE.Feeds;
using System.Collections.Generic;
using System.Linq;
using Comments.Common;
using Comments.Models;
using NICE.Feeds.Models.Indev.Detail;

namespace Comments.Services
{
	public interface IConsultationService
    {
        ChapterContent GetChapterContent(int consultationId, int documentId, string chapterSlug);
        IEnumerable<Document> GetDocuments(int consultationId);
        ViewModels.Consultation GetConsultation(int consultationId, bool isReview);
        IEnumerable<ViewModels.Consultation> GetConsultations();
	    ConsultationState GetConsultationState(string sourceURI, IEnumerable<Models.Location> locations = null, ConsultationDetail consultation = null);
	    ConsultationState GetConsultationState(int consultationId, IEnumerable<Models.Location> locations = null, ConsultationDetail consultation = null);

		bool HasSubmittedCommentsOrQuestions(string consultationSourceURI, Guid userId);
	    Breadcrumb GetBreadcrumb(int consultationId, bool isReview);
    }

	public class ConsultationService : IConsultationService
    {
	    private readonly ConsultationsContext _context;
	    private readonly IFeedService _feedConverterService;
        private readonly ILogger<ConsultationService> _logger;
        private readonly IUserService _userService;

		public ConsultationService(ConsultationsContext context, IFeedService feedConverterService, ILogger<ConsultationService> logger, IUserService userService)
        {
	        _context = context;
	        _feedConverterService = feedConverterService;
            _logger = logger;
            _userService = userService;
		}

        public ChapterContent GetChapterContent(int consultationId, int documentId, string chapterSlug)
        {
            return new ViewModels.ChapterContent(
                _feedConverterService.GetConsultationChapterForPublishedProject(consultationId, documentId, chapterSlug));
        }

        public IEnumerable<Document> GetDocuments(int consultationId)
        {
            var consultationDetail = _feedConverterService.GetIndevConsultationDetailForPublishedProject(consultationId, PreviewState.NonPreview);
            return consultationDetail.Resources.Select(r => new ViewModels.Document(consultationId, r)).ToList();
        }

        public ViewModels.Consultation GetConsultation(int consultationId, bool isReview)
        {
            var user = _userService.GetCurrentUser();
	        var consultationDetail = GetConsultationDetail(consultationId);
	        var consultationState = GetConsultationState(consultationId, null, consultationDetail);
	        var breadcrumb = GetBreadcrumb(consultationId, isReview);
            return new ViewModels.Consultation(consultationDetail, user, breadcrumb, consultationState);
        }

	    public Breadcrumb GetBreadcrumb(int consultationId, bool isReview)
	    {
		    return null;
	    }

	    public IEnumerable<ViewModels.Consultation> GetConsultations()
        {
            var user = _userService.GetCurrentUser();
            var consultations = _feedConverterService.GetConsultationList();
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
			    data.questions.Any(), data.questions.Any(q => q.Answers.Any()), data.comments.Any(), hasSubmitted);

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
		    var consultationDetail = _feedConverterService.GetIndevConsultationDetailForPublishedProject(consultationId, PreviewState.NonPreview);
		    return consultationDetail;
	    }
	}
}
