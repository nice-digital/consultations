using Comments.Common;
using Comments.Models;
using Comments.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NICE.Auth.NetCore.Services;

namespace Comments.Services
{
    public interface ICommentService
    {
        CommentsAndQuestions GetCommentsAndQuestions(string relativeURL);
	    CommentsAndQuestions GetUsersCommentsAndQuestionsForConsultation(int consultationId);
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
        private readonly User _currentUser;

        public CommentService(ConsultationsContext context, IUserService userService, IAuthenticateService authenticateService)
        {
            _context = context;
            _userService = userService;
            _authenticateService = authenticateService;
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
            
            var commentToSave = new Models.Comment(comment.LocationId, _currentUser.UserId.Value, comment.CommentText, _currentUser.UserId.Value, locationToSave, 1, null);
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

            if (!user.IsAuthorised)
                return new CommentsAndQuestions(new List<ViewModels.Comment>(), new List<ViewModels.Question>(), user.IsAuthorised, signInURL);

            var sourceURIs = new List<string>
            {
                ConsultationsUri.ConvertToConsultationsUri(relativeURL, CommentOn.Consultation),
                ConsultationsUri.ConvertToConsultationsUri(relativeURL, CommentOn.Document),
                ConsultationsUri.ConvertToConsultationsUri(relativeURL, CommentOn.Chapter)
            };

            var locations = _context.GetAllCommentsAndQuestionsForDocument(sourceURIs);

            var commentsData = new List<ViewModels.Comment>();
            var questionsData = new List<ViewModels.Question>();
            foreach (var location in locations)
            {
                commentsData.AddRange(location.Comment.Select(comment => new ViewModels.Comment(location, comment)));
                questionsData.AddRange(location.Question.Select(question => new ViewModels.Question(location, question)));
            }

            return new CommentsAndQuestions(commentsData, questionsData, user.IsAuthorised, signInURL);
        }

        public CommentsAndQuestions GetUsersCommentsAndQuestionsForConsultation(int consultationId)
        {
            var user = _userService.GetCurrentUser();

            if (!user.IsAuthorised)
                return new CommentsAndQuestions(new List<ViewModels.Comment>(), new List<ViewModels.Question>(), user.IsAuthorised, null);

	        var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);


			var locations = _context.GetAllCommentsAndQuestionsForConsultation(sourceURI);

            var commentsData = new List<ViewModels.Comment>();
            var questionsData = new List<ViewModels.Question>();
            foreach (var location in locations)
            {
                commentsData.AddRange(location.Comment.Select(comment => new ViewModels.Comment(location, comment)));
                questionsData.AddRange(location.Question.Select(question => new ViewModels.Question(location, question)));
            }

            return new CommentsAndQuestions(commentsData, questionsData, user.IsAuthorised, null);
        }

		public CommentsAndAnswers GetUsersCommentsAndAnswersForConsultation(int consultationId)  //TODO: Can this be refactored with GetUsersCommentsAndQuestionsForConsultation? 
		{
			var user = _userService.GetCurrentUser();

			if (!user.IsAuthorised)
				return new CommentsAndAnswers(new List<ViewModels.Comment>(), new List<ViewModels.Answer>());

			var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);

			var locations = _context.GetAllCommentsAndQuestionsForConsultation(sourceURI);

			var commentsData = new List<ViewModels.Comment>();
			var answersData = new List<ViewModels.Answer>();
			foreach (var location in locations)
			{
				commentsData.AddRange(location.Comment.Select(comment => new ViewModels.Comment(location, comment)));

				foreach (var question in location.Question)
				{
					answersData.AddRange(question.Answer.Select(answer => new ViewModels.Answer(answer)));
				}
			}

			return new CommentsAndAnswers(commentsData, answersData);
		}
	}
}
