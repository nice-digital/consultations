using Comments.Models;
using Comments.Services;
using Comments.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Comment = Comments.ViewModels.Comment;
using Location = Comments.Models.Location;
using Question = Comments.ViewModels.Question;

namespace Comments.Test.Infrastructure
{
	public class FakeConsultationService : IConsultationService
    {
	    private readonly bool _consultationIsOpen;
	    public FakeConsultationService(bool consultationIsOpen = true)
	    {
		    _consultationIsOpen = consultationIsOpen;
	    }

	    public bool ConsultationIsOpen(string sourceURI)
	    {
		    return _consultationIsOpen;
	    }

		#region Not Implemented Members
		public ConsultationDetail GetConsultationDetail(int consultationId)
	    {
		    throw new NotImplementedException();
	    }

	    public ChapterContent GetChapterContent(int consultationId, int documentId, string chapterSlug)
	    {
		    throw new NotImplementedException();
	    }

	    public IEnumerable<Document> GetDocuments(int consultationId)
	    {
		    throw new NotImplementedException();
	    }

	    public Consultation GetConsultation(int consultationId)
	    {
		    throw new NotImplementedException();
	    }

	    public IEnumerable<Consultation> GetConsultations()
	    {
		    throw new NotImplementedException();
	    }

		public ConsultationState GetConsultationState(string sourceURI, IEnumerable<Models.Location> locations = null)
		{
			return new ConsultationState(DateTime.MinValue, _consultationIsOpen ? DateTime.MaxValue : DateTime.MinValue, true, true, true, false);
		}

	    public (int rowsUpdated, Validate validate) SubmitCommentsAndAnswers(CommentsAndAnswers commentsAndAnswers)
	    {
		    throw new NotImplementedException();
	    }

	    public bool HasSubmittedCommentsOrQuestions(string consultationSourceURI, Guid userId)
	    {
		    throw new NotImplementedException();
	    }

		/// <summary>
		/// maybe this code should be a static somewhere..?
		/// </summary>
		/// <param name="locations"></param>
		/// <returns></returns>
	    public (IList<Comment> comments, IList<Question> questions) ConvertLocationsToCommentsAndQuestionsViewModels(IEnumerable<Location> locations)
	    {
			var commentsData = new List<ViewModels.Comment>();
		    var questionsData = new List<ViewModels.Question>();
		    foreach (var location in locations)
		    {
			    commentsData.AddRange(location.Comment.Select(comment => new ViewModels.Comment(location, comment)));
			    questionsData.AddRange(location.Question.Select(question => new ViewModels.Question(location, question)));
		    }
		    return (commentsData, questionsData);
		}

	    #endregion Not Implemented Members
	}
}
