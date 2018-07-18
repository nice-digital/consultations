using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Comments.Services;
using Comments.ViewModels;
using Microsoft.Extensions.Configuration;

namespace Comments.Models
{
    public partial class ConsultationsContext : DbContext
    {
	    private readonly IEncryption _encryption;
	    private readonly IConfiguration _configuration;

		//these commented out constructors are just here for use when creating scaffolding with EF core. without them it won't work.
		//don't leave them in uncommented though. and don't set that connection string to a valid value and commit it.
		//public ConsultationsContext(DbContextOptions options)
		//	: base(options)
		//{
		//	_createdByUserID = Guid.Empty;
		//}
		//public ConsultationsContext() : base()
		//{
		//	_createdByUserID = Guid.Empty;
		//}
		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	optionsBuilder.UseSqlServer("[snip]");
		//}


		public ConsultationsContext(DbContextOptions options, IUserService userService, IEncryption encryption) : base(options)
        {
	        _encryption = encryption;
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

	    public Submission InsertSubmission(Guid currentUser)
	    {
		    var submission = new Models.Submission(currentUser, DateTime.UtcNow);
		    Submission.Add(submission);
		    return submission;
	    }

	    public bool HasSubmitted(string consultationSourceURI, Guid currentUser)
	    {
		    var submissions = Submission.Where(s => s.SubmissionByUserId.Equals(currentUser))
			    .Include(s => s.SubmissionComment)
					.ThenInclude(sc => sc.Comment)
						.ThenInclude(c => c.Location)

			    .Include(s => s.SubmissionAnswer)
					.ThenInclude(sa => sa.Answer)
						.ThenInclude(a => a.Question)
							.ThenInclude(q => q.Location)

				.ToList();


		    var allQuestionSourceUrisForThisUser = submissions.SelectMany(s => s.SubmissionAnswer,
			    ((submission, answer) => answer.Answer.Question.Location.SourceURI)).ToList();

		    var allCommentSourceUrisForThisUser = submissions.SelectMany(s => s.SubmissionComment,
			    ((submission, comment) => comment.Comment.Location.SourceURI)).ToList();

		    var allSourceUris = allQuestionSourceUrisForThisUser.Concat(allCommentSourceUrisForThisUser).ToList();

		    return allSourceUris.Any(sourceURI => sourceURI.StartsWith(consultationSourceURI, StringComparison.OrdinalIgnoreCase));

	    }

	    public int DeleteAllSubmissionsFromUser(Guid usersSubmissionsToDelete)
	    {
		    var draftStatus = GetStatus(StatusName.Draft);

			var submissions = Submission.Where(s => s.SubmissionByUserId.Equals(usersSubmissionsToDelete))
			    .Include(s => s.SubmissionComment)
					.ThenInclude(sc => sc.Comment) 
				.Include(s => s.SubmissionAnswer)
					.ThenInclude(sa => sa.Answer)
				.IgnoreQueryFilters() //without this you'd only be able to delete your own data..
				.ToList();
			
		    var submissionCommentsToDelete = new List<SubmissionComment>();
		    var submissionAnswersToDelete = new List<SubmissionAnswer>();
		    foreach (var submission in submissions)
		    {
			    foreach (var submissionComment in submission.SubmissionComment)
			    {
					submissionComment.Comment.StatusId = draftStatus.StatusId;
				    submissionCommentsToDelete.Add(submissionComment);
				}

			    foreach (var submissionAnswer in submission.SubmissionAnswer)
			    {
					submissionAnswer.Answer.StatusId = draftStatus.StatusId;
				    submissionAnswersToDelete.Add(submissionAnswer);
				}

				SubmissionComment.RemoveRange(submissionCommentsToDelete);
			    SubmissionAnswer.RemoveRange(submissionAnswersToDelete);
			    Submission.Remove(submission);
		    }
		    return SaveChanges();
	    }

	    public int InsertQuestionsWithScript(int consultationId)
	    {
		    return Database.ExecuteSqlCommand(@"
				--DECLARE @consultationId AS int
				--SET @consultationId = 1

				DECLARE @questionTypeID AS int
				DECLARE @locationID1 AS int, @locationID2 AS int, @locationID3 AS int

				DECLARE @userID as uniqueidentifier
				SELECT @userID = cast(cast(0 AS binary) AS uniqueidentifier)

				DECLARE @questionTextDescription nvarchar(100)
				SET @questionTextDescription = 'A text question requiring a text answer.'

				DECLARE @questionOneText nvarchar(MAX)
				SET @questionOneText = 'Which areas will have the biggest impact on practice and be challenging to implement? Please say for whom and why.'

				--question type insert
				SELECT @questionTypeID = QuestionTypeID
				FROM QuestionType
				WHERE [Description] = @questionTextDescription

				IF @questionTypeID IS NULL 
				BEGIN
					INSERT INTO QuestionType ([Description], HasBooleanAnswer, HasTextAnswer)
					VALUES (@questionTextDescription, 0, 1)

					SET @questionTypeID = SCOPE_IDENTITY();
				END

				--3 location inserts
				IF NOT EXISTS (SELECT * FROM [Location] L
								INNER JOIN Question Q ON Q.LocationID = L.LocationID
								WHERE L.SourceURI = 'consultations://./consultation/' + CAST(@consultationId AS varchar) AND
								Q.QuestionText = @questionOneText)
				BEGIN

					INSERT INTO Location (SourceURI)
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar))

					SET @locationID1 = SCOPE_IDENTITY();

					INSERT INTO Location (SourceURI)
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar) + '/document/1')

					SET @locationID2 = SCOPE_IDENTITY();

					INSERT INTO Location (SourceURI)
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar) + '/document/2')

					SET @locationID3 = SCOPE_IDENTITY();

					--now the question inserts

					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, QuestionOrder, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID1, @questionOneText, @questionTypeID, 1, @userID, @userID, GETDATE())


					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, QuestionOrder, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID2, 'Would implementation of any of the draft recommendations have significant cost implications?', @questionTypeID, 2, @userID, @userID, GETDATE())


					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, QuestionOrder, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID3, 'Would implementation of any of the draft recommendations have cost implications?', @questionTypeID, 3, @userID, @userID, GETDATE())			
		
				END
			", new SqlParameter("@consultationId", consultationId));
	    }
	}
}
