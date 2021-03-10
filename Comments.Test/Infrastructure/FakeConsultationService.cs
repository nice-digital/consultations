using Comments.Services;
using Comments.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NICE.Feeds;
using NICE.Feeds.Indev.Models;
using NICE.Feeds.Indev.Models.Detail;
using NICE.Feeds.Indev.Models.List;
using Location = Comments.Models.Location;
using NICE.Identity.Authentication.Sdk.Domain;

namespace Comments.Test.Infrastructure
{
	public class FakeConsultationService : IConsultationService
    {
	    private readonly bool _consultationIsOpen;
	    private readonly int _documentCount;
	    private readonly DateTime? _consultationEndDate;

	    public FakeConsultationService(bool consultationIsOpen = true, int documentCount = 1, DateTime? consultationEndDate = null)
	    {
		    _consultationIsOpen = consultationIsOpen;
		    _documentCount = documentCount;
		    _consultationEndDate = consultationEndDate ?? DateTime.MaxValue;
	    }

	    public bool ConsultationIsOpen(string sourceURI)
	    {
		    return _consultationIsOpen;
	    }

	    public DateTime? GetSubmittedDate(string consultationSourceURI)
	    {
		    return null;
	    }


	    public Task<ChapterContent> GetPreviewChapterContent(int consultationId, int documentId, string chapterSlug, string reference)
	    {
		    throw new NotImplementedException();
	    }

	    public async Task<ConsultationState> GetConsultationState(string sourceURI, PreviewState previewState, IEnumerable<Location> locations = null,
		    ConsultationBase consultation = null)
	    {
			return new ConsultationState(DateTime.MinValue, _consultationIsOpen ? _consultationEndDate.Value : DateTime.MinValue, true, true, true, null, null);
		}
	    public async Task<ConsultationState> GetConsultationState(int consultationId, int? documentId, string reference, PreviewState previewState, IEnumerable<Location> locations = null,
		    ConsultationBase consultation = null)
	    {
		    return new ConsultationState(DateTime.MinValue, _consultationIsOpen ? DateTime.MaxValue : DateTime.MinValue, true, true, true, null, null);
	    }

	    public async Task<IEnumerable<Consultation>> GetConsultations()
	    {
		    return new List<Consultation>(){ new Consultation("GID-WAVE", "title", "some name", DateTime.MinValue, DateTime.MaxValue, "consultation type", "resource title id", "project type",
				"product type name", true, "developed as", "relevant to", 1, "process", true, true, true, true, "partially updated reference", "original reference",
				new User(User.AuthenticationMechanism.Accounts, AuthenticationConstants.AuthenticationScheme, "Benjamin Button", Guid.Empty.ToString(), null, null))};
		}

	    public async Task<(int? documentId, string chapterSlug)> GetFirstConvertedDocumentAndChapterSlug(int consultationId)
	    {
		    return (documentId: 1, chapterSlug: "my-chapter-slug");
	    }

		#region Not Implemented Members
		public IEnumerable<BreadcrumbLink> GetBreadcrumbs(ConsultationDetail consultation, bool isReview)
	    {
		    throw new NotImplementedException();
	    }

	    public async Task<string> GetFirstChapterSlug(int consultationId, int documentId)
	    {
		    throw new NotImplementedException();
	    }

	    public async Task<string> GetFirstChapterSlugFromPreviewDocument(string reference, int consultationId, int documentId)
	    {
		    throw new NotImplementedException();
	    }

	    public (int rowsUpdated, Validate validate) SubmitCommentsAndAnswers(ViewModels.Submission submission)
		{
			throw new NotImplementedException();
		}

		public async Task<ChapterContent> GetChapterContent(int consultationId, int documentId, string chapterSlug)
	    {
		    throw new NotImplementedException();
	    }

	    public async Task<(IEnumerable<Document> documents, string consultationTitle)> GetDocuments(int consultationId, string reference = null, bool draft = false)
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

	    public async Task<IEnumerable<Document>> GetPreviewDraftDocuments(int consultationId, int documentId, string reference)
	    {
		    throw new NotImplementedException();
	    }

	    public async Task<IEnumerable<Document>> GetPreviewPublishedDocuments(int consultationId, int documentId)
	    {
		    throw new NotImplementedException();
	    }

	    public async Task<Consultation> GetConsultation(int consultationId, BreadcrumbType breadcrumbType, bool useFilter)
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

	    public async Task<Consultation> GetDraftConsultation(int consultationId, int documentId, string reference, bool isReview)
	    {
		    throw new NotImplementedException();
	    }

	    public async Task<IEnumerable<BreadcrumbLink>> GetBreadcrumbs(ConsultationDetail consultation, BreadcrumbType breadcrumbType)
	    {
		    throw new NotImplementedException();
	    }

		List<string> IConsultationService.GetEmailAddressForComment(CommentsAndQuestions commentsAndQuestions)
		{
			throw new NotImplementedException();
		}

		#endregion Not Implemented Members
	}
}
