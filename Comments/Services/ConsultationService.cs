using Comments.ViewModels;
using NICE.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Common;
using Microsoft.Extensions.Logging;

namespace Comments.Services
{
    public interface IConsultationService
    {
        ConsultationDetail GetConsultationDetail(int consultationId);
        ChapterContent GetChapterContent(int consultationId, int documentId, string chapterSlug);
        IEnumerable<Document> GetDocuments(int consultationId);
        ViewModels.Consultation GetConsultation(int consultationId);
        IEnumerable<ViewModels.Consultation> GetConsultations();
	    bool ConsultationIsOpen(string sourceURI);
    }

    public class ConsultationService : IConsultationService
    {

        private readonly IFeedService _feedConverterService;
        private readonly ILogger<ConsultationService> _logger;
        private readonly IUserService _userService;

        public ConsultationService(IFeedService feedConverterService, ILogger<ConsultationService> logger, IUserService userService)
        {
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
            return new ViewModels.Consultation(consultation, user);
        }

        public IEnumerable<ViewModels.Consultation> GetConsultations()
        {
            var user = _userService.GetCurrentUser();
            var consultations = _feedConverterService.GetConsultationList();
            return consultations.Select(c => new ViewModels.Consultation(c, user)).ToList();
        }

		public bool ConsultationIsOpen(string sourceURI)
		{
			var consultationsUriElements = ConsultationsUri.ParseConsultationsUri(sourceURI);
			var consultationDetail = GetConsultationDetail(consultationsUriElements.ConsultationId);

			var now = DateTime.Now; //note, not a UTC date as indev uses local time.

			return (now >= consultationDetail.StartDate && now <= consultationDetail.EndDate);
		}
	}
}
