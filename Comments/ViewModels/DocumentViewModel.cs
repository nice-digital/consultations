using System.Collections.Generic;

namespace Comments.ViewModels
{
    public class DocumentViewModel
    {
        public DocumentViewModel(Consultation consultation, ChapterWithHTML chapterHtml, IEnumerable<Comment> comments, IEnumerable<Question> questions)
        {
            Consultation = consultation;
            ChapterHTML = chapterHtml;
            Comments = comments;
            Questions = questions;
        }

        public Consultation Consultation { get; private set; }

        public ChapterWithHTML ChapterHTML { get; private set; }

        public IEnumerable<ViewModels.Comment> Comments { get; private set; }

        public IEnumerable<ViewModels.Question> Questions { get; private set; }
    }
}