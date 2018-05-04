namespace Comments.Models
{   
    public class ConsultationsUriElements
    {
        public ConsultationsUriElements(int consultationId, int? documentId, string chapterSlug)
        {
            ConsultationId = consultationId;
            DocumentId = documentId;
            ChapterSlug = chapterSlug;
        }
        public int ConsultationId { get; set; }
        public int? DocumentId { get; set; }
        public string ChapterSlug { get; set; }

        public bool IsConsultationLevel()
        {
            return !DocumentId.HasValue;
        }
        public bool IsDocumentLevel()
        {
            return DocumentId.HasValue && string.IsNullOrEmpty(ChapterSlug);
        }
        public bool IsChapterLevel()
        {
            return !string.IsNullOrEmpty(ChapterSlug);
        }
    }    
}