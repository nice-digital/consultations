using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Common;
using Comments.Migrations;
using Comments.Models;
using Comments.ViewModels;
using Microsoft.EntityFrameworkCore;
using NICE.Auth.NetCore.Services;
using Answer = Comments.ViewModels.Answer;
using Comment = Comments.ViewModels.Comment;

namespace Comments.Services
{
	public interface ISubmitService
	{
		(int rowsUpdated, Validate validate) SubmitCommentsAndAnswers(CommentsAndAnswers commentsAndAnswers);
	}
	public class SubmitService : ISubmitService
    {
	    private readonly ConsultationsContext _context;
	    private readonly IUserService _userService;
	    private readonly IAuthenticateService _authenticateService;
	    private readonly User _currentUser;

	    public SubmitService(ConsultationsContext context, IUserService userService, IAuthenticateService authenticateService)
	    {
		    _context = context;
		    _userService = userService;
		    _authenticateService = authenticateService;
		    _currentUser = _userService.GetCurrentUser();
	    }

	    public (int rowsUpdated, Validate validate) SubmitCommentsAndAnswers(CommentsAndAnswers commentsAndAnswers)
	    {
		    if (!_currentUser.IsAuthorised || !_currentUser.UserId.HasValue)
			    return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in submitting comments and answers"));

		    //if a user is submitting a different users comment, the context will throw an exception.

			var submissionToSave = new Models.Submission(_currentUser.UserId.Value, DateTime.UtcNow);
		    _context.Submission.Add(submissionToSave);

		    var submittedStatus = _context.GetStatus(StatusName.Submitted);
			
			UpdateCommentsModel(commentsAndAnswers.Comments, submissionToSave, submittedStatus);
		    UpdateAnswersModel(commentsAndAnswers.Answers, submissionToSave, submittedStatus);

			return (rowsUpdated: _context.SaveChanges(), validate: null);
		}

	    public void UpdateCommentsModel(IList<Comment> comments, Submission submission, Models.Status status)
	    {
			var commentIds = comments.Select(c => c.CommentId).ToList();
			_context.UpdateCommentStatus(commentIds, status);

		    foreach (var commentInViewModel in comments)
		    {
			    commentInViewModel.UpdateStatusFromDBModel(status);
		    }

			_context.AddSubmissionComments(commentIds, submission.SubmissionId);
		}

		public void UpdateAnswersModel(IList<Answer> answers, Submission submission, Models.Status status)
	    {
			var answerIds = answers.Select(a => a.AnswerId).ToList();
			_context.UpdateAnswerStatus(answerIds, status);

		    foreach (var answerInViewModel in answers)
		    {
			    answerInViewModel.UpdateStatusFromDBModel(status);
		    }

		    _context.AddSubmissionAnswers(answerIds, submission.SubmissionId);
		}
	}
}
