using Comments.Services;
using Comments.ViewModels;
using System;
using System.Collections.Generic;
using NICE.Feeds.Models.Indev.Detail;
using Location = Comments.Models.Location;

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

	    public bool HasSubmittedCommentsOrQuestions(string consultationSourceURI, Guid userId)
	    {
		    return false;
	    }

	    

	    public ConsultationState GetConsultationState(string sourceURI, IEnumerable<Location> locations = null,
		    ConsultationDetail consultation = null)
	    {
			return new ConsultationState(DateTime.MinValue, _consultationIsOpen ? DateTime.MaxValue : DateTime.MinValue, true, true, true, false);
		}
	    public ConsultationState GetConsultationState(int consultationId, IEnumerable<Location> locations = null,
		    ConsultationDetail consultation = null)
	    {
		    return new ConsultationState(DateTime.MinValue, _consultationIsOpen ? DateTime.MaxValue : DateTime.MinValue, true, true, true, false);
	    }

		#region Not Implemented Members
	    public Breadcrumb GetBreadcrumb(ConsultationDetail consultation, bool isReview)
	    {
		    throw new NotImplementedException();
	    }
		public (int rowsUpdated, Validate validate) SubmitCommentsAndAnswers(CommentsAndAnswers commentsAndAnswers)
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

	    public Consultation GetConsultation(int consultationId, bool isReview)
	    {
		    throw new NotImplementedException();
	    }

		public IEnumerable<Consultation> GetConsultations()
	    {
		    throw new NotImplementedException();
	    }

	    #endregion Not Implemented Members
	}
}
