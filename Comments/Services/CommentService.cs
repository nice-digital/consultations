using Comments.Common;
using Comments.Models;
using Comments.ViewModels;
using NICE.Auth.NetCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Configuration;

namespace Comments.Services
{
	public interface ICommentService
    {
	    CommentsAndQuestions GetCommentsAndQuestions(string relativeURL);
	    ReviewPageViewModel GetCommentsAndQuestionsForReview(string relativeURL, ReviewPageViewModel model);
		(ViewModels.Comment comment, Validate validate) GetComment(int commentId);
        (int rowsUpdated, Validate validate) EditComment(int commentId, ViewModels.Comment comment);
        (ViewModels.Comment comment, Validate validate) CreateComment(ViewModels.Comment comment);
        (int rowsUpdated, Validate validate) DeleteComment(int commentId);
	}

    public class CommentService : ICommentService
    {
        private readonly ConsultationsContext _context;
        private readonly IUserService _userService;
        private readonly IAuthenticateService _authenticateService;
	    private readonly IConsultationService _consultationService;
	    private readonly ISubmitService _submitService;
	    private readonly User _currentUser;

        public CommentService(ConsultationsContext context, IUserService userService, IAuthenticateService authenticateService, IConsultationService consultationService)
        {
            _context = context;
            _userService = userService;
            _authenticateService = authenticateService;
	        _consultationService = consultationService;
	        _currentUser = _userService.GetCurrentUser();
        }

	    public (ViewModels.Comment comment, Validate validate) GetComment(int commentId)
        {
            if (!_currentUser.IsAuthorised)
                return (comment: null, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in accessing comment id:{commentId}"));

            var commentInDatabase = _context.GetComment(commentId);

            if (commentInDatabase == null)
                return (comment: null, validate: new Validate(valid: false, notFound: true, message: $"Comment id:{commentId} not found trying to get comment for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));

            if (!commentInDatabase.CreatedByUserId.Equals(_currentUser.UserId.Value))
                return (comment: null, validate: new Validate(valid: false, unauthorised: true, message: $"User id: {_currentUser.UserId} display name: {_currentUser.DisplayName} tried to access comment id: {commentId}, but it's not their comment"));

            return (comment: new ViewModels.Comment(commentInDatabase.Location, commentInDatabase), validate: null); 
        }

        public (int rowsUpdated, Validate validate) EditComment(int commentId, ViewModels.Comment comment)
        {
            if (!_currentUser.IsAuthorised)
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in editing comment id:{commentId}"));

            var commentInDatabase = _context.GetComment(commentId);

            if (commentInDatabase == null)
                return (rowsUpdated: 0, validate: new Validate(valid: false, notFound: true, message: $"Comment id:{commentId} not found trying to edit comment for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));

            if (!commentInDatabase.CreatedByUserId.Equals(_currentUser.UserId.Value))
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"User id: {_currentUser.UserId} display name: {_currentUser.DisplayName} tried to edit comment id: {commentId}, but it's not their comment"));

            comment.LastModifiedByUserId = _currentUser.UserId.Value;
            comment.LastModifiedDate = DateTime.UtcNow;
            commentInDatabase.UpdateFromViewModel(comment);
            return (rowsUpdated: _context.SaveChanges(), validate: null);
        }

        public (ViewModels.Comment comment, Validate validate) CreateComment(ViewModels.Comment comment)
        {
            if (!_currentUser.IsAuthorised)
                return (comment: null, validate: new Validate(valid: false, unauthorised: true, message: "Not logged in creating comment"));


			var sourceURI = ConsultationsUri.ConvertToConsultationsUri(comment.SourceURI, CommentOnHelpers.GetCommentOn(comment.CommentOn));
			var order = GetOrder(comment.Order, sourceURI);
	        var locationToSave = new Models.Location(comment as ViewModels.Location)
	        {
		        SourceURI = sourceURI,
				Order = order
	        };
			_context.Location.Add(locationToSave);

	        var status = _context.GetStatus(StatusName.Draft);
			var commentToSave = new Models.Comment(comment.LocationId, _currentUser.UserId.Value, comment.CommentText, _currentUser.UserId.Value, locationToSave, status.StatusId, null);
            _context.Comment.Add(commentToSave);
            _context.SaveChanges();

            return (comment: new ViewModels.Comment(locationToSave, commentToSave), validate: null);
        }

        public (int rowsUpdated, Validate validate) DeleteComment(int commentId)
        {
            if (!_currentUser.IsAuthorised)
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in deleting comment id:{commentId}"));

            var commentInDatabase = _context.GetComment(commentId);

            if (commentInDatabase == null)
                return (rowsUpdated: 0, validate: new Validate(valid: false, notFound: true, message: $"Comment id:{commentId} not found trying to delete comment for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));

            if (!commentInDatabase.CreatedByUserId.Equals(_currentUser.UserId.Value))
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"User id: {_currentUser.UserId} display name: {_currentUser.DisplayName} tried to delete comment id: {commentId}, but it's not their comment"));

            commentInDatabase.IsDeleted = true;
            commentInDatabase.LastModifiedDate = DateTime.UtcNow;
            commentInDatabase.LastModifiedByUserId = _currentUser.UserId.Value;
            return (rowsUpdated: _context.SaveChanges(), validate: null);
        }

	    public CommentsAndQuestions GetCommentsAndQuestions(string relativeURL)
	    {
		    var user = _userService.GetCurrentUser();
		    var signInURL = _authenticateService.GetLoginURL(relativeURL.ToConsultationsRelativeUrl());
		    var isReview = ConsultationsUri.IsReviewPageRelativeUrl(relativeURL);
			var consultationSourceURI = ConsultationsUri.ConvertToConsultationsUri(relativeURL, CommentOn.Consultation);
		    ConsultationState consultationState;

		    if (!user.IsAuthorised)
		    {
			    consultationState = _consultationService.GetConsultationState(consultationSourceURI);
				return new CommentsAndQuestions(new List<ViewModels.Comment>(), new List<ViewModels.Question>(),
				    user.IsAuthorised, signInURL, consultationState);
		    }

		    var sourceURIs = new List<string> { consultationSourceURI };
		    if (!isReview)
		    {
				sourceURIs.Add(ConsultationsUri.ConvertToConsultationsUri(relativeURL, CommentOn.Document));
			    sourceURIs.Add(ConsultationsUri.ConvertToConsultationsUri(relativeURL, CommentOn.Chapter));
			}

			var locations = _context.GetAllCommentsAndQuestionsForDocument(sourceURIs, isReview).ToList();

		    var data = ModelConverters.ConvertLocationsToCommentsAndQuestionsViewModels(locations);

		    consultationState = _consultationService.GetConsultationState(consultationSourceURI, locations);

			return new CommentsAndQuestions(data.comments.ToList(), data.questions.ToList(), user.IsAuthorised, signInURL, consultationState);
	    }

		public ReviewPageViewModel GetCommentsAndQuestionsForReview(string relativeURL, ReviewPageViewModel model)
		{
			var commentsAndQuestions = GetCommentsAndQuestions(relativeURL);

			if (model.Sort == ReviewSortOrder.DocumentAsc)
			{
				commentsAndQuestions.Comments = commentsAndQuestions.Comments.OrderByAlphaNumeric(c => c.Order).ToList();
				//commentsAndQuestions.Questions = commentsAndQuestions.Questions.OrderByAlphaNumeric(q => q.Order).ToList();
			}

			model.CommentsAndQuestions = FilterCommentsAndQuestions(commentsAndQuestions, model.Type, model.Document);

			var consultationId = ConsultationsUri.ParseRelativeUrl(relativeURL).ConsultationId;
			model.Filters = GetFilterGroups(consultationId, commentsAndQuestions, model.Type, model.Document);
			return model;
		}

		/// <summary>
		/// Filtering is just setting a Show property on the comments and questions to false.
		/// </summary>
		/// <param name="commentsAndQuestions"></param>
		/// <param name="questionsOrComments"></param>
		/// <param name="documentIdsToFilter"></param>
		/// <returns></returns>
		private static CommentsAndQuestions FilterCommentsAndQuestions(CommentsAndQuestions commentsAndQuestions, IEnumerable<QuestionsOrComments> type, IEnumerable<int> documentIdsToFilter)
	    {
		    commentsAndQuestions.Questions.ForEach(q => q.Show = type == null ||  type.Contains(QuestionsOrComments.Questions));
		    commentsAndQuestions.Comments.ForEach(q => q.Show = type == null || type.Contains(QuestionsOrComments.Comments));

		    var idsToFilter = documentIdsToFilter?.ToList() ?? new List<int>(0);
		    if (idsToFilter.Any())
		    {
			    commentsAndQuestions.Questions.ForEach(q => q.Show = (!q.Show || !q.DocumentId.HasValue) ? false : idsToFilter.Contains(q.DocumentId.Value));
			    commentsAndQuestions.Comments.ForEach(c => c.Show = (!c.Show || !c.DocumentId.HasValue) ? false : idsToFilter.Contains(c.DocumentId.Value));
			}
		    return commentsAndQuestions;
	    }

	    private IEnumerable<ReviewFilterGroup> GetFilterGroups(int consultationId, CommentsAndQuestions commentsAndQuestions, IEnumerable<QuestionsOrComments> type, IEnumerable<int> documentIdsToFilter)
	    {
		    var filters = AppSettings.ReviewConfig.Filters.ToList();

		    var questionsAndCommentsFilter = filters.Single(f => f.Id.Equals("Type", StringComparison.OrdinalIgnoreCase));
			var questionOption = questionsAndCommentsFilter.Options.Single(o => o.Id.Equals("Questions", StringComparison.OrdinalIgnoreCase));
		    var commentsOption = questionsAndCommentsFilter.Options.Single(o => o.Id.Equals("Comments", StringComparison.OrdinalIgnoreCase));
			
		    var documentsFilter = filters.Single(f => f.Id.Equals("Document", StringComparison.OrdinalIgnoreCase));

			//questions
		    questionOption.IsSelected = type != null && type.Contains(QuestionsOrComments.Questions);
		    questionOption.FilteredResultCount = commentsAndQuestions.Questions.Count(q => q.Show); 
			questionOption.UnfilteredResultCount = commentsAndQuestions.Questions.Count();

			//comments
			commentsOption.IsSelected = type != null && type.Contains(QuestionsOrComments.Comments);
			commentsOption.FilteredResultCount = commentsAndQuestions.Comments.Count(q => q.Show); 
			commentsOption.UnfilteredResultCount = commentsAndQuestions.Comments.Count(); 

			//populate documents
			var documents = _consultationService.GetDocuments(consultationId).Where(d => d.ConvertedDocument).ToList();
		    documentsFilter.Options = new List<ReviewFilterOption>(documents.Count());

			foreach (var document in documents)
			{
				var isSelected = documentIdsToFilter != null && documentIdsToFilter.Contains(document.DocumentId);
				documentsFilter.Options.Add(
					new ReviewFilterOption(document.DocumentId.ToString(), document.Title, isSelected)
					{
						FilteredResultCount = commentsAndQuestions.Comments.Count(c => c.Show && c.DocumentId.HasValue && c.DocumentId.Equals(document.DocumentId)) +
						                      commentsAndQuestions.Questions.Count(q => q.Show && q.DocumentId.HasValue && q.DocumentId.Equals(document.DocumentId)),

						UnfilteredResultCount = commentsAndQuestions.Comments.Count(c => c.DocumentId.HasValue && c.DocumentId.Equals(document.DocumentId)) +
						                        commentsAndQuestions.Questions.Count(q => q.DocumentId.HasValue && q.DocumentId.Equals(document.DocumentId))
					}
				);
		    }
			return filters;
	    }

	    public const string OrderFormat = "{0}.{1}.{2}.{3}";
	    public string GetOrder(string orderWithinChapter, string sourceURI)
	    {
		    var consultationsUriElements = ConsultationsUri.ParseConsultationsUri(sourceURI);

			var consultationId = consultationsUriElements.ConsultationId;
			var documentId = consultationsUriElements.DocumentId;
		    int? chapterIndex = null;
		    
			if (consultationsUriElements.IsChapterLevel())
			{
				var document = _consultationService.GetDocuments(consultationsUriElements.ConsultationId).FirstOrDefault(d => d.DocumentId.Equals(documentId));
				var chapterSelected = document?.Chapters.FirstOrDefault(c => c.Slug.Equals(consultationsUriElements.ChapterSlug, StringComparison.OrdinalIgnoreCase));
				if (chapterSelected != null)
				{
					chapterIndex = document.Chapters.Select((chapter, index) => new {chapter, index}).FirstOrDefault(c => c.chapter.Slug.Equals(chapterSelected.Slug, StringComparison.OrdinalIgnoreCase))?.index;
				}
			}
			
		    return string.Format(OrderFormat, consultationId, documentId ?? 0, chapterIndex ?? 0, orderWithinChapter);
	    }
	}
}
