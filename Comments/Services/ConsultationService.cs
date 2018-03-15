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
        (int validDocumentId, string validChapterSlug) ValidateDocumentAndChapterWithinConsultation(ConsultationDetail consultation, int documentId, string chapterSlug);
    }

    public class ConsultationService : IConsultationService
    {

        private readonly IFeedReaderService _feedReaderService;
        private readonly IFeedConverterService _feedConverterService;
        private readonly ILogger<ConsultationService> _logger;

        public ConsultationService(IFeedReaderService feedReaderService, IFeedConverterService feedConverterService, ILogger<ConsultationService> logger)
        {
            _feedReaderService = feedReaderService;
            _feedConverterService = feedConverterService;
            _logger = logger;
        }

        public ConsultationDetail GetConsultationDetail(int consultationId)
        {
            try
            {
                return new ViewModels.ConsultationDetail(_feedConverterService.ConvertConsultationDetail(consultationId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NICE.Feeds returned error:" + ex.ToString());
                throw;
            }
        }

        public ChapterContent GetChapterContent(int consultationId, int documentId, string chapterSlug)
        {
            try
            {
                return new ViewModels.ChapterContent(
                    _feedConverterService.ConvertConsultationChapter(consultationId, documentId, chapterSlug));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NICE.Feeds returned error:" + ex.ToString());
                throw;
            }
        }


        public IEnumerable<Document> GetDocuments(int consultationId)
        {
            try
            {
                var consultationDetail = _feedConverterService.ConvertConsultationDetail(consultationId);
                return consultationDetail.Resources.Select(r => new ViewModels.Document(r)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NICE.Feeds returned error:" + ex.ToString());
                throw;
            }
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
