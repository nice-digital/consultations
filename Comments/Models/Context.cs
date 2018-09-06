using Comments.Services;
using Comments.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Remotion.Linq.Clauses;
using SQLitePCL;

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
		//	optionsBuilder.UseSqlServer("[you don't need a valid connection string when creating migrations. the real connection string should never be put here though. it should be kept in secrets.json]");
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
		/// <param name="partialMatchSourceURI">True if data is being retrieved for the review page</param>
		/// <param name="getSubmitted">True if data is being retrieved should only be submitted data</param>
		/// <returns></returns>
		public IEnumerable<Location> GetAllCommentsAndQuestionsForDocument(IList<string> sourceURIs, bool partialMatchSourceURI, bool getSubmitted = false)
		{
			string partialSourceURIToUse = null, partialMatchExactSourceURIToUse = null;
		    if (partialMatchSourceURI)
		    {
			    partialMatchExactSourceURIToUse = sourceURIs.SingleOrDefault();
				if (partialMatchExactSourceURIToUse == null)
					throw new ArgumentException("There should be one and only one source uri passed when doing a partial match.");

			    partialSourceURIToUse = $"{partialMatchExactSourceURIToUse}/";
		    }

			//Answer.Where(a => a.Status = 1 && a.Question.Location.SourceURI)

			var data = Location.Where(l => partialMatchSourceURI
					? (l.SourceURI.Equals(partialMatchExactSourceURIToUse) || l.SourceURI.Contains(partialSourceURIToUse))
					: sourceURIs.Contains(l.SourceURI))
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

					.OrderBy(l => l.Order)

				.ThenByDescending(l =>
					l.Comment.OrderByDescending(c => c.LastModifiedDate).Select(c => c.LastModifiedDate).FirstOrDefault())

				.ToList();

			if (getSubmitted)
			{
				var filteredData = data.Where(l =>
												(l.Comment != null && l.Comment.Count != 0 ? l.Comment.Any(c => c.StatusId == (int)StatusName.Submitted) : false)
												||
												(l.Question != null && l.Question.Count != 0 ? l.Question.Any(q => q.Answer.Count() == 0) : false)
												||
												(l.Question != null && l.Question.Count != 0 ? l.Question.FirstOrDefault().Answer.Any(c => c.StatusId == (int)StatusName.Submitted) : false)
											);

				return filteredData;
			}

			return data;
			
			
		}

	    public List<Comment> GetAllSubmittedCommentsForURI(string  sourceURI)
	    {
		    var comment = Comment.Where(c => c.StatusId == (int)StatusName.Submitted && c.Location.SourceURI.Contains(sourceURI) && c.IsDeleted == false)
			    .Include(l => l.Location)
			    .Include(s => s.Status)
				.Include(sc => sc.SubmissionComment)
				.ThenInclude(s => s.Submission)
			    .IgnoreQueryFilters()
				.ToList();

		    return comment;
		}

	    public List<Answer> GetAllSubmittedAnswersForURI(string sourceURI)
	    {
		    var answer = Answer.Where(a => a.StatusId == (int) StatusName.Submitted && a.Question.Location.SourceURI.Contains(sourceURI) && a.IsDeleted == false)
				.Include(q => q.Question)
				.ThenInclude(l => l.Location)
			    .Include(sc => sc.SubmissionAnswer)
			    .ThenInclude(s => s.Submission)
				.IgnoreQueryFilters()
			    .ToList();

			return answer;
	    }
	    public List<Question> GetUnansweredQuestionsForURI(string sourceURI)
	    {
		    var question = Question.Where(q => q.Answer.Count == 0 && q.Location.SourceURI.Contains(sourceURI) && q.IsDeleted == false)
				.Include(l => l.Location)
			    .IgnoreQueryFilters()
				.ToList();

		    return question;
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

	    public Submission InsertSubmission(Guid currentUser, bool respondingAsOrganisation, string organisationName, bool hasTobaccoLinks, string tobaccoDisclosure)
	    {
		    var submission = new Models.Submission(currentUser, DateTime.UtcNow, respondingAsOrganisation, organisationName, hasTobaccoLinks, tobaccoDisclosure);
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

		/// <summary>
		/// this question insert script is temporary, until the question administration features are built.
		/// </summary>
		/// <param name="consultationId"></param>
		/// <returns></returns>
		public int InsertQuestionsWithScript(int consultationId)
	    {
		    return Database.ExecuteSqlCommand(@"
				--DECLARE @consultationId AS int --UNCOMMENT OUT THESE 2 LINES TO USE IN SQL MANAGEMENT STUDIO
				--SET @consultationId = 11

				DECLARE @questionTypeID AS int
				DECLARE @locationID1 AS int, @locationID2 AS int, @locationID3 AS int

				DECLARE @userID as uniqueidentifier
				SELECT @userID = cast(cast(0 AS binary) AS uniqueidentifier)

				DECLARE @questionTextDescription nvarchar(100)
				SET @questionTextDescription = 'A text question requiring a text answer.'

				DECLARE @questionOneText nvarchar(MAX)
				SET @questionOneText = 'Which areas will have the biggest impact on practice and be challenging to implement? Please say for whom and why.'

				DECLARE @consultationIdPaddedForOrder nvarchar(3)
				SELECT @consultationIdPaddedForOrder = RIGHT('000'+ CAST(@consultationId AS VARCHAR(3)),3)

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

					INSERT INTO [Location] (SourceURI, [Order])
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar), @consultationIdPaddedForOrder + '.000.000.000')

					SET @locationID1 = SCOPE_IDENTITY();

					INSERT INTO [Location] (SourceURI, [Order])
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar) + '/document/1', @consultationIdPaddedForOrder + '.001.000.000')

					SET @locationID2 = SCOPE_IDENTITY();

					INSERT INTO [Location] (SourceURI, [Order])
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar) + '/document/2', @consultationIdPaddedForOrder + '.002.000.000')

					SET @locationID3 = SCOPE_IDENTITY();

					--now the question inserts

					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID1, @questionOneText, @questionTypeID, @userID, @userID, GETDATE())


					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID2, 'Would implementation of any of the draft recommendations have significant cost implications?', @questionTypeID, @userID, @userID, GETDATE())


					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID3, 'Would implementation of any of the draft recommendations have cost implications?', @questionTypeID, @userID, @userID, GETDATE())			
		
				END
			", new SqlParameter("@consultationId", consultationId));
	    }

		/// <summary>
		/// Deletes everything except for the Status table.
		/// </summary>
		/// <returns></returns>
		public int DeleteEverything()
		{
			return Database.ExecuteSqlCommand(@"
				DELETE FROM SubmissionComment;
				DELETE FROM SubmissionAnswer;
				DELETE FROM Submission;
				DELETE FROM Answer;
				DELETE FROM Question;
				DELETE FROM Comment;
				DELETE FROM [Location];
				DELETE FROM QuestionType;
			");
		}
	}
}
