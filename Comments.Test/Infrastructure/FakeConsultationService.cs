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
	    #endregion Not Implemented Members
	}
}
