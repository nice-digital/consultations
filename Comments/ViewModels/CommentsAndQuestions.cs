using System.Collections.Generic;

namespace Comments.ViewModels
{
	public class CommentsAndQuestions
    {
	    public CommentsAndQuestions(IList<Comment> comments, IList<Question> questions, bool isAuthorised, string signInURL, ConsultationState consultationState, IEnumerable<TopicListFilterGroup> filters)
	    {
		    Comments = comments;
		    Questions = questions;
		    IsAuthorised = isAuthorised;
		    SignInURL = signInURL;
		    ConsultationState = consultationState;
		    Filters = filters;
	    }

	    public IList<ViewModels.Comment> Comments { get; private set; }

        public IList<ViewModels.Question> Questions { get; private set; }

        public bool IsAuthorised { get; private set; }

        public string SignInURL { get; private set; }

		public ConsultationState ConsultationState { get; private set; }

	    public IEnumerable<TopicListFilterGroup> Filters { get; private set; }
	}
}
