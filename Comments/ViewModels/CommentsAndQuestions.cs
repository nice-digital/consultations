using System.Collections.Generic;

namespace Comments.ViewModels
{
    public class CommentsAndQuestions
    {
        public CommentsAndQuestions(IEnumerable<Comment> comments, IEnumerable<Question> questions, bool isAuthorised, string signInURL)
        {
            Comments = comments;
            Questions = questions;
            IsAuthorised = isAuthorised;
            SignInURL = signInURL;
        }

        public IEnumerable<ViewModels.Comment> Comments { get; private set; }

        public IEnumerable<ViewModels.Question> Questions { get; private set; }

        public bool IsAuthorised { get; private set; }

        public string SignInURL { get; private set; }
    }
}