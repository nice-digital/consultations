using System;
using System.Collections.Generic;

namespace Comments.ViewModels
{
    public class DocumentViewModel
    {
        public DocumentViewModel(ConsultationDetail consultation, int selectedConsultationDocumentId, ChapterWithHTML chapterHtml, IEnumerable<Comment> comments, IEnumerable<Question> questions)
        {
            Consultation = consultation;
            SelectedConsultationDocumentId = selectedConsultationDocumentId;
            ChapterHTML = chapterHtml;
            Comments = comments;
            Questions = questions;
        }

        public ConsultationDetail Consultation { get; private set; }

        public int SelectedConsultationDocumentId { get; private set; }

        public ChapterWithHTML ChapterHTML { get; private set; }

        public IEnumerable<ViewModels.Comment> Comments { get; private set; }

        public IEnumerable<ViewModels.Question> Questions { get; private set; }
    }
}