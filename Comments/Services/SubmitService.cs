using System;
using System.Collections.Generic;
using Comments.Common;
using Comments.Migrations;
using Comments.Models;
using Comments.ViewModels;
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
		    if (!_currentUser.IsAuthorised)
			    return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in submitting comments and answers"));

		    var submissionToSave = new Models.Submission(_currentUser.UserId.Value, DateTime.UtcNow);
		    _context.Submission.Add(submissionToSave);

		    var submittedStatus = _context.GetStatus(StatusName.Submitted);

			foreach (var comment in commentsAndAnswers.Comments)
			{
				comment.StatusId = StatusName.Submitted;
				comment.Status = new ViewModels.Status(submittedStatus);

				_context.GetComment(comment.CommentId).UpdateFromViewModel(comment);
				
				_context.SubmissionComment.Add(new Models.SubmissionComment(submissionToSave.SubmissionId, comment.CommentId));
			}

			foreach (var answer in commentsAndAnswers.Answers)
		    {
			    answer.StatusId = StatusName.Submitted;
				answer.Status = new ViewModels.Status(submittedStatus);

				_context.GetAnswer(answer.AnswerId).UpdateFromViewModel(answer);
				
			    _context.SubmissionAnswer.Add(new Models.SubmissionAnswer(submissionToSave.SubmissionId, answer.AnswerId));
			}
			
		return (rowsUpdated: _context.SaveChanges(), validate: null);
		}
	}
}
