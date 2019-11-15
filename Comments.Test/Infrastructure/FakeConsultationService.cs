using Comments.Services;
using Comments.ViewModels;
using System;
using System.Collections.Generic;
using NICE.Feeds;
using NICE.Feeds.Models.Indev.Detail;
using NICE.Feeds.Models.Indev.List;
using Location = Comments.Models.Location;

namespace Comments.Test.Infrastructure
{
	public class FakeConsultationService : IConsultationService
    {
	    private readonly bool _consultationIsOpen;
	    private readonly int _documentCount;

	    public FakeConsultationService(bool consultationIsOpen = true, int documentCount = 1)
	    {
		    _consultationIsOpen = consultationIsOpen;
		    _documentCount = documentCount;
	    }

	    public bool ConsultationIsOpen(string sourceURI)
	    {
		    return _consultationIsOpen;
	    }

	    public DateTime? GetSubmittedDate(string consultationSourceURI)
	    {
		    return null;
	    }


	    public ChapterContent GetPreviewChapterContent(int consultationId, int documentId, string chapterSlug, string reference)
	    {
		    throw new NotImplementedException();
	    }

	    public ConsultationState GetConsultationState(string sourceURI, PreviewState previewState, IEnumerable<Location> locations = null,
		    ConsultationBase consultation = null)
	    {
			return new ConsultationState(DateTime.MinValue, _consultationIsOpen ? DateTime.MaxValue : DateTime.MinValue, true, true, true, null, null);
		}
	    public ConsultationState GetConsultationState(int consultationId, int? documentId, string reference, PreviewState previewState, IEnumerable<Location> locations = null,
		    ConsultationBase consultation = null)
	    {
		    return new ConsultationState(DateTime.MinValue, _consultationIsOpen ? DateTime.MaxValue : DateTime.MinValue, true, true, true, null, null);
	    }

	    public IEnumerable<Consultation> GetConsultations()
	    {
		    return new List<Consultation>(){ new Consultation("GID-WAVE", "title", "some name", DateTime.MinValue, DateTime.MaxValue, "consultation type", "resource title id", "project type",
				"product type name", true, "developed as", "relevant to", 1, "process", true, true, true, true, "partially updated reference", "original reference", new User(true, "Benjamin Button", Guid.Empty.ToString(), "org name"))};
		}

	    public (int? documentId, string chapterSlug) GetFirstConvertedDocumentAndChapterSlug(int consultationId)
	    {
		    return (documentId: 1, chapterSlug: "my-chapter-slug");
	    }

		#region Not Implemented Members
		public IEnumerable<BreadcrumbLink> GetBreadcrumbs(ConsultationDetail consultation, bool isReview)
	    {
		    throw new NotImplementedException();
	    }

	    public string GetFirstChapterSlug(int consultationId, int documentId)
	    {
		    throw new NotImplementedException();
	    }

	    public string GetFirstChapterSlugFromPreviewDocument(string reference, int consultationId, int documentId)
	    {
		    throw new NotImplementedException();
	    }

	    public (int rowsUpdated, Validate validate) SubmitCommentsAndAnswers(ViewModels.Submission submission)
		{
			throw new NotImplementedException();
		}

		public ChapterContent GetChapterContent(int consultationId, int documentId, string chapterSlug)
	    {
		    throw new NotImplementedException();
	    }

	    public (IEnumerable<Document> documents, string consultationTitle) GetDocuments(int consultationId, string reference = null, bool draft = false)
	    {
		    return (new List<Document>()
		    {
			    new Document(1, 1, true, "doc 1", new List<Chapter>()
			    {
				    new Chapter("chapter-slug", "title"),
				    new Chapter("chapter-slug2", "title2")
			    }, true)
		    }, "Consultation Title");
	    }

	    public IEnumerable<Document> GetPreviewDraftDocuments(int consultationId, int documentId, string reference)
	    {
		    throw new NotImplementedException();
	    }

	    public IEnumerable<Document> GetPreviewPublishedDocuments(int consultationId, int documentId)
	    {
		    throw new NotImplementedException();
	    }

	    public Consultation GetConsultation(int consultationId, BreadcrumbType breadcrumbType, bool useFilter)
	    {
			var userService = FakeUserService.Get(true, "Benjamin Button", Guid.NewGuid().ToString());
			var consultationBase = new ConsultationBase()
		    {
			    ConsultationId = 1,
			    ConsultationName = "ConsultationName",
				Title = "Consultation Title"
		    };

		    return new Consultation(consultationBase, userService.GetCurrentUser());
	    }

	    public Consultation GetDraftConsultation(int consultationId, int documentId, string reference, bool isReview)
	    {
		    throw new NotImplementedException();
	    }

	    public IEnumerable<BreadcrumbLink> GetBreadcrumbs(ConsultationDetail consultation, BreadcrumbType breadcrumbType)
	    {
		    throw new NotImplementedException();
	    }

		#endregion Not Implemented Members
	}
}
