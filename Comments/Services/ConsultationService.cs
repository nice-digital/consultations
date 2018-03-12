using Comments.ViewModels;
using NICE.Feeds;
using System;
using System.Linq;

namespace Comments.Services
{
    public interface IConsultationService
    {
        ConsultationDetail GetConsultationDetail(int consultationId);
        ChapterContent GetChapterContent(int consultationId, int documentId, string chapterSlug);
        (int validDocumentId, string validChapterSlug) ValidateDocumentAndChapterWithinConsultation(ConsultationDetail consultation, int documentId, string chapterSlug);
    }

    public class ConsultationService : IConsultationService
    {

        private readonly IFeedReaderService _feedReaderService;

        public ConsultationService(IFeedReaderService feedReaderService)
        {
            _feedReaderService = feedReaderService;
        }

        public ConsultationDetail GetConsultationDetail(int consultationId)
        {
            var feedService = new FeedConverterConverterService(_feedReaderService); 
            return new ViewModels.ConsultationDetail(feedService.ConvertConsultationDetail(consultationId));
        }

        public ChapterContent GetChapterContent(int consultationId, int documentId, string chapterSlug)
        {
            var feedService = new FeedConverterConverterService(_feedReaderService); 
            return new ViewModels.ChapterContent(feedService.ConvertConsultationChapter(consultationId, documentId, chapterSlug));
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
