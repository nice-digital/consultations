using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
    public class CommentsAndAnswers
    {
	    public CommentsAndAnswers() {} //this is here for the model binding.

	    public CommentsAndAnswers(IList<Comment> comments, IList<Answer> answers)
		{
			_comments = comments;
			_answers = answers;
		}

	    private IList<Comment> _comments = new List<Comment>();
	    public IList<ViewModels.Comment> Comments {
			get
			{
				if (_comments == null)
					_comments = new List<Comment>();

				return _comments;
			}
			set => _comments = value;
	    }

	    private IList<Answer> _answers = new List<Answer>();
		public IList<ViewModels.Answer> Answers {
			get
			{
				if (_answers == null)
					_answers = new List<Answer>();

				return _answers;
			}
			set => _answers = value;
		}

	    public IList<string> SourceURIs
	    {
		    get
		    {
			    return Comments.Select(c => c.SourceURI).ToList();
			    //var questionSourceURIs = Answers.Select(a => a. //TODO: question source uri
			    //return commentSourceURIs.Concat(questionSourceURIs).ToList();
		    }
	    }
    }
}
