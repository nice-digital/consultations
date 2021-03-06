﻿using Comments.Common;
using Comments.Configuration;
using Comments.Models;
using Comments.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NICE.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NICE.Feeds.Indev.Models;
using Comment = Comments.Models.Comment;

namespace Comments.Services
{
	public interface ICommentService
    {
	    Task<CommentsAndQuestions> GetCommentsAndQuestions(string relativeURL, IUrlHelper urlHelper);
	    Task<ReviewPageViewModel> GetCommentsAndQuestionsForReview(string relativeURL, IUrlHelper urlHelper, ReviewPageViewModel model);
	    OrganisationCommentsAndQuestions GetCommentsAndQuestionsFromOtherOrganisationCommenters(string relativeURL, IUrlHelper urlHelper);
		(ViewModels.Comment comment, Validate validate) GetComment(int commentId);
        (int rowsUpdated, Validate validate) EditComment(int commentId, ViewModels.Comment comment);
        Task<(ViewModels.Comment comment, Validate validate)> CreateComment(ViewModels.Comment comment);
        (int rowsUpdated, Validate validate) DeleteComment(int commentId);
	}

    public class CommentService : ICommentService
    {
        private readonly ConsultationsContext _context;
        private readonly IUserService _userService;
	    private readonly IConsultationService _consultationService;
	    private readonly IHttpContextAccessor _httpContextAccessor;
	    private readonly User _currentUser;

        public CommentService(ConsultationsContext context, IUserService userService, IConsultationService consultationService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userService = userService;
	        _consultationService = consultationService;
			_httpContextAccessor = httpContextAccessor;
	        _currentUser = _userService.GetCurrentUser();
        }

	    public (ViewModels.Comment comment, Validate validate) GetComment(int commentId)
        {
            if (!_currentUser.IsAuthenticatedByAnyMechanism)
                return (comment: null, validate: new Validate(valid: false, unauthenticated: true, message: $"Not logged in accessing comment id:{commentId}"));

            var commentInDatabase = _context.GetComment(commentId);

            if (commentInDatabase == null)
                return (comment: null, validate: new Validate(valid: false, notFound: true, message: $"Comment id:{commentId} not found trying to get comment for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));

            var consultationId = ConsultationsUri.ParseConsultationsUri(commentInDatabase.Location.SourceURI).ConsultationId;
			if (_currentUser.IsAuthenticatedByOrganisationCookieForThisConsultation(consultationId))
            {
	            if (!commentInDatabase.OrganisationUserId.HasValue || !_currentUser.IsAuthorisedByOrganisationUserId(commentInDatabase.OrganisationUserId.Value))
		            return (comment: null, validate: new Validate(valid: false, unauthenticated: true, message: $"Organisation cookie user tried to access comment id: {commentId}, but it's not their comment"));
            }
			else
			{
				if (!commentInDatabase.CreatedByUserId.Equals(_currentUser.UserId, StringComparison.OrdinalIgnoreCase))
				   return (comment: null, validate: new Validate(valid: false, unauthenticated: true, message: $"User id: {_currentUser.UserId} display name: {_currentUser.DisplayName} tried to access comment id: {commentId}, but it's not their comment"));
			}
            return (comment: new ViewModels.Comment(commentInDatabase.Location, commentInDatabase), validate: null); 
        }

        public (int rowsUpdated, Validate validate) EditComment(int commentId, ViewModels.Comment comment)
        {
            if (!_currentUser.IsAuthenticatedByAnyMechanism)
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: true, message: $"Not logged in editing comment id:{commentId}"));

            var commentInDatabase = _context.GetComment(commentId);

            if (commentInDatabase == null)
                return (rowsUpdated: 0, validate: new Validate(valid: false, notFound: true, message: $"Comment id:{commentId} not found trying to edit comment for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));

            // Comments created by individal commenters can only be edited by that commenter.
            if (commentInDatabase.CommentByUserType == UserType.IndividualCommenter && !_currentUser.UserId.Equals(commentInDatabase.CreatedByUserId))
	            return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: true, message: $"User id: {_currentUser.UserId} display name: {_currentUser.DisplayName} tried to edit comment id: {commentId}, but it's not their comment"));

            // Comments created by organisational commenters and not yet submitted to a lead can only be edited by that commenter
            if (commentInDatabase.CommentByUserType == UserType.OrganisationalCommenter && !_currentUser.ValidatedOrganisationUserIds.Any(o => o.Equals(commentInDatabase.OrganisationUserId)))
	            return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: false, unauthorised: true, message: $"Organisation commenter tried to edit comment id: {commentId}, but it's not their comment"));

            // Comments submitted to organisational leads or created by organisational leads can be edited by any leads for that organisation
            if (commentInDatabase.CommentByUserType == UserType.OrganisationLead && !_currentUser.OrganisationsAssignedAsLead.Any(o => o.OrganisationId.Equals(commentInDatabase.OrganisationId)))
	            return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: false, unauthorised: true, message: $"Organisation lead tried to edit comment id: {commentId}, but it's not from their organisation"));

			comment.LastModifiedByUserId = _currentUser.UserId;
            comment.LastModifiedDate = DateTime.UtcNow;
            commentInDatabase.UpdateFromViewModel(comment);
            return (rowsUpdated: _context.SaveChanges(), validate: null);
        }

        public async Task<(ViewModels.Comment comment, Validate validate)> CreateComment(ViewModels.Comment comment)
        {
            if (!_currentUser.IsAuthenticatedByAnyMechanism)
                return (comment: null, validate: new Validate(valid: false, unauthenticated: true, message: "Not logged in creating comment"));

            var sourceURI = ConsultationsUri.ConvertToConsultationsUri(comment.SourceURI, CommentOnHelpers.GetCommentOn(comment.CommentOn));

			var consultationId = ConsultationsUri.ParseConsultationsUri(sourceURI).ConsultationId;
			if (!_currentUser.IsAuthorisedByConsultationId(consultationId))
				return (comment: null, validate: new Validate(valid: false, unauthenticated: false, unauthorised: true, message: "Not authorised to create comment on this consultation"));

			comment.Order = await UpdateOrderWithSourceURI(comment.Order, sourceURI);
	        var locationToSave = new Models.Location(comment as ViewModels.Location)
	        {
		        SourceURI = sourceURI
	        };
			_context.Location.Add(locationToSave);

			int? organisationUserId = null;
			int? organisationId;
			var userId = _currentUser.UserId;
			if (_currentUser.IsAuthenticatedByOrganisationCookieForThisConsultation(consultationId))
			{
				organisationUserId = _currentUser.ValidatedSessions.FirstOrDefault(session => session.ConsultationId.Equals(consultationId))?.OrganisationUserId;
				organisationId = _currentUser.ValidatedSessions.FirstOrDefault(session => session.ConsultationId.Equals(consultationId))?.OrganisationId;
				userId = null;
			}
			else
			{
				organisationId = _currentUser.OrganisationsAssignedAsLead?.FirstOrDefault()?.OrganisationId;
			}
			var status = _context.GetStatus(StatusName.Draft);
			var commentToSave = new Models.Comment(comment.LocationId, userId, comment.CommentText, _currentUser.UserId, locationToSave, status.StatusId, null, organisationUserId: organisationUserId, organisationId: organisationId);
            _context.Comment.Add(commentToSave);
            _context.SaveChanges();

            return (comment: new ViewModels.Comment(locationToSave, commentToSave), validate: null);
        }

        public (int rowsUpdated, Validate validate) DeleteComment(int commentId)
        {
            if (!_currentUser.IsAuthenticatedByAnyMechanism)
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: true, message: $"Not logged in deleting comment id:{commentId}"));

            var commentInDatabase = _context.GetComment(commentId);

            if (commentInDatabase == null)
                return (rowsUpdated: 0, validate: new Validate(valid: false, notFound: true, message: $"Comment id:{commentId} not found trying to delete comment for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));

            // Comments created by individal commenters can only be deleted by that commenter.
            if (commentInDatabase.CommentByUserType == UserType.IndividualCommenter && !_currentUser.UserId.Equals(commentInDatabase.CreatedByUserId))
	            return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: true, message: $"User id: {_currentUser.UserId} display name: {_currentUser.DisplayName} tried to delete comment id: {commentId}, but it's not their comment"));

            // Comments created by organisational commenters and not yet submitted to a lead can only be deleted by that commenter
            if (commentInDatabase.CommentByUserType == UserType.OrganisationalCommenter && !_currentUser.ValidatedOrganisationUserIds.Any(o => o.Equals(commentInDatabase.OrganisationUserId)))
	            return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: false, unauthorised: true, message: $"Organisation commenter tried to delete comment id: {commentId}, but it's not their comment"));

            // Comments submitted to organisational leads or created by organisational leads can be deleted by any leads for that organisation
            if (commentInDatabase.CommentByUserType == UserType.OrganisationLead && !_currentUser.OrganisationsAssignedAsLead.Any(o => o.OrganisationId.Equals(commentInDatabase.OrganisationId)))
	            return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: false, unauthorised: true, message: $"Organisation lead tried to delete comment id: {commentId}, but it's not from their organisation"));

			_context.Comment.Remove(commentInDatabase);
			
            return (rowsUpdated: _context.SaveChanges(), validate: null);
        }

	    public async Task<CommentsAndQuestions> GetCommentsAndQuestions(string relativeURL, IUrlHelper urlHelper)
	    {
		    var user = _userService.GetCurrentUser();

			var signInURL = urlHelper.Action(Constants.Auth.LoginAction, Constants.Auth.ControllerName, new {returnUrl = relativeURL.ToConsultationsRelativeUrl() });

			var isReview = ConsultationsUri.IsReviewPageRelativeUrl(relativeURL);
			var consultationSourceURI = ConsultationsUri.ConvertToConsultationsUri(relativeURL, CommentOn.Consultation);
		    ConsultationState consultationState;
		    var sourceURIs = new List<string> { consultationSourceURI };
		    if (!isReview)
		    {
			    sourceURIs.Add(ConsultationsUri.ConvertToConsultationsUri(relativeURL, CommentOn.Document));
			    sourceURIs.Add(ConsultationsUri.ConvertToConsultationsUri(relativeURL, CommentOn.Chapter));
		    }

			if (!user.IsAuthenticatedByAnyMechanism)
		    {
			    consultationState = await _consultationService.GetConsultationState(consultationSourceURI, PreviewState.NonPreview);
			    var locationsQuestionsOnly = _context.GetQuestionsForDocument(sourceURIs, isReview);
			    var questions = ModelConverters.ConvertLocationsToCommentsAndQuestionsViewModels(locationsQuestionsOnly).questions.ToList();
				return new CommentsAndQuestions(new List<ViewModels.Comment>(), questions,
				    user.IsAuthenticatedByAnyMechanism, signInURL, consultationState);
		    }

			var locations = _context.GetAllCommentsAndQuestionsForDocument(sourceURIs, isReview).ToList();
		    consultationState = await _consultationService.GetConsultationState(consultationSourceURI, PreviewState.NonPreview, locations);

			var data = ModelConverters.ConvertLocationsToCommentsAndQuestionsViewModels(locations);
		    var resortedComments = data.comments.OrderByDescending(c => c.LastModifiedDate).ToList(); //comments should be sorted in date by default, questions by document order.
			
			return new CommentsAndQuestions(resortedComments, data.questions.ToList(), user.IsAuthenticatedByAnyMechanism, signInURL, consultationState);
	    }

		public async Task<ReviewPageViewModel> GetCommentsAndQuestionsForReview(string relativeURL, IUrlHelper urlHelper, ReviewPageViewModel model)
		{
			var commentsAndQuestions = await GetCommentsAndQuestions(relativeURL, urlHelper);

			if (model.Sort == ReviewSortOrder.DocumentAsc)
			{
				commentsAndQuestions.Comments = commentsAndQuestions.Comments.OrderBy(c => c.Order).ToList();
			}

			model.CommentsAndQuestions = FilterCommentsAndQuestions(commentsAndQuestions, model.Type, model.Document, model.Commenter);
			model.OrganisationName = _currentUser?.OrganisationsAssignedAsLead.FirstOrDefault()?.OrganisationName;
			model.IsLead = _currentUser?.OrganisationsAssignedAsLead.Any();

			var consultationId = ConsultationsUri.ParseRelativeUrl(relativeURL).ConsultationId;
			model.Filters = await GetFilterGroups(consultationId, commentsAndQuestions, model.Type, model.Document, model.Commenter);
			return model;
		}

		public OrganisationCommentsAndQuestions GetCommentsAndQuestionsFromOtherOrganisationCommenters(string relativeURL, IUrlHelper urlHelper)
		{
			var user = _userService.GetCurrentUser();
			
			var consultationSourceURI = ConsultationsUri.ConvertToConsultationsUri(relativeURL, CommentOn.Consultation);
			var sourceURIs = new List<string> { consultationSourceURI };
			sourceURIs.Add(ConsultationsUri.ConvertToConsultationsUri(relativeURL, CommentOn.Document));
			sourceURIs.Add(ConsultationsUri.ConvertToConsultationsUri(relativeURL, CommentOn.Chapter));

			var collectedData = _context.GetOtherOrganisationUsersCommentsAndQuestionsForDocument(sourceURIs);

            var convertedData =
                ModelConverters.ConvertLocationsToCommentsAndQuestionsViewModels(collectedData);

            var data = new OrganisationCommentsAndQuestions(convertedData.questions, convertedData.comments);

            return data;
		}

		/// <summary>
		/// Filtering is just setting a Show property on the comments and questions to false.
		/// </summary>
		/// <param name="commentsAndQuestions"></param>
		/// <param name="questionsOrComments"></param>
		/// <param name="documentIdsToFilter"></param>
		/// <returns></returns>
		public CommentsAndQuestions FilterCommentsAndQuestions(CommentsAndQuestions commentsAndQuestions, IEnumerable<QuestionsOrComments> type, IEnumerable<int> documentIdsToFilter, IEnumerable<string> commentersToFilter)
		{
			var types = type?.ToList() ?? new List<QuestionsOrComments>(0);
		    commentsAndQuestions.Questions.ForEach(q => q.Show = !types.Any() ||  types.Contains(QuestionsOrComments.Questions));
			commentsAndQuestions.Comments.ForEach(q => q.Show = !types.Any() || types.Contains(QuestionsOrComments.Comments));

			var documentIds = documentIdsToFilter?.ToList() ?? new List<int>(0);
			if (documentIds.Any())
		    {
			    commentsAndQuestions.Questions.ForEach(q => q.Show = (!q.Show || !q.DocumentId.HasValue) ? false : documentIds.Contains(q.DocumentId.Value));
			    commentsAndQuestions.Comments.ForEach(c => c.Show = (!c.Show || !c.DocumentId.HasValue) ? false : documentIds.Contains(c.DocumentId.Value));
			}

			var commenters = commentersToFilter?.ToList() ?? new List<string>();
			if (commenters.Any())
			{
				commentsAndQuestions.Comments.ForEach(c => c.Show = (!c.Show || (c.CommenterEmail != null && c.CommenterEmail.Length == 0)) ? false : commenters.Contains(c.CommenterEmail, StringComparer.OrdinalIgnoreCase));

                foreach (var question in commentsAndQuestions.Questions)
                {
                    if (question.Show == false) //if other filters have already excluded this question from showing we don't need to process its answers
                        break;

                    question.Show= false; //don't show question unless we have answer that match filter
                    foreach (var answer in question.Answers)
                    {
                        answer.showWhenFiltered = commenters.Contains(answer.CommenterEmail);
                        if (!question.Show && answer.showWhenFiltered)
                            question.Show = true;
                    }
                }
            }

		    return commentsAndQuestions;
	    }

	    private async Task<IEnumerable<OptionFilterGroup>> GetFilterGroups(int consultationId, CommentsAndQuestions commentsAndQuestions, IEnumerable<QuestionsOrComments> type, IEnumerable<int> documentIdsToFilter, IEnumerable<string> commentersToFilter)
	    {
		    var filters = AppSettings.ReviewConfig.Filters.ToList();

		    var questionsAndCommentsFilter = filters.Single(f => f.Id.Equals("Type", StringComparison.OrdinalIgnoreCase));
			var questionOption = questionsAndCommentsFilter.Options.Single(o => o.Id.Equals("Questions", StringComparison.OrdinalIgnoreCase));
		    var commentsOption = questionsAndCommentsFilter.Options.Single(o => o.Id.Equals("Comments", StringComparison.OrdinalIgnoreCase));
			
		    var documentsFilter = filters.Single(f => f.Id.Equals("Document", StringComparison.OrdinalIgnoreCase));

			var commentersFilter = filters.Single(f => f.Id.Equals("Commenter", StringComparison.OrdinalIgnoreCase));

			//questions
			questionOption.IsSelected = type != null && type.Contains(QuestionsOrComments.Questions);
		    questionOption.FilteredResultCount = commentsAndQuestions.Questions.Count(q => q.Show); 
			questionOption.UnfilteredResultCount = commentsAndQuestions.Questions.Count();

			//comments
			commentsOption.IsSelected = type != null && type.Contains(QuestionsOrComments.Comments);
			commentsOption.FilteredResultCount = commentsAndQuestions.Comments.Count(q => q.Show); 
			commentsOption.UnfilteredResultCount = commentsAndQuestions.Comments.Count();

			//populate commenters
			if (_currentUser.IsAuthenticatedByAccounts && _currentUser.OrganisationsAssignedAsLead.Any())
			{
				var commenters = _context.GetEmailAddressForCommentsAndAnswers(commentsAndQuestions).ToList();
				commentersFilter.Options = new List<FilterOption>(commenters.Count());

				foreach (var commenter in commenters)
				{
					var isSelected = commentersToFilter != null && commentersToFilter.Contains(commenter);
					commentersFilter.Options.Add(
						new FilterOption(commenter, commenter, isSelected)
						{
							FilteredResultCount = commentsAndQuestions.Comments.Count(c => c.Show) + commentsAndQuestions.Questions.Count(q => q.Show),
							UnfilteredResultCount = commentsAndQuestions.Comments.Count() + commentsAndQuestions.Questions.Count()
                        }
					);
                }
			}
			else
			{
				commentersFilter.Options = new List<FilterOption>(0);
			}

			//populate documents
			var documents = (await _consultationService.GetDocuments(consultationId)).documents.Where(d => d.ConvertedDocument).ToList();
		    documentsFilter.Options = new List<FilterOption>(documents.Count());

			foreach (var document in documents)
			{
				var isSelected = documentIdsToFilter != null && documentIdsToFilter.Contains(document.DocumentId);
				documentsFilter.Options.Add(
					new FilterOption(document.DocumentId.ToString(), document.Title, isSelected)
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
	    public async Task<string> UpdateOrderWithSourceURI(string orderWithinChapter, string sourceURI)
	    {
		    var consultationsUriElements = ConsultationsUri.ParseConsultationsUri(sourceURI);

			var consultationId = consultationsUriElements.ConsultationId;
			var documentId = consultationsUriElements.DocumentId;
		    int? chapterIndex = null;
		    
			if (consultationsUriElements.IsChapterLevel())
			{
				var document = (await _consultationService.GetDocuments(consultationsUriElements.ConsultationId)).documents.FirstOrDefault(d => d.DocumentId.Equals(documentId));
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
