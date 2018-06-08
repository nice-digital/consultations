using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Models;
using Comments.ViewModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NICE.Auth.NetCore.Services;

namespace Comments.Services
{
    public class SubmitService
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
			
			foreach (var comment in commentsAndAnswers.Comments)
		    {
			    comment.StatusId = 2;
			    comment.LastModifiedByUserId = _currentUser.UserId.Value;
			    comment.LastModifiedDate = DateTime.UtcNow;

			    _context.GetComment(comment.CommentId).UpdateFromViewModel(comment);
			}

		    foreach (var answer in commentsAndAnswers.Answers)
		    {
			    answer.StatusId = 2;
			    answer.LastModifiedByUserId = _currentUser.UserId.Value;
			    answer.LastModifiedDate = DateTime.UtcNow;

				_context.GetAnswer(answer.AnswerId).UpdateFromViewModel(answer);
			}

		//TODO: do I need to update context???
		 //   var commentInDatabase = _context.GetComment(commentId);


			//commentsAndAnswers.Comments.UpdateFromViewModel(comment);
			return (rowsUpdated: _context.SaveChanges(), validate: null);
		}
    }
}
