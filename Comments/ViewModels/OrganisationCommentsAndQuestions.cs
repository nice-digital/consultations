using System.Collections.Generic;

namespace Comments.ViewModels
{
	public class OrganisationCommentsAndQuestions
    {
	    public OrganisationCommentsAndQuestions(IList<Question> questions, IList<Comment> comments)
	    {
		    Comments = comments;
		    Questions = questions;
	    }

	    public IList<ViewModels.Comment> Comments { get; set; }

        public IList<ViewModels.Question> Questions { get; set; }

	}
}
