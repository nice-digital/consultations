using Comments.Services;
using Comments.ViewModels;
using System;
using System.Collections.Generic;

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

	    public ConsultationDetail GetConsultationDetail(int consultationId)
	    {
		    return new ConsultationDetail("CG80", "some consultation", "consultation name", DateTime.MinValue, DateTime.MaxValue,
			    "type", "resource title", "CG", "Clinical guidelines", "CG", null, 1, null, true, true, true, true, null, null, null, null);
	    }

	    public bool HasSubmittedCommentsOrQuestions(string consultationSourceURI, Guid userId)
	    {
		    return true;
	    }


		#region Not Implemented Members


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

	    #endregion Not Implemented Members
	}
}
