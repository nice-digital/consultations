using Comments.ViewModels;
using NICE.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
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
        (int validDocumentId, string validChapterSlug) ValidateDocumentAndChapterWithinConsultation(ConsultationDetail consultation, int documentId, string chapterSlug);
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
            return consultationDetail.Resources.Select(r => new ViewModels.Document(r)).ToList();
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

        /// <summary>
        /// This method is called to ensure the documentId and chapter slug have been set and that they belong together.
        /// i.e. the document belongs to the consultation, and the chapter is in the document.
        /// </summary>
        /// <param name="consultation"></param>
        /// <param name="documentId"></param>
        /// <param name="chapterSlug"></param>
        /// <returns></returns>
        public (int validDocumentId, string validChapterSlug) ValidateDocumentAndChapterWithinConsultation(ConsultationDetail consultation, int documentId, string chapterSlug)
        {
            if (consultation.Documents == null || !consultation.Documents.Any())
            {
                throw new Exception("No documents found on consultation:" + consultation.ConsultationId);
            }

            var document = consultation.Documents.FirstOrDefault(d => d.DocumentId.Equals(documentId));
            if (document == null)
                document = consultation.Documents.FirstOrDefault(d => d.SupportsComments);
            if (document == null)
                document = consultation.Documents.First();

            if (!document.SupportsComments)
                return (document.DocumentId, null);

            if (document.Chapters == null || !document.Chapters.Any())
            {
                throw new Exception("No chapters found within document:" + document.DocumentId);
            }

            var chapter = document.Chapters.FirstOrDefault(c => c.Slug.Equals(chapterSlug));
            if (chapter == null)
                chapter = document.Chapters.First();

            return (document.DocumentId, chapter.Slug);
        }
    }
}
