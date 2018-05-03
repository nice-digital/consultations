using System.Collections.Generic;

namespace Comments.ViewModels
{
    public class CommentsAndQuestions
    {
        public CommentsAndQuestions(IEnumerable<Comment> comments, IEnumerable<Question> questions, bool isAuthenticated)
        {
            Comments = comments;
            Questions = questions;
            IsAuthenticated = isAuthenticated;
        }

        public IEnumerable<ViewModels.Comment> Comments { get; private set; }

        public IEnumerable<ViewModels.Question> Questions { get; private set; }

        public bool IsAuthenticated { get; private set; }
    }
}