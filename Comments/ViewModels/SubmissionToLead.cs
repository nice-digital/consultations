using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
    public class SubmissionToLead
    {
	    public SubmissionToLead() {} //this is here for the model binding.

	    public SubmissionToLead(IList<Comment> comments, IList<Answer> answers, string emailAddress, bool respondingAsOrganisation, string organisationName)
		{
			_comments = comments;
			_answers = answers;
			EmailAddress = emailAddress;
			RespondingAsOrganisation = respondingAsOrganisation;
			OrganisationName = organisationName;
		}

	    private IList<Comment> _comments = new List<Comment>();
	    public IList<Comment> Comments {
			get
			{
				if (_comments == null)
					_comments = new List<Comment>();

				return _comments;
			}
			set => _comments = value;
	    }

	    private IList<Answer> _answers = new List<Answer>();
		public IList<Answer> Answers {
			get
			{
				if (_answers == null)
					_answers = new List<Answer>();

				return _answers;
			}
			set => _answers = value;
		}

	    public string EmailAddress { get; set; }
	    public bool RespondingAsOrganisation { get; set; }
	    public string OrganisationName { get; set; }

		public IList<string> SourceURIs
	    {
		    get
		    {
			    var commentSourceURIs = Comments.Select(c => c.SourceURI).ToList();
			    var questionSourceURIs = Answers.Select(a => a.SourceURI).ToList();
				return commentSourceURIs.Concat(questionSourceURIs).ToList();
		    }
	    }
		public double DurationBetweenFirstCommentOrAnswerSavedAndSubmissionInSeconds { get; set; }
	}
}
