using System.Collections.Generic;

namespace Comments.ViewModels
{
    public class CommentsAndAnswers
    {
		public CommentsAndAnswers(IEnumerable<Comment> comments, IEnumerable<Answer> answers)
		{
			Comments = comments;
			Answers = answers;
		}

		public IEnumerable<ViewModels.Comment> Comments { get; private set; }

        public IEnumerable<ViewModels.Answer> Answers { get; private set; }
    }
}
