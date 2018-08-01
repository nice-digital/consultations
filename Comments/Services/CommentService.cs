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

            
            var locationToSave = new Models.Location(comment as ViewModels.Location);
            locationToSave.SourceURI = ConsultationsUri.ConvertToConsultationsUri(comment.SourceURI, CommentOnHelpers.GetCommentOn(comment.CommentOn));
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

			return new CommentsAndQuestions(data.comments, data.questions, user.IsAuthorised, signInURL, consultationState);
	    }

		/// <summary>
		/// TODO: this needs to change to pass in the state of the filters.
		/// </summary>
		/// <param name="consultationId"></param>
		/// <param name="commentsAndQuestions"></param>
		/// <returns></returns>
	    public IEnumerable<TopicListFilterGroup> GetFilterGroups(int consultationId, CommentsAndQuestions commentsAndQuestions)
	    {
		    var filters = AppSettings.ReviewConfig.Filters;

		    var questionsAndCommentsFilter = filters.Single(f => f.Id.Equals("questionscomments", StringComparison.OrdinalIgnoreCase));
			var questionOption = questionsAndCommentsFilter.Options.Single(o => o.Id.Equals("questions", StringComparison.OrdinalIgnoreCase));
		    var commentsOption = questionsAndCommentsFilter.Options.Single(o => o.Id.Equals("comments", StringComparison.OrdinalIgnoreCase));
			
		    var documentsFilter = filters.Single(f => f.Id.Equals("documents", StringComparison.OrdinalIgnoreCase));

			//questions
			questionOption.IsSelected = false; //todo 
		    questionOption.FilteredResultCount = commentsAndQuestions.Questions.Count(q => q.Answers != null && q.Answers.Any()); //todo 
			questionOption.UnfilteredResultCount = commentsAndQuestions.Questions.Count(); //todo 

			//comments
			commentsOption.IsSelected = false; //todo 
		    commentsOption.FilteredResultCount = commentsAndQuestions.Comments.Count(); //todo 
			commentsOption.UnfilteredResultCount = commentsAndQuestions.Comments.Count(); //todo 

			//populate documents
			var documents = _consultationService.GetDocuments(consultationId).ToList();
		    documentsFilter.Options = new List<TopicListFilterOption>(documents.Count());

			foreach (var document in documents)
			{
				documentsFilter.Options.Add(
					new TopicListFilterOption($"doc-{document.DocumentId}", document.Title, isSelected: false) //todo
					{
						//todo 
						FilteredResultCount = commentsAndQuestions.Comments.Count(c => c.DocumentId.HasValue && c.DocumentId.Equals(document.DocumentId)) +
						                      commentsAndQuestions.Questions.Count(q => q.DocumentId.HasValue && q.DocumentId.Equals(document.DocumentId) && q.Answers != null && q.Answers.Any()), //this assumes 1 answer per question. no good for admins..

						//todo 
						UnfilteredResultCount = commentsAndQuestions.Comments.Count() +
						                        commentsAndQuestions.Questions.Count()
					}
				);
		    }
			return filters;
	    }
	}
}
