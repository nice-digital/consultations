using System;
using Comments.Models;
using Comments.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Comments.Common;

namespace Comments.Services
{
	public interface ISubmitService
	{
		(int rowsUpdated, Validate validate) SubmitCommentsAndAnswers(CommentsAndAnswers commentsAndAnswers);

		int DeleteAllSubmissionsFromUser(Guid usersSubmissionsToDelete);
	}

	public class SubmitService : ISubmitService
	{
		private readonly ConsultationsContext _context;
		private readonly IConsultationService _consultationService;
		private readonly User _currentUser;

		public SubmitService(ConsultationsContext context, IUserService userService, IConsultationService consultationService)
		{
			_context = context;
			_consultationService = consultationService;
			_currentUser = userService.GetCurrentUser();
		}

		public (int rowsUpdated, Validate validate) SubmitCommentsAndAnswers(CommentsAndAnswers commentsAndAnswers)
		{
			if (!_currentUser.IsAuthorised || !_currentUser.UserId.HasValue)
				return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in submitting comments and answers"));

			//if a user is submitting a different users comment, the context will throw an exception.

			var anySourceURI = commentsAndAnswers.SourceURIs.FirstOrDefault();
			if (anySourceURI == null)
				return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: false, message: "Could not find SourceURI"));

			var consultationState = _consultationService.GetConsultationState(anySourceURI);

			if (!consultationState.ConsultationIsOpen)
				return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: false, message: "Consultation is not open for submissions"));

			var hasSubmitted = _consultationService.HasSubmittedCommentsOrQuestions(anySourceURI, _currentUser.UserId.Value);
			if (hasSubmitted)
				return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: false, message: "User has already submitted."));

			var submissionToSave = _context.InsertSubmission(_currentUser.UserId.Value);

			var submittedStatus = _context.GetStatus(StatusName.Submitted);

			UpdateCommentsModel(commentsAndAnswers.Comments, submissionToSave, submittedStatus);
			UpdateAnswersModel(commentsAndAnswers.Answers, submissionToSave, submittedStatus);

			return (rowsUpdated: _context.SaveChanges(), validate: null);
		}

		private void UpdateCommentsModel(IList<ViewModels.Comment> comments, Submission submission, Models.Status status)
		{
			var commentIds = comments.Select(c => c.CommentId).ToList();
			_context.UpdateCommentStatus(commentIds, status);

			foreach (var commentInViewModel in comments)
			{
				commentInViewModel.UpdateStatusFromDBModel(status);
			}
			_context.AddSubmissionComments(commentIds, submission.SubmissionId);
		}

		private void UpdateAnswersModel(IList<ViewModels.Answer> answers, Submission submission, Models.Status status)
		{
			var answerIds = answers.Select(a => a.AnswerId).ToList();
			_context.UpdateAnswerStatus(answerIds, status);

			foreach (var answerInViewModel in answers)
			{
				answerInViewModel.UpdateStatusFromDBModel(status);
			}
			_context.AddSubmissionAnswers(answerIds, submission.SubmissionId);
		}

		
		public int DeleteAllSubmissionsFromUser(Guid usersSubmissionsToDelete)
		{
			//todo: some kind of service authorisation
			return _context.DeleteAllSubmissionsFromUser(usersSubmissionsToDelete);
		}
	}
}
