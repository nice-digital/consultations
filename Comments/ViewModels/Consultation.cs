using System.Collections.Generic;

namespace Comments.ViewModels
{
    public class Consultation
    {
        public Consultation(int consultationId, string title, IEnumerable<Document> documents)
        {
            ConsultationId = consultationId;
            Title = title;
            Documents = documents;
        }

        public int ConsultationId { get; private set; }
        public string Title { get; private set; }
        public IEnumerable<Document> Documents { get; private set; }
    }

    public class Document
    {
        public Document(int documentId, bool supportsComments, bool supportsQuestions, bool isSupportingDocument, IEnumerable<Chapter> chapters)
        {
            DocumentId = documentId;
            SupportsComments = supportsComments;
            SupportsQuestions = supportsQuestions;
            IsSupportingDocument = isSupportingDocument;
            Chapters = chapters;
        }

        public int DocumentId { get; private set; }
        public bool SupportsComments { get; private set; }
        public bool SupportsQuestions { get; private set; }
        public bool IsSupportingDocument { get; private set; }
        public IEnumerable<Chapter> Chapters { get; private set; }
    }

    public class Chapter
    {
        public Chapter(string chapterSlug, string chapterHtml)
        {
            ChapterSlug = chapterSlug;
            ChapterHTML = chapterHtml;
        }
        public string ChapterSlug { get; private set; }
        public string ChapterHTML { get; private set; }
    }
}
