using Comments.Common;
using Comments.Configuration;
using Comments.Models;
using Comments.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Word;
using NICE.Feeds.Indev;
using NICE.Feeds.Indev.Models;
using NICE.Feeds.Indev.Models.Detail;
using NICE.Feeds.Indev.Models.List;

namespace Comments.Services
{
	public enum BreadcrumbType
	{
		DocumentPage,
		Review,
		None
	}

	public interface IConsultationService
    {
        Task<ChapterContent> GetChapterContent(int consultationId, int documentId, string chapterSlug);
        Task<(IEnumerable<Document> documents, string consultationTitle)> GetDocuments(int consultationId, string reference = null, bool draft = false);
		Task<IEnumerable<Document>> GetPreviewDraftDocuments(int consultationId, int documentId, string reference);
	    Task<IEnumerable<Document>> GetPreviewPublishedDocuments(int consultationId, int documentId);
	    Task<ViewModels.Consultation> GetConsultation(int consultationId, BreadcrumbType breadcrumbType, bool useFilters);
	    Task<ViewModels.Consultation> GetDraftConsultation(int consultationId, int documentId, string reference, bool isReview);

	    Task<IEnumerable<ViewModels.Consultation>> GetConsultations();

	    Task<ChapterContent> GetPreviewChapterContent(int consultationId, int documentId, string chapterSlug, string reference);
	    Task<ConsultationState> GetConsultationState(string sourceURI, PreviewState previewState, IEnumerable<Models.Location> locations = null, ConsultationBase consultation = null);
	    Task<ConsultationState> GetConsultationState(int consultationId, int? documentId, string reference, PreviewState previewState, IEnumerable<Models.Location> locations = null, ConsultationBase consultationDetail = null);

		DateTime? GetSubmittedDate(string consultationSourceURI);
	    Task<IEnumerable<BreadcrumbLink>> GetBreadcrumbs(ConsultationDetail consultation, BreadcrumbType breadcrumbType);

	    Task<(int? documentId, string chapterSlug)> GetFirstConvertedDocumentAndChapterSlug(int consultationId);
	    Task<string> GetFirstChapterSlug(int consultationId, int documentId);
	    Task<string> GetFirstChapterSlugFromPreviewDocument(string reference, int consultationId, int documentId);

		List<string> GetEmailAddressForComment(CommentsAndQuestions commentsAndQuestions);
	}

	public class ConsultationService : IConsultationService
    {
	    private readonly ConsultationsContext _context;
	    private readonly IIndevFeedService _feedService;
        private readonly ILogger<ConsultationService> _logger;
        private readonly IUserService _userService;

		public ConsultationService(ConsultationsContext context, IIndevFeedService feedService, ILogger<ConsultationService> logger, IUserService userService)
        {
	        _context = context;
	        _feedService = feedService;
            _logger = logger;
            _userService = userService;
		}

        public async Task<ChapterContent> GetChapterContent(int consultationId, int documentId, string chapterSlug)
        {
            return new ViewModels.ChapterContent(
                await _feedService.GetConsultationChapterForPublishedProject(consultationId, documentId, chapterSlug));
        }

	    public async Task<ChapterContent> GetPreviewChapterContent(int consultationId, int documentId, string chapterSlug, string reference)
	    {
		    return new ViewModels.ChapterContent(
			    await _feedService.GetIndevConsultationChapterForDraftProject(consultationId, documentId, chapterSlug, reference));
	    }

	    public async Task<(IEnumerable<Document> documents, string consultationTitle)> GetDocuments(int consultationId, string reference = null, bool draft = false)
        {
	        if (draft)
	        {
		        var consultationPreviewDetail = await _feedService.GetIndevConsultationDetailForDraftProject(consultationId, Constants.DummyDocumentNumberForPreviewProject, reference);
		        return (consultationPreviewDetail.Resources.Select(r => new ViewModels.Document(consultationId, r)).ToList(), consultationPreviewDetail.ConsultationName);
			}

	        var consultationDetail = await _feedService.GetIndevConsultationDetailForPublishedProject(consultationId, PreviewState.NonPreview);
            return (consultationDetail.Resources.Select(r => new ViewModels.Document(consultationId, r)).ToList(), consultationDetail.ConsultationName);
        }


	    public async Task<IEnumerable<Document>> GetPreviewPublishedDocuments(int consultationId, int documentId)
	    {
		    var consultationDetail = await _feedService.GetIndevConsultationDetailForPublishedProject(consultationId, PreviewState.Preview, documentId);
		    return consultationDetail.Resources.Select(r => new ViewModels.Document(consultationId, r)).ToList();
	    }

		public async Task<IEnumerable<Document>> GetPreviewDraftDocuments(int consultationId, int documentId, string reference)
	    {
		    var consultationDetail = await _feedService.GetIndevConsultationDetailForDraftProject(consultationId, documentId, reference);
		    return consultationDetail.Resources.Select(r => new ViewModels.Document(consultationId, r)).ToList();
	    }

        public async Task<ViewModels.Consultation> GetConsultation(int consultationId, BreadcrumbType breadcrumbType, bool useFilters)
        {
            var user = _userService.GetCurrentUser();
	        var consultationDetail = await GetConsultationDetail(consultationId);
	        var consultationState = await GetConsultationState(consultationId, null, null, PreviewState.NonPreview, null, consultationDetail);
	        var breadcrumbs = await GetBreadcrumbs(consultationDetail, breadcrumbType);
	        var filters = useFilters ? AppSettings.ReviewConfig.Filters : null;
            return new ViewModels.Consultation(consultationDetail, user, breadcrumbs, consultationState, filters);
        }

	    public async Task<ViewModels.Consultation> GetDraftConsultation(int consultationId, int documentId, string reference, bool isReview)
	    {
		    var user = _userService.GetCurrentUser();
		    var draftConsultationDetail = await GetDraftConsultationDetail(consultationId, documentId, reference);
		    var consultationState = await GetConsultationState(consultationId, documentId, reference, PreviewState.Preview, null, draftConsultationDetail);
		    var filters = isReview ? AppSettings.ReviewConfig.Filters : null;
		    return new ViewModels.Consultation(draftConsultationDetail, user, null, consultationState, filters);
	    }

		public async Task<IEnumerable<BreadcrumbLink>> GetBreadcrumbs(ConsultationDetail consultation, BreadcrumbType breadcrumbType)
	    {
		    if (breadcrumbType == BreadcrumbType.None)
		    {
			    return null;
		    }

			var breadcrumbs = new List<BreadcrumbLink>{
					new BreadcrumbLink("Home", Routes.External.HomePage),
					new BreadcrumbLink("All consultations", Routes.External.InconsultationListPage),
					new BreadcrumbLink("Consultation responses", Routes.Internal.DownloadPageRoute, localRoute: true),
					new BreadcrumbLink(consultation.Title, Routes.External.ConsultationUrl(consultation))
			};

		    if (breadcrumbType == BreadcrumbType.Review)
		    {
			    var firstDocument = (await GetDocuments(consultation.ConsultationId)).documents.FirstOrDefault(d => d.ConvertedDocument && d.DocumentId > 0);
			    var firstChapter = firstDocument?.Chapters.FirstOrDefault();

			    if (firstChapter != null)
				    breadcrumbs.Add(new BreadcrumbLink("Consultation documents", $"/{consultation.ConsultationId}/{firstDocument.DocumentId}/{firstChapter.Slug}", true));
		    }

		    return breadcrumbs;
	    }

	    public async Task<(int? documentId, string chapterSlug)> GetFirstConvertedDocumentAndChapterSlug(int consultationId)
	    {
			var firstDocument = (await GetDocuments(consultationId)).documents.FirstOrDefault(d => d.ConvertedDocument && d.DocumentId > 0);
		    if (firstDocument == null)
			    return (null, null);

			var chapterSlug = firstDocument.Chapters.FirstOrDefault()?.Slug;
			return (firstDocument.DocumentId, chapterSlug);
	    }

	    public async Task<string> GetFirstChapterSlug(int consultationId, int documentId)
	    {
		    return (await GetDocuments(consultationId)).documents.FirstOrDefault(d => d.DocumentId.Equals(documentId))?.Chapters.FirstOrDefault()?.Slug;
	    }
	    public async Task<string> GetFirstChapterSlugFromPreviewDocument(string reference, int consultationId, int documentId)
	    {
		    return (await GetPreviewDraftDocuments(consultationId, documentId, reference)).FirstOrDefault(d => d.DocumentId.Equals(documentId))?.Chapters.FirstOrDefault()?.Slug;
	    }

		public async Task<IEnumerable<ViewModels.Consultation>> GetConsultations()
        {
            var user = _userService.GetCurrentUser();
            var consultations = await _feedService.GetConsultationList();
            return consultations.Select(c => new ViewModels.Consultation(c, user)).ToList();
        }

	    public async Task<ConsultationState> GetConsultationState(string sourceURI, PreviewState previewState, IEnumerable<Models.Location> locations = null, ConsultationBase consultation = null)
	    {
		    var consultationsUriElements = ConsultationsUri.ParseConsultationsUri(sourceURI);
		    return await GetConsultationState(consultationsUriElements.ConsultationId, consultationsUriElements.DocumentId, null, previewState, locations, consultation);
	    }

	    public async Task<ConsultationState> GetConsultationState(int consultationId, int? documentId, string reference, PreviewState previewState, IEnumerable<Models.Location> locations = null, ConsultationBase consultationDetail = null)
	    {
		    if (previewState == PreviewState.Preview)
		    {
			    if (documentId == null)
				    throw new ArgumentException(nameof(documentId));
				if (reference == null)
					throw new ArgumentException(nameof(reference));
			}

		    var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);
		    if (consultationDetail == null)
				if (previewState ==  PreviewState.NonPreview)
					consultationDetail = await GetConsultationDetail(consultationId);
				else
					consultationDetail = await GetDraftConsultationDetail(consultationId, (int)documentId, reference);

		    IEnumerable<Document> documents;
		    if (previewState == PreviewState.NonPreview)
			    documents = (await GetDocuments(consultationId)).documents.ToList();
			else
				documents = (await GetPreviewDraftDocuments(consultationId, (int)documentId, reference)).ToList();

		   // var documentsWhichSupportQuestions = documents.Where(d => d.SupportsQuestions).Select(d => d.DocumentId).ToList();
		    var documentsWhichSupportComments = documents.Where(d => d.SupportsComments).Select(d => d.DocumentId).ToList();

		    var currentUser = _userService.GetCurrentUser();

		    if (currentUser.IsAuthenticatedByAnyMechanism)
		    {
			    locations = locations ?? _context.GetAllCommentsAndQuestionsForDocument(new[] { sourceURI }, partialMatchSourceURI: true);
		    }
		    else
		    {
			    locations = _context.GetQuestionsForDocument(new[] {sourceURI}, partialMatchSourceURI: true);
		    }
		    
		    var data = ModelConverters.ConvertLocationsToCommentsAndQuestionsViewModels(locations);

		    var consultationState = new ConsultationState(consultationDetail.StartDate, consultationDetail.EndDate,
			    data.questions.Any(), data.questions.Any(q => q.Answers.Any()), data.comments.Any(),GetSubmittedDate(sourceURI),
			    documentsWhichSupportComments);

		    return consultationState;
	    }

		public DateTime? GetSubmittedDate(string anySourceURI)
	    {
		    if (string.IsNullOrWhiteSpace(anySourceURI))
			    return null;

		    var consultationsUriElements = ConsultationsUri.ParseConsultationsUri(anySourceURI);

		    var consultationSourceURI = ConsultationsUri.CreateConsultationURI(consultationsUriElements.ConsultationId);

		    return _context.GetSubmittedDate(consultationSourceURI);
	    }

	    /// <summary>
	    /// This is intentionally private as it gets the ConsultationDetail straight from the feed. not for external consumption outside of this class.
	    /// </summary>
	    /// <param name="consultationId"></param>
	    /// <returns></returns>
	    private async Task<ConsultationDetail> GetConsultationDetail(int consultationId)
	    {
		    var consultationDetail = await _feedService.GetIndevConsultationDetailForPublishedProject(consultationId, PreviewState.NonPreview);
		    return consultationDetail;
	    }

		/// <summary>
		/// This is intentionally private as it gets the ConsultationDetail straight from the feed. not for external consumption outside of this class.
		/// </summary>
		/// <param name="consultationId"></param>
		/// <param name="documentId"></param>
		/// <param name="reference"></param>
		/// <returns></returns>
		private async Task<ConsultationPublishedPreviewDetail> GetDraftConsultationDetail(int consultationId, int documentId,  string reference)
	    {
		    var consultationDetail = await _feedService.GetIndevConsultationDetailForDraftProject(consultationId, documentId, reference);
		    return consultationDetail;
	    }


		//public ConsultationState GetDraftConsultationState(int consultationId, int documentId, string reference, IEnumerable<Models.Location> locations = null, ConsultationPublishedPreviewDetail consultationDetail = null)
		//{
		// var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);
		// if (consultationDetail == null)
		//  consultationDetail = GetDraftConsultationDetail(consultationId, documentId, reference);

		// var documents = GetPreviewDraftDocuments(consultationId, documentId, reference).ToList();
		// var documentsWhichSupportQuestions = documents.Where(d => d.SupportsQuestions).Select(d => d.DocumentId).ToList();
		// var documentsWhichSupportComments = documents.Where(d => d.SupportsComments).Select(d => d.DocumentId).ToList();

		// var currentUser = _userService.GetCurrentUser();

		// if (locations == null && currentUser.IsAuthenticated && currentUser.UserId.HasValue)
		// {
		//  locations = _context.GetAllCommentsAndQuestionsForDocument(new[] { sourceURI }, partialMatchSourceURI: true);
		// }
		// else
		// {
		//  locations = new List<Models.Location>(0);
		// }

		// var hasSubmitted = currentUser != null && currentUser.IsAuthenticated && currentUser.UserId.HasValue ? GetSubmittedDate(sourceURI, currentUser.UserId.Value) : false;

		// var data = ModelConverters.ConvertLocationsToCommentsAndQuestionsViewModels(locations);

		// var consultationState = new ConsultationState(consultationDetail.StartDate, consultationDetail.EndDate,
		//  data.questions.Any(), data.questions.Any(q => q.Answers.Any()), data.comments.Any(), hasSubmitted,
		//  false, false, documentsWhichSupportQuestions, documentsWhichSupportComments);

		// return consultationState;
		//}

		//TODO: Move to OrganisationService
		public List<string> GetEmailAddressForComment(CommentsAndQuestions commentsAndQuestions)
		{
			var commentIds = commentsAndQuestions.Comments.Select(c => c.CommentId).ToList();
			var questionIds = commentsAndQuestions.Questions.Select(q => q.QuestionId).ToList();

			var organisationUserIds = _context.Comment.Where(c => commentIds.Contains(c.CommentId))
													.Select(c => c.OrganisationUserId)
													.Distinct()
													.ToList();

			var emailAddresses = _context.OrganisationUser.Where(o => organisationUserIds.Contains(o.OrganisationUserId))
														.Select(o => o.EmailAddress)
														.ToList();

			return emailAddresses;
		}
	}
}
