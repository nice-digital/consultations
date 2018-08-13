using System.Collections.Generic;

namespace Comments.ViewModels
{
	public class CommentsAndQuestions
    {
	    public CommentsAndQuestions(List<Comment> comments, List<Question> questions, bool isAuthorised, string signInURL, ConsultationState consultationState)
	    {
		    Comments = comments;
		    Questions = questions;
		    IsAuthorised = isAuthorised;
		    SignInURL = signInURL;
		    ConsultationState = consultationState;
	    }

	    public List<ViewModels.Comment> Comments { get; set; }

        public List<ViewModels.Question> Questions { get; set; }

        public bool IsAuthorised { get; private set; }

        public string SignInURL { get; private set; }

		public ConsultationState ConsultationState { get; private set; }
	}
}
