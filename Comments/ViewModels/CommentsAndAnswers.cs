using System.Collections.Generic;

namespace Comments.ViewModels
{
    public class CommentsAndAnswers
    {
	    public CommentsAndAnswers() {} //this is here for the model binding.

	    public CommentsAndAnswers(IEnumerable<Comment> comments, IEnumerable<Answer> answers)
		{
			_comments = comments;
			_answers = answers;
		}

	    private IEnumerable<Comment> _comments = new List<Comment>();
	    public IEnumerable<ViewModels.Comment> Comments {
			get
			{
				if (_comments == null)
					_comments = new List<Comment>();

				return _comments;
			}
			set { _comments = value; }
		}

	    private IEnumerable<Answer> _answers = new List<Answer>();
		public IEnumerable<ViewModels.Answer> Answers {
			get
			{
				if (_answers == null)
					_answers = new List<Answer>();

				return _answers;
			}
			set { _answers = value; }
		}
    }
}
