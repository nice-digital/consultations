using System.Collections.Generic;

namespace Comments.ViewModels
{
	public class CommentsAndQuestions
    {
	    public CommentsAndQuestions(IList<Comment> comments, IList<Question> questions, bool isAuthorised, string signInURL, ConsultationState consultationState)
	    {
		    Comments = comments;
		    Questions = questions;
		    IsAuthorised = isAuthorised;
		    SignInURL = signInURL;
		    ConsultationState = consultationState;
	    }

	    public IList<ViewModels.Comment> Comments { get; set; }

        public IList<ViewModels.Question> Questions { get; set; }

        public bool IsAuthorised { get; private set; }

        public string SignInURL { get; private set; }

		public ConsultationState ConsultationState { get; private set; }
	}
}
