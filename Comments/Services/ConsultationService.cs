using System;
using Comments.ViewModels;
using Microsoft.Extensions.Logging;
using NICE.Feeds;
using System.Collections.Generic;
using System.Linq;
using Comments.Common;
using Comments.Models;
using Location = Comments.Models.Location;

namespace Comments.Services
{
	public interface IConsultationService
    {
        ConsultationDetail GetConsultationDetail(int consultationId);
        ChapterContent GetChapterContent(int consultationId, int documentId, string chapterSlug);
        IEnumerable<Document> GetDocuments(int consultationId);
        ViewModels.Consultation GetConsultation(int consultationId);
        IEnumerable<ViewModels.Consultation> GetConsultations();
	    ConsultationState GetConsultationState(string sourceURI, IEnumerable<Models.Location> locations = null);
	    ConsultationState GetConsultationState(int consultationId, IEnumerable<Models.Location> locations = null);

		bool HasSubmittedCommentsOrQuestions(string consultationSourceURI, Guid userId);
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

        public ConsultationDetail GetConsultationDetail(int consultationId)
        {
            var user = _userService.GetCurrentUser();
            return new ViewModels.ConsultationDetail(_feedConverterService.GetIndevConsultationDetailForPublishedProject(consultationId, PreviewState.NonPreview), user); 
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

        public ViewModels.Consultation GetConsultation(int consultationId)
        {
            var user = _userService.GetCurrentUser();
            var consultation = _feedConverterService.GetIndevConsultationDetailForPublishedProject(consultationId, PreviewState.NonPreview);
	        var consultationState = GetConsultationState(consultationId);
            return new ViewModels.Consultation(consultation, user, consultationState);
        }

        public IEnumerable<ViewModels.Consultation> GetConsultations()
        {
            var user = _userService.GetCurrentUser();
            var consultations = _feedConverterService.GetConsultationList();
            return consultations.Select(c => new ViewModels.Consultation(c, user)).ToList();
        }

	    public ConsultationState GetConsultationState(string sourceURI, IEnumerable<Models.Location> locations = null)
	    {
		    var consultationsUriElements = ConsultationsUri.ParseConsultationsUri(sourceURI);
		    return GetConsultationState(consultationsUriElements.ConsultationId, locations);
	    }

		public ConsultationState GetConsultationState(int consultationId, IEnumerable<Models.Location> locations = null)
	    {
			var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);
			var consultationDetail = GetConsultationDetail(consultationId);
		    var currentUser = _userService.GetCurrentUser();

			if (locations == null && currentUser.IsAuthorised && currentUser.UserId.HasValue)
		    {
			    locations = _context.GetAllCommentsAndQuestionsForDocument(new[] { sourceURI }, partialMatchSourceURI: true);
		    }
			else
			{
				locations = new List<Location>(0);
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
	}
}
