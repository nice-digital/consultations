using System.Collections.Generic;

namespace Comments.ViewModels
{
    public class CommentsAndQuestions
    {
        public CommentsAndQuestions(IList<Comment> comments, IList<Question> questions, bool isAuthorised, string signInURL)
        {
            Comments = comments;
            Questions = questions;
            IsAuthorised = isAuthorised;
            SignInURL = signInURL;
        }

        public IList<ViewModels.Comment> Comments { get; private set; }

        public IList<ViewModels.Question> Questions { get; private set; }

        public bool IsAuthorised { get; private set; }

        public string SignInURL { get; private set; }
    }
}
