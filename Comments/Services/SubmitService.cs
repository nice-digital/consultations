using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Common;
using Comments.Migrations;
using Comments.Models;
using Comments.ViewModels;
using Microsoft.EntityFrameworkCore;
using NICE.Auth.NetCore.Services;

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

		    //TODO: check if user is submitting own comment, would mean hitting DB to get CreatedByUserId?

			var submissionToSave = new Models.Submission(_currentUser.UserId.Value, DateTime.UtcNow);
		    _context.Submission.Add(submissionToSave);

		    var submittedStatus = _context.GetStatus((int)StatusName.Submitted);
			
			UpdateCommentsModel(commentsAndAnswers, submissionToSave, submittedStatus);
		    UpdateAnswersModel(commentsAndAnswers, submissionToSave, submittedStatus);

			return (rowsUpdated: _context.SaveChanges(), validate: null);
		}

	    public void UpdateCommentsModel(CommentsAndAnswers commentsAndAnswers, Submission submission, Models.Status status)
	    {
		    foreach (var comment in commentsAndAnswers.Comments)
		    {
			    comment.StatusId = (int)StatusName.Submitted;
			    comment.Status = new ViewModels.Status(status);
		    }

		    var commentIds = commentsAndAnswers.Comments.Select(c => c.CommentId);
			var dbComments = _context.Comment.Where(c => commentIds.Contains(c.CommentId));

			foreach (var comment in dbComments)
		    {
			    var commentViewModel = commentsAndAnswers.Comments.Single(c => c.CommentId == comment.CommentId);
			    comment.UpdateFromViewModel(commentViewModel);

			    comment.Status = status;

			    _context.SubmissionComment.Add(new Models.SubmissionComment(submission.SubmissionId, comment.CommentId));
		    }
		}

	    public void UpdateAnswersModel(CommentsAndAnswers commentsAndAnswers, Submission submission, Models.Status status)
	    {
		    foreach (var answer in commentsAndAnswers.Answers)
		    {
			    answer.StatusId = (int)StatusName.Submitted;
			    answer.Status = new ViewModels.Status(status);
		    }

			var answerIds = commentsAndAnswers.Answers.Select(a => a.AnswerId);
			var answerDbModel = _context.Answer.Where(a => answerIds.Contains(a.AnswerId));

			foreach (var answer in answerDbModel)
			{
				var answerViewModel = commentsAndAnswers.Answers.Single(c => c.AnswerId == answer.AnswerId);
				answer.UpdateFromViewModel(answerViewModel);

				answer.Status = status;

				_context.SubmissionAnswer.Add(new Models.SubmissionAnswer(submission.SubmissionId, answer.AnswerId));
			}
		}
	}
}
