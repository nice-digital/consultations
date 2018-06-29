using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Comments.Services;
using Comments.ViewModels;
using Microsoft.Extensions.Configuration;

namespace Comments.Models
{
    public partial class ConsultationsContext : DbContext
    {
	    private readonly IConfiguration _configuration;

		//these commented out constructors are just here for use when creating scaffolding with EF core. without them it won't work.
		//don't leave them in uncommented though. and don't set that connection string to a valid value and commit it.
		//public ConsultationsContext(DbContextOptions options)
		//	: base(options) {
		//    _createdByUserID = Guid.Empty;
		//}
	 //   public ConsultationsContext() : base()
	 //   {
		//    _createdByUserID = Guid.Empty;
	 //   }
	 //   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	 //   {
		//	optionsBuilder.UseSqlServer("[snip]");
		//}


		public ConsultationsContext(DbContextOptions options, IUserService userService) : base(options)
        {
	        _userService = userService;
            _createdByUserID = _userService.GetCurrentUser().UserId;
        }

		/// <summary>
		/// It's not obvious from this code, but this it actually filtering on more than it looks like. There's global filters defined in the context, specifically
		/// for the IsDeleted flag and the CreatedByUserId. So, this is only going to return data that isn't deleted and belongs to the current user.
		/// This behaviour can be overridden with the IgnoreQueryFilters command. See the ConsultationContext.Tests for example usage.
		/// </summary>
		/// <param name="sourceURIs"></param>
		/// <param name="isReview">True if data is being retrieved for the review page</param>
		/// <returns></returns>
		public IEnumerable<Location> GetAllCommentsAndQuestionsForDocument(IEnumerable<string> sourceURIs, bool isReview)
	    {
			if (!_userService.GetCurrentUser().IsAuthorised)
			    throw new Exception("trying to return comments and questions when not logged in. this should have been trapped in the service.");

			var data = Location.Where(l => isReview ? l.SourceURI.Contains(sourceURIs.First()) : sourceURIs.Contains(l.SourceURI))
					.Include(l => l.Comment)
						.ThenInclude(s => s.SubmissionComment)
							.ThenInclude(s => s.Submission)

					.Include(l => l.Comment)
						.ThenInclude(s => s.Status)

					.Include(l => l.Question)
						.ThenInclude(q => q.QuestionType)
				    .Include(l => l.Question)
						.ThenInclude(q => q.Answer)
							.ThenInclude(s => s.SubmissionAnswer)
					.OrderByDescending(l => l.Comment
					    .OrderByDescending(c => c.LastModifiedDate).Select(c => c.LastModifiedDate).FirstOrDefault());

			return data;
	    }
		
        public Comment GetComment(int commentId)
        {
            var comment = Comment.Where(c => c.CommentId.Equals(commentId))
                            .Include(c => c.Location)
							.Include(s => s.Status)
                            .FirstOrDefault();

            return comment;
        }

        public Answer GetAnswer(int answerId)
        {
	        var answer = Answer.Where(a => a.AnswerId.Equals(answerId))
		        .Include(s => s.Status)
		        .FirstOrDefault();

	        return answer;
        }

        public Question GetQuestion(int questionId)
        {
            return Question.Where(q => q.QuestionId.Equals(questionId))
                .Include(q => q.Location)
                .Include(q => q.QuestionType)
                .FirstOrDefault();
        }

	    public Status GetStatus(StatusName statusName)
	    {
		    return Status
			    .Single(s => s.Name.Equals(statusName.ToString(), StringComparison.OrdinalIgnoreCase));
	    }

	    public void UpdateCommentStatus(IEnumerable<int> commentIds, Status status)
	    {
		    var commentsToUpdate = Comment.Where(c => commentIds.Contains(c.CommentId)).ToList();

		    if (commentsToUpdate.Any(c => c.CreatedByUserId != _createdByUserID))
			    throw new Exception("Attempt to update status of a comment which doesn't belong to the current user");

		    commentsToUpdate.ForEach(c => c.StatusId = status.StatusId);
		}

	    public void AddSubmissionComments(IEnumerable<int> commentIds, int submissionId)
	    {
			//the extra DB hit here is to ensure that duplicate rows aren't inserted. currently, you should only be able to submit a comment once. in the future though that might change as resubmitting is on the cards, and the DB supports that now.
		    var existingSubmissionCommentIdsForPassedInComments = SubmissionComment.Where(sc => commentIds.Contains(sc.CommentId)).Select(sc => sc.CommentId).ToList();

		    var submissionCommentsToInsert = commentIds.Where(commentId =>
				    !existingSubmissionCommentIdsForPassedInComments.Contains(commentId))
			    .Select(commentId => new Models.SubmissionComment(submissionId, commentId)).ToList();

			SubmissionComment.AddRange(submissionCommentsToInsert);
		}
		public void UpdateAnswerStatus(IEnumerable<int> answerIds, Status status)
		{
			var answersToUpdate = Answer.Where(a => answerIds.Contains(a.AnswerId)).ToList();

			if (answersToUpdate.Any(a => a.CreatedByUserId != _createdByUserID))
				throw new Exception("Attempt to update status of an answer which doesn't belong to the current user");

			answersToUpdate.ForEach(c => c.StatusId = status.StatusId);
		}

	    public void AddSubmissionAnswers(IEnumerable<int> answerIds, int submissionId)
	    {
		    //the extra DB hit here is to ensure that duplicate rows aren't inserted. currently, you should only be able to submit a comment once. in the future though that might change as resubmitting is on the cards, and the DB supports that now.
		    var existingSubmissionAnswerIdsForPassedInAnswers = SubmissionAnswer.Where(sa => answerIds.Contains(sa.AnswerId)).Select(sa => sa.AnswerId).ToList();

		    var submissionAnswersToInsert = answerIds.Where(commentId =>
				    !existingSubmissionAnswerIdsForPassedInAnswers.Contains(commentId))
			    .Select(commentId => new Models.SubmissionAnswer(submissionId, commentId)).ToList();

		    SubmissionAnswer.AddRange(submissionAnswersToInsert);
	    }

	}
}
