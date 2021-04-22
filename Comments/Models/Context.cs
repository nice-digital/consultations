using Comments.Services;
using Comments.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Comments.Common;
using Microsoft.EntityFrameworkCore.Internal;
using Z.EntityFramework.Plus;

namespace Comments.Models
{
	public partial class ConsultationsContext : DbContext
    {
	    private readonly IEncryption _encryption;

		//these commented out constructors are just here for use when creating scaffolding with EF core. without them it won't work.
		//don't leave them in uncommented though. and don't set that connection string to a valid value and commit it.
		//public ConsultationsContext(DbContextOptions options)
		//	: base(options)
		//{
		//	_createdByUserID = Guid.Empty.ToString();
		//}
		//public ConsultationsContext() : base()
		//{
		//	_createdByUserID = Guid.Empty.ToString();
		//}
		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	optionsBuilder.UseSqlServer("[you don't need a valid connection string when creating migrations. the real connection string should never be put here though. it should be kept in secrets.json]");
		//}

		public ConsultationsContext(DbContextOptions options, IUserService userService, IEncryption encryption) : base(options)
		{
			_encryption = encryption;
			_userService = userService;
			ConfigureContext();
		}

		public void ConfigureContext()
		{
			var currentUserInThisScope = _userService.GetCurrentUser(); //this dbcontext's service lifetime is scoped, i.e. new for every request.
			_createdByUserID = currentUserInThisScope.UserId;
			_organisationUserIDs = currentUserInThisScope.ValidatedOrganisationUserIds;
			_organisationIDs = currentUserInThisScope.ValidatedOrganisationIds;
			var currentUsersOrganisationTheyAreLeadOf = currentUserInThisScope.OrganisationsAssignedAsLead?.FirstOrDefault(); //the new plan is to only support being lead of 1 organisation.
			_organisationalLeadOrganisationID = currentUsersOrganisationTheyAreLeadOf != null ? (int?)currentUsersOrganisationTheyAreLeadOf.OrganisationId : null;
		}

		/// <summary>
		/// It's not obvious from this code, but this it actually filtering on more than it looks like. There's global filters defined in the context, specifically
		/// for the IsDeleted flag and the CreatedByUserId. So, this is only going to return data that isn't deleted and belongs to the current user.
		/// This behaviour can be overridden with the IgnoreQueryFilters command. See the ConsultationContext.Tests for example usage.
		/// </summary>
		/// <param name="sourceURIs"></param>
		/// <param name="partialMatchSourceURI">True if data is being retrieved for the review page</param>
		/// <returns></returns>
		public IEnumerable<Location> GetAllCommentsAndQuestionsForDocument(IList<string> sourceURIs, bool partialMatchSourceURI)
		{
			string partialSourceURIToUse = null, partialMatchExactSourceURIToUse = null;
		    if (partialMatchSourceURI)
		    {
			    partialMatchExactSourceURIToUse = sourceURIs.SingleOrDefault();
				if (partialMatchExactSourceURIToUse == null)
					throw new ArgumentException("There should be one and only one source uri passed when doing a partial match.");

			    partialSourceURIToUse = $"{partialMatchExactSourceURIToUse}/";
		    }

			var authorisedComments = Comment
				.Include(c => c.Location)
				.Where(c => (partialMatchSourceURI
					? (c.Location.SourceURI.Equals(partialMatchExactSourceURIToUse) || c.Location.SourceURI.Contains(partialSourceURIToUse))
					: sourceURIs.Contains(c.Location.SourceURI, StringComparer.OrdinalIgnoreCase)))
				.ToList();

			var data = Location.Where(l => (l.Order != null) &&

			                               (partialMatchSourceURI
				                               		? (l.SourceURI.Equals(partialMatchExactSourceURIToUse) || l.SourceURI.Contains(partialSourceURIToUse))
				                               		: sourceURIs.Contains(l.SourceURI, StringComparer.OrdinalIgnoreCase)))
				//(allAuthorisedLocations.Contains(l.LocationId)))

				.Include(l => l.Comment)
					.ThenInclude(s => s.SubmissionComment)
						.ThenInclude(s => s.Submission)

				.Include(l => l.Comment)
					.ThenInclude(s => s.Status)

				.Include(l => l.Comment)
					.ThenInclude(o => o.OrganisationUser)

				.Include(l => l.Question)
					.ThenInclude(q => q.QuestionType)

				.Include(l => l.Question)
					.ThenInclude(q => q.Answer)
						.ThenInclude(s => s.SubmissionAnswer)

				.Include(l => l.Question)
					.ThenInclude(q => q.Answer)
						.ThenInclude(o => o.OrganisationUser)

				.OrderBy(l => l.Order)
					//.ThenByDescending(l => l.Comment.OrderByDescending(c => c.LastModifiedDate).Select(c => c.LastModifiedDate).FirstOrDefault())

				.ToList();


			//EF can't translate the thenbydescending properly, so moving it out and doing it in memory.
			var sortedData = data.Where(l => l.Comment.Count > 0 || l.Question.Count > 0)
				.OrderBy(l => l.Order).ThenByDescending(l =>
					l.Comment.OrderByDescending(c => c.LastModifiedDate).Select(c => c.LastModifiedDate).FirstOrDefault());

			return sortedData;
		}

		/// <summary>
		/// Organisation users view each others submitted responses to inform their own, but can't interact with or submit them.
		/// </summary>
		/// <param name="sourceURIs"></param>
        /// <returns></returns>
		public IEnumerable<Location> GetOtherOrganisationUsersCommentsAndQuestionsForDocument(IList<string> sourceURIs)
		{
            var commentsAndAnswers = Location.IgnoreQueryFilters()
                .IncludeFilter(l => l.Comment.Where(c => c.StatusId == (int)StatusName.SubmittedToLead
                                                         && _organisationIDs.Any(o => o.Equals(c.OrganisationId))
                                                         && !_organisationUserIDs.Contains(c.OrganisationUserId.Value)))
                .IncludeFilter(l => l.Comment.Where(c => c.StatusId == (int)StatusName.SubmittedToLead 
                                                         && _organisationIDs.Any(o => o.Equals(c.OrganisationId)) 
                                                         && !_organisationUserIDs.Contains(c.OrganisationUserId.Value))
                    .Select(c=> c.Status))
                .IncludeFilter(l => l.Comment.Where(c => c.StatusId == (int)StatusName.SubmittedToLead
                                                         && _organisationIDs.Any(o => o.Equals(c.OrganisationId))
                                                         && !_organisationUserIDs.Contains(c.OrganisationUserId.Value))
                    .Select(c=> c.OrganisationUser))

                .IncludeFilter(l => l.Question)
                .IncludeFilter(l => l.Question
                    .Select(q => q.QuestionType))
                .IncludeFilter(l => l.Question
                    .Select(q => q.Answer.Where(a => a.StatusId == (int)StatusName.SubmittedToLead
                                                     && _organisationIDs.Contains(a.OrganisationId.Value) 
                                                     && !_organisationUserIDs.Contains(a.OrganisationUserId.Value))
                        .OrderByDescending(a=> a.LastModifiedDate).Select(a => a.LastModifiedDate).FirstOrDefault()))
                .IncludeFilter(l=> l.Question
                    .Select(q => q.Answer.Where(a => a.StatusId == (int)StatusName.SubmittedToLead
                                                     && _organisationIDs.Contains(a.OrganisationId.Value)
                                                     && !_organisationUserIDs.Contains(a.OrganisationUserId.Value))
                    .Select(a => a.OrganisationUser)))
                .OrderBy(l => l.Order)
                .ToList();

            var filteredLocations = commentsAndAnswers.Where(l =>
                (l.Order != null) && sourceURIs.Contains(l.SourceURI, StringComparer.OrdinalIgnoreCase));

            var sortedData = filteredLocations.Where(l => l.Comment.Count > 0 || l.Question.Count > 0)
                .OrderBy(l => l.Order)
                .ThenByDescending(l =>
                    l.Comment.OrderByDescending(c => c.LastModifiedDate).Select(c => c.LastModifiedDate)
                        .FirstOrDefault());

            return sortedData;
		}

		public IEnumerable<Location> GetQuestionsForDocument(IList<string> sourceURIs, bool partialMatchSourceURI)
		{
			string partialSourceURIToUse = null, partialMatchExactSourceURIToUse = null;
			if (partialMatchSourceURI)
			{
				partialMatchExactSourceURIToUse = sourceURIs.SingleOrDefault();
				if (partialMatchExactSourceURIToUse == null)
					throw new ArgumentException("There should be one and only one source uri passed when doing a partial match.");

				partialSourceURIToUse = $"{partialMatchExactSourceURIToUse}/";
			}

			var data = Location.Where(l => partialMatchSourceURI
					? (l.SourceURI.Equals(partialMatchExactSourceURIToUse) || l.SourceURI.Contains(partialSourceURIToUse))
					: sourceURIs.Contains(l.SourceURI, StringComparer.OrdinalIgnoreCase))

				.Include(l => l.Question)
					.ThenInclude(q => q.QuestionType)

				.OrderBy(l => l.Order)

				.ThenByDescending(l =>
					l.Question.OrderByDescending(c => c.LastModifiedDate).Select(c => c.LastModifiedDate).FirstOrDefault())

				.ToList();

			return data;
		}


	    public virtual IList<SubmittedCommentsAndAnswerCount> GetSubmittedCommentsAndAnswerCounts()
	    {
		    return SubmittedCommentsAndAnswerCounts.ToList();
	    }

        public virtual SubmittedToLeadCommentsAndAnswerCount GetSubmittedToLeadCommentsAndAnswerCounts(string sourceURI, int organisationId)
        {
            var data = SubmittedToLeadCommentsAndAnswerCounts
                .Where(s => s.SourceURI.Equals(sourceURI) && s.OrganisationId.Equals(organisationId))
                .FirstOrDefault();

            return data;
        }

        public List<Comment> GetAllSubmittedCommentsForURI(string sourceURI)
	    {
			var comment = Comment.Where(c =>
					c.StatusId == (int)StatusName.Submitted && (c.Location.SourceURI.Contains($"{sourceURI}/") || c.Location.SourceURI.Equals(sourceURI)))
				.Include(l => l.Location)
				.Include(s => s.Status)
				.Include(sc => sc.SubmissionComment)
				.ThenInclude(s => s.Submission)
				.IgnoreQueryFilters()
				.ToList();

			return comment;
	    }

		public (List<Comment> comments, List<Answer> answers) GetCommentsAndAnswersSubmittedToALeadForURI(string sourceURI)
		{
			var comments = Comment.Where(c =>
					c.StatusId == (int)StatusName.SubmittedToLead &&
					(c.Location.SourceURI.Contains($"{sourceURI}/") || c.Location.SourceURI.Equals(sourceURI)) &&
					c.OrganisationId.Equals(_organisationalLeadOrganisationID))
				.Include(l => l.Location)
				.Include(s => s.Status)
				.Include(sc => sc.SubmissionComment)
				.ThenInclude(s => s.Submission)
				.IgnoreQueryFilters()
				.ToList();

            var answers = Answer.Where(a =>
                    a.StatusId == (int)StatusName.SubmittedToLead &&
                    (a.Question.Location.SourceURI.Contains($"{sourceURI}/") ||
                     a.Question.Location.SourceURI.Equals(sourceURI)) &&
                    a.OrganisationId.Equals(_organisationalLeadOrganisationID))
                .Include(q => q.Question)
                .ThenInclude(l => l.Location)
                .Include(sc => sc.SubmissionAnswer)
                .ThenInclude(s => s.Submission)
                .IgnoreQueryFilters()
                .ToList();

            return (comments, answers);
		}

        public bool GetLeadHasBeenSentResponse(string sourceURI)
        {
            var leadHasBeenSentComment = Comment.Where(c =>
                c.StatusId == (int)StatusName.SubmittedToLead &&
                (c.Location.SourceURI.Contains($"{sourceURI}/") || c.Location.SourceURI.Equals(sourceURI)) &&
                c.OrganisationId.Equals(_organisationalLeadOrganisationID)).IgnoreQueryFilters().Any();

            var leadHasBeenSentAnswer = Answer.Where(a =>
                a.StatusId == (int)StatusName.SubmittedToLead &&
                (a.Question.Location.SourceURI.Contains($"{sourceURI}/") ||
                 a.Question.Location.SourceURI.Equals(sourceURI)) &&
                a.OrganisationId.Equals(_organisationalLeadOrganisationID)).IgnoreQueryFilters().Any();

            var leadHasBeenSentResponse = leadHasBeenSentComment || leadHasBeenSentAnswer;

            return leadHasBeenSentResponse;
        }

		public List<Comment> GetUsersCommentsForURI(string sourceURI)
	    {
		   var comment = Comment.Where(c => (c.Location.SourceURI.Contains($"{sourceURI}/") || c.Location.SourceURI.Equals(sourceURI)))
				.Include(l => l.Location)
				.Include(s => s.Status)
				.Include(sc => sc.SubmissionComment)
				.ThenInclude(s => s.Submission)
				.ToList();

			return comment;
	    }

		public List<Answer> GetAllSubmittedAnswersForURI(string sourceURI)
	    {
			var answer = Answer.Where(a =>
					a.StatusId == (int)StatusName.Submitted && (a.Question.Location.SourceURI.Contains($"{sourceURI}/") || a.Question.Location.SourceURI.Equals(sourceURI)))
				.Include(q => q.Question)
				.ThenInclude(l => l.Location)
				.Include(sc => sc.SubmissionAnswer)
				.ThenInclude(s => s.Submission)
				.IgnoreQueryFilters()
				.ToList();

			return answer;
	    }

        public List<Answer> GetUsersAnswersForURI(string sourceURI)
	    {
			var answer = Answer.Where(a => (a.Question.Location.SourceURI.Contains($"{sourceURI}/") || a.Question.Location.SourceURI.Equals(sourceURI)))
				.Include(q => q.Question)
				.ThenInclude(l => l.Location)
				.Include(sc => sc.SubmissionAnswer)
				.ThenInclude(s => s.Submission)
				.ToList();

			return answer;
	    }

	    public List<Question> GetQuestionsForURI(string sourceURI)
	    {
		    return Question.Where(q => q.Location.SourceURI.Equals(sourceURI))
			    .Include(q => q.Location)
			    .Include(q => q.QuestionType)
			    .IgnoreQueryFilters()
			    .OrderBy(q => q.Location.Order)
			    .ToList();
	    }

		public List<Question> GetUnansweredQuestionsForURI(string sourceURI)
	    {
			var question = Question.Where(q =>
					q.Answer.Count == 0 && (q.Location.SourceURI.Contains($"{sourceURI}/") || q.Location.SourceURI.Equals(sourceURI)) && q.IsDeleted == false)
				.Include(l => l.Location)
				.IgnoreQueryFilters()
				.ToList();

			return question;
	    }

	    public List<Question> GetUsersUnansweredQuestionsForURI(string sourceURI)
	    {
		    var question = Question.Where(q => q.Answer.Count == 0 && (q.Location.SourceURI.Contains($"{sourceURI}/") || q.Location.SourceURI.Equals(sourceURI)))
				    .Include(l => l.Location)
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
		        .Include(q => q.Question)
					.ThenInclude(qt => qt.QuestionType)
		        .Include(q => q.Question)
					.ThenInclude(l => l.Location)
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

			// Comments created by individual commenters can only be submitted by that user, updating the status of those comments
			if(commentsToUpdate.Any(c=>c.CommentByUserType == UserType.IndividualCommenter && c.CreatedByUserId != _createdByUserID))
				throw new Exception("Attempt to update status of a comment which doesn't belong to the current user");

			// Comments created by organisational commenters are submitted to their organisation lead only by that organisational commenter, updating the status of those comments
			if(commentsToUpdate.Any(c => c.CommentByUserType == UserType.OrganisationalCommenter &&  !_organisationUserIDs.Any(o=> o.Equals(c.OrganisationUserId))))
				throw new Exception("Attempt to update status of a comment which doesn't belong to the current organisation code user");

			// Comments that have been submitted to organisational leads, or created by those leads, can be submitted by any lead in that organisation, updating the status of those comments
			if (commentsToUpdate.Any(c => c.CommentByUserType == UserType.OrganisationLead && c.OrganisationId != _organisationalLeadOrganisationID))
				throw new Exception("Attempt to update status of a comment which doesn't belong to the current organisation lead users organisation");

			commentsToUpdate.ForEach(c => c.StatusId = status.StatusId);
		}

	    public void DuplicateComment(IEnumerable<int> commentIds)
	    {
		    var commentsToDuplicate = Comment.Where(c => commentIds.Contains(c.CommentId)).ToList();

		    var status = GetStatus(StatusName.SubmittedToLead);
			commentsToDuplicate.ForEach(c => c.StatusId = status.StatusId);

			status = GetStatus(StatusName.Draft);

		    foreach (var comment in commentsToDuplicate)
			{
				var commentToSave = new Models.Comment(comment.LocationId, null, comment.CommentText, comment.LastModifiedByUserId, comment.Location,status.StatusId, status, comment.OrganisationUserId, comment.CommentId, comment.OrganisationId);
				Comment.Add(commentToSave);
			}
	    }

		public void DuplicateAnswer(IEnumerable<int> answerIds)
		{
			var answersToDuplicate = Answer.Where(c => answerIds.Contains(c.AnswerId)).ToList();

			var status = GetStatus(StatusName.SubmittedToLead);
			answersToDuplicate.ForEach(c => c.StatusId = status.StatusId);

			status = GetStatus(StatusName.Draft);

			foreach (var answer in answersToDuplicate)
			{
				var answerToSave = new Models.Answer(answer.QuestionId, null, answer.AnswerText, answer.AnswerBoolean, answer.Question, status.StatusId, status, answer.OrganisationUserId, answer.AnswerId, answer.OrganisationId);
				Answer.Add(answerToSave);
			}
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

			// Answers created by individual commenters can only be submitted by that user, updating the status of those answers
			if (answersToUpdate.Any(a => a.AnswerByUserType == UserType.IndividualCommenter && a.CreatedByUserId != _createdByUserID))
				throw new Exception("Attempt to update status of an answer which doesn't belong to the current user");

			// Answers created by organisational commenters are submitted to their organisation lead only by that organisational commenter, updating the status of those answers
			if (answersToUpdate.Any(a => a.AnswerByUserType == UserType.OrganisationalCommenter && !_organisationUserIDs.Any(o => o.Equals(a.OrganisationUserId))))
				throw new Exception("Attempt to update status of an answer which doesn't belong to the current organisation code user");

			// Answers that have been submitted to organisational leads, or created by those leads, can be submitted by any lead in that organisation, updating the status of those answers
			if (answersToUpdate.Any(a => a.AnswerByUserType == UserType.OrganisationLead && a.OrganisationId != _organisationalLeadOrganisationID))
				throw new Exception("Attempt to update status of an answer which doesn't belong to the current organisation lead users organisation");

			answersToUpdate.ForEach(a => a.StatusId = status.StatusId);
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

	    public Submission InsertSubmission(string currentUser, bool? respondingAsOrganisation, string organisationName, bool? hasTobaccoLinks, string tobaccoDisclosure, bool? organisationExpressionOfInterest)
	    {
		    var submission = new Models.Submission(currentUser, DateTime.UtcNow, respondingAsOrganisation, organisationName, hasTobaccoLinks, tobaccoDisclosure, organisationExpressionOfInterest);
		    Submission.Add(submission);

		    return submission;
	    }

	    public DateTime? GetSubmittedDate(string sourceURI)
	    {
		    var partialSourceURIToUse = $"{sourceURI}/";

		    var comments = Comment.Where(loc => loc.Location.SourceURI.Equals(sourceURI) || loc.Location.SourceURI.StartsWith(partialSourceURIToUse))
				.Include(l => l.Location)
				.Include(s => s.SubmissionComment)
				.ThenInclude(s => s.Submission)
				.ToList();

			if (comments.Count > 0 )
			{
				if (comments.FirstOrDefault().SubmissionComment.Count > 0)
				{
					return comments.FirstOrDefault().SubmissionComment.FirstOrDefault().Submission.SubmissionDateTime;
				}
			}

			var answers = Answer.Where(q => q.Question.Location.SourceURI.Equals(sourceURI) || q.Question.Location.SourceURI.StartsWith(partialSourceURIToUse))
				.Include(q => q.Question)
				.ThenInclude(l => l.Location)
				.Include(s => s.SubmissionAnswer)
				.ThenInclude(s => s.Submission)
				.ToList();

			if (answers.Count > 0)
			{
				if (answers.FirstOrDefault().SubmissionAnswer.Count > 0)
				{
					return answers.FirstOrDefault().SubmissionAnswer.FirstOrDefault().Submission.SubmissionDateTime;
				}
			}

			return (DateTime?)null;
	    }

		public int DeleteAllSubmissionsFromUser(string usersSubmissionsToDelete)
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
		public int InsertQuestionsWithScriptForDocument1And2InConsultation(int consultationId)
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
								WHERE L.sourceURI = 'consultations://./consultation/' + CAST(@consultationId AS varchar) AND
								Q.QuestionText = @questionOneText)
				BEGIN

					INSERT INTO [Location] (sourceURI, [Order])
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar), @consultationIdPaddedForOrder + '.000.000.000')

					SET @locationID1 = SCOPE_IDENTITY();

					INSERT INTO [Location] (sourceURI, [Order])
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar) + '/document/1', @consultationIdPaddedForOrder + '.001.000.000')

					SET @locationID2 = SCOPE_IDENTITY();

					INSERT INTO [Location] (sourceURI, [Order])
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
		/// this question insert script is temporary, until the question administration features are built.
		/// </summary>
		/// <param name="consultationId"></param>
		/// <returns></returns>
		public int InsertQuestionsWithScriptForConsultation(int consultationId)
		{
			return Database.ExecuteSqlCommand(@"
				--DECLARE @consultationId AS int --UNCOMMENT OUT THESE 2 LINES TO USE IN SQL MANAGEMENT STUDIO
				--SET @consultationId = 210

				DECLARE @questionTypeID AS int
				DECLARE @locationID1 AS int, @locationID2 AS int, @locationID3 AS int

				DECLARE @userID as uniqueidentifier
				SELECT @userID = cast(cast(0 AS binary) AS uniqueidentifier)

				DECLARE @questionTextDescription nvarchar(100)
				SET @questionTextDescription = 'A text question requiring a text answer.'

				DECLARE @questionOneText nvarchar(MAX)
				SET @questionOneText = 'Has all of the relevant evidence been taken into account?'

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

				--3 location inserts. the questions are all consultation level, but there's an order to preserve.
				IF NOT EXISTS (SELECT * FROM [Location] L
								INNER JOIN Question Q ON Q.LocationID = L.LocationID
								WHERE L.sourceURI = 'consultations://./consultation/' + CAST(@consultationId AS varchar) AND
								Q.QuestionText = @questionOneText)
				BEGIN

					INSERT INTO [Location] (sourceURI, [Order])
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar), @consultationIdPaddedForOrder + '.000.000.000.001')

					SET @locationID1 = SCOPE_IDENTITY();

					INSERT INTO [Location] (sourceURI, [Order])
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar), @consultationIdPaddedForOrder + '.000.000.000.002')

					SET @locationID2 = SCOPE_IDENTITY();

					INSERT INTO [Location] (sourceURI, [Order])
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar), @consultationIdPaddedForOrder + '.000.000.000.003')

					SET @locationID3 = SCOPE_IDENTITY();

					--now the question inserts

					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID1, @questionOneText, @questionTypeID, @userID, @userID, GETDATE())

					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID2, 'Are the summaries of clinical and cost effectiveness reasonable interpretations of the evidence?', @questionTypeID, @userID, @userID, GETDATE())

					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID3, 'Are the recommendations sound and a suitable basis for guidance to the NHS?', @questionTypeID, @userID, @userID, GETDATE())

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

		/// <summary>
		/// This method is just here for temporary debug purposes. it's locked down so only administrators can run it, and even then it's only meant for devs.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IList<object> GetAllOfATable(string typeName)
	    {
			//the below works and is kinda nice, but the big switch is probably safer.
			//var method = typeof(DbContext).GetMethod("Set").MakeGenericMethod(Type.GetType("Comments.Models." + typeName));
			//var query = method.Invoke(this, null) as IQueryable;
			//return (query ?? throw new InvalidOperationException()).OfType<object>().ToList();

		    switch (typeName.ToLower())
		    {
				case "submissioncomment":
					return SubmissionComment.IgnoreQueryFilters().Select(x => (object)x).ToList();
			    case "submissionanswer":
				    return SubmissionAnswer.IgnoreQueryFilters().Select(x => (object)x).ToList();
			    case "submission":
				    return Submission.IgnoreQueryFilters().Select(x => (object)x).ToList();
			    case "answer":
				    return Answer.IgnoreQueryFilters().Select(x => (object)x).ToList();
			    case "question":
				    return Question.IgnoreQueryFilters().Select(x => (object)x).ToList();
			    case "comment":
				    return Comment.IgnoreQueryFilters().Select(x => (object)x).ToList();
			    case "location":
				    return Location.IgnoreQueryFilters().Select(x => (object)x).ToList();
			    case "questiontype":
				    return QuestionType.IgnoreQueryFilters().Select(x => (object)x).ToList();
			    case "status":
				    return Status.IgnoreQueryFilters().Select(x => (object)x).ToList();

				default:
					throw new Exception("Unknown table name");
		    }
	    }

	    public (int totalComments, int totalAnswers, int totalSubmissions) GetStatusData()
	    {
		    return (totalComments: Comment.IgnoreQueryFilters().Count(),
			    totalAnswers: Answer.IgnoreQueryFilters().Count(),
			    totalSubmissions: Submission.IgnoreQueryFilters().Count());
	    }

	    public IEnumerable<QuestionType> GetQuestionTypes()
	    {
		    return QuestionType;
	    }

		/// <summary>
		/// this question insert script is temporary, until the question administration features are built.
		/// </summary>
		/// <param name="consultationId"></param>
		/// <returns></returns>
		public int InsertQuestionsWithScriptForCfGConsultation(int consultationId)
		{
			return Database.ExecuteSqlCommand(@"
				--DECLARE @consultationId AS int --UNCOMMENT OUT THESE 2 LINES TO USE IN SQL MANAGEMENT STUDIO
				--SET @consultationId = 210

				DECLARE @questionTypeID AS int
				DECLARE @locationID1 AS int, @locationID2 AS int, @locationID3 AS int

				DECLARE @userID as uniqueidentifier
				SELECT @userID = cast(cast(0 AS binary) AS uniqueidentifier)

				DECLARE @questionTextDescription nvarchar(100)
				SET @questionTextDescription = 'A text question requiring a text answer.'

				DECLARE @questionOneText nvarchar(MAX)
				SET @questionOneText = 'Do you agree with the proposal for a partial update/not to update the guideline?'

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

				--3 location inserts. the questions are all consultation level, but there's an order to preserve.
				IF NOT EXISTS (SELECT * FROM [Location] L
								INNER JOIN Question Q ON Q.LocationID = L.LocationID
								WHERE L.sourceURI = 'consultations://./consultation/' + CAST(@consultationId AS varchar) AND
								Q.QuestionText = @questionOneText)
				BEGIN

					INSERT INTO [Location] (sourceURI, [Order])
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar), @consultationIdPaddedForOrder + '.000.000.000.001')

					SET @locationID1 = SCOPE_IDENTITY();

					INSERT INTO [Location] (sourceURI, [Order])
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar), @consultationIdPaddedForOrder + '.000.000.000.002')

					SET @locationID2 = SCOPE_IDENTITY();

					INSERT INTO [Location] (sourceURI, [Order])
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar), @consultationIdPaddedForOrder + '.000.000.000.003')

					SET @locationID3 = SCOPE_IDENTITY();

					--now the question inserts

					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID1, @questionOneText, @questionTypeID, @userID, @userID, GETDATE())

					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID2, 'Do you have any comments on areas excluded from the scope of the guideline?', @questionTypeID, @userID, @userID, GETDATE())

					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID3, 'Do you have any comments on equalities issues?', @questionTypeID, @userID, @userID, GETDATE())

				END

			", new SqlParameter("@consultationId", consultationId));
		}

		/// <summary>
		/// this question insert script is temporary, until the question administration features are built.
		/// </summary>
		/// <param name="consultationId"></param>
		/// <returns></returns>
		public int InsertQuestionsWithScriptForQSConsultation(int consultationId)
		{
			return Database.ExecuteSqlCommand(@"
				--DECLARE @consultationId AS int --UNCOMMENT OUT THESE 2 LINES TO USE IN SQL MANAGEMENT STUDIO
				--SET @consultationId = 210

				DECLARE @questionTypeID AS int
				DECLARE @locationID1 AS int, @locationID2 AS int, @locationID3 AS int

				DECLARE @userID as uniqueidentifier
				SELECT @userID = cast(cast(0 AS binary) AS uniqueidentifier)

				DECLARE @questionTextDescription nvarchar(100)
				SET @questionTextDescription = 'A text question requiring a text answer.'

				DECLARE @questionOneText nvarchar(MAX)
				SET @questionOneText = 'Does this draft quality standard accurately reflect the key areas for quality improvement?'

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

				--3 location inserts. the questions are all consultation level, but there's an order to preserve.
				IF NOT EXISTS (SELECT * FROM [Location] L
								INNER JOIN Question Q ON Q.LocationID = L.LocationID
								WHERE L.sourceURI = 'consultations://./consultation/' + CAST(@consultationId AS varchar) AND
								Q.QuestionText = @questionOneText)
				BEGIN

					INSERT INTO [Location] (sourceURI, [Order])
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar), @consultationIdPaddedForOrder + '.000.000.000.001')

					SET @locationID1 = SCOPE_IDENTITY();

					INSERT INTO [Location] (sourceURI, [Order])
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar), @consultationIdPaddedForOrder + '.000.000.000.002')

					SET @locationID2 = SCOPE_IDENTITY();

					INSERT INTO [Location] (sourceURI, [Order])
					VALUES ('consultations://./consultation/' + CAST(@consultationId AS varchar), @consultationIdPaddedForOrder + '.000.000.000.003')

					SET @locationID3 = SCOPE_IDENTITY();

					--now the question inserts

					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID1, @questionOneText, @questionTypeID, @userID, @userID, GETDATE())

					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID2, 'Are local systems and structures in place to collect data for the proposed quality measures? If not, how feasible would it be for these to be put in place?', @questionTypeID, @userID, @userID, GETDATE())

					INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
					VALUES (@locationID3, 'Do you think each of the statements in this draft quality standard would be achievable by local services given the net resources needed to deliver them? Please describe any resource requirements that you think would be necessary for any statement. Please describe any potential cost savings or opportunities for disinvestment', @questionTypeID, @userID, @userID, GETDATE())

				END

			", new SqlParameter("@consultationId", consultationId));
		}

		public IEnumerable<Question> GetAllPreviousUniqueQuestions()
		{
			return Question
				.Include(q => q.Location)
				.Include(q => q.QuestionType)
				.GroupBy(q => q.QuestionText)
				.Select(q => q.OrderByDescending(x => x.CreatedDate).First())
				.OrderByDescending(q => q.CreatedDate);
		}

		public IEnumerable<string> GetUniqueUsers()
		{
			var allUserIds =    Answer.IgnoreQueryFilters().Select(answer => answer.CreatedByUserId).Distinct().Concat(
								Answer.IgnoreQueryFilters().Select(answer => answer.LastModifiedByUserId).Distinct().Concat(

								Comment.IgnoreQueryFilters().Select(comment => comment.CreatedByUserId).Distinct().Concat(
								Comment.IgnoreQueryFilters().Select(comment => comment.LastModifiedByUserId).Distinct().Concat(

								Question.IgnoreQueryFilters().Select(question => question.CreatedByUserId).Distinct().Concat(
								Question.IgnoreQueryFilters().Select(question => question.LastModifiedByUserId).Distinct().Concat(

								Submission.IgnoreQueryFilters().Select(comment => comment.SubmissionByUserId).Distinct()))))));

			return allUserIds.Distinct();
		}


		/// <summary>
		/// Returns a list of all the source uri's and the status of each for the current users comments and answers
		/// note: there could be duplicate source uris. this list will likely need filtering in the service.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<string, Status>> GetAllSourceURIsTheCurrentUserHasCommentedOrAnsweredAQuestion()
		{
			var comments = Comment
				.Include(l => l.Location)
				.Include(s => s.Status)
				.ToList();

			var answers = Answer
				.Include(q => q.Question)
				.ThenInclude(l => l.Location)
				.Include(s => s.Status)
				.ToList();

			var commentSourceURIsAndStatus = comments.Select(comment => new KeyValuePair<string, Status>(comment.Location.SourceURI, comment.Status));
			var answerSourceURIsAndStatus = answers.Select(answer => new KeyValuePair<string, Status>(answer.Question.Location.SourceURI, answer.Status));
			return commentSourceURIsAndStatus.Concat(answerSourceURIsAndStatus);
		}

		public IEnumerable<OrganisationAuthorisation> GetOrganisationAuthorisations(IList<string> consultationSourceURIs)
		{
			var organisationAuthorisations = OrganisationAuthorisation
				.Include(oa => oa.Location)
				.Where(oa => consultationSourceURIs.Contains(oa.Location.SourceURI, StringComparer.OrdinalIgnoreCase))
				.ToList();

			return organisationAuthorisations;
		}

		public OrganisationAuthorisation GetOrganisationAuthorisationByCollationCode(string collationCode)
		{
			collationCode = collationCode.Replace(" ", "");

			var organisationAuthorisations = OrganisationAuthorisation
				.Include(oa => oa.Location)
				.Where(oa => oa.CollationCode.Equals(collationCode))
				.ToList();

			return organisationAuthorisations.FirstOrDefault(); //there should only be 1 with a given collation code.
		}

		public OrganisationAuthorisation SaveCollationCode(string sourceURI, string createdByUserId, DateTime createdDate, int organisationId, string collationCode)
		{
			collationCode = collationCode.Replace(" ", "");

			var location = new Location(sourceURI, null, null, null, null, null, null, null, null, null, null);
			Location.Add(location);
			SaveChanges();
			var organisationAuthorisation = new OrganisationAuthorisation(createdByUserId, createdDate, organisationId, location.LocationId, collationCode);
			OrganisationAuthorisation.Add(organisationAuthorisation);
			SaveChanges();
			return organisationAuthorisation;
		}

		public OrganisationUser CreateOrganisationUser(int organisationAuthorisationID, Guid authorisationSession, DateTime expirationDate)
		{
			var organisationUser = new OrganisationUser(organisationAuthorisationID, authorisationSession, expirationDate);
			OrganisationUser.Add(organisationUser);
			SaveChanges();
			return organisationUser;
		}

		public OrganisationUser UpdateEmailAddressForOrganisationUser(string emailAddress, int organisationUserId)
		{
			var organisationUser = OrganisationUser
				.Where(ou => ou.OrganisationUserId.Equals(organisationUserId))
				.Single();

			organisationUser.EmailAddress = emailAddress;

			SaveChanges();
			return organisationUser;
		}

		public OrganisationUser GetOrganisationUser(Guid sessionId)
		{
			return GetOrganisationUsers(new List<Guid> {sessionId}).FirstOrDefault();
		}

		public IEnumerable<OrganisationUser> GetOrganisationUsers(IEnumerable<Guid> sessionIds)
		{
			return
				OrganisationUser
					.Include(ou => ou.OrganisationAuthorisation).
					ThenInclude(a => a.Location)
					.Where(ou => sessionIds.Contains(ou.AuthorisationSession));
		}

		public IEnumerable<OrganisationUser> GetOrganisationUsersByOrganisationUserIds(IEnumerable<int> organisationUserIds)
		{
			return OrganisationUser.Where(ou => organisationUserIds.Contains(ou.OrganisationUserId));
		}


		public bool AreCommentsForThisOrganisation(IEnumerable<int> commentIds, int organisationId)
		{
			var comments = Comment
				.Include(c => c.OrganisationUser)
					.ThenInclude(ou => ou.OrganisationAuthorisation)
				.IgnoreQueryFilters()
				.Where(c => commentIds.Contains(c.CommentId)).ToList();

			return comments.Where(c => c.OrganisationUser?.OrganisationAuthorisation != null)
				.Any(c => c.OrganisationUser.OrganisationAuthorisation.OrganisationId.Equals(organisationId));
		}

		public bool AreAnswersForThisOrganisation(List<int> answerIds, int organisationId)
		{
			var answers = Answer
				.Include(a => a.OrganisationUser)
					.ThenInclude(ou => ou.OrganisationAuthorisation)
				.IgnoreQueryFilters()
				.Where(a => answerIds.Contains(a.AnswerId)).ToList();

			return answers.Where(c => c.OrganisationUser?.OrganisationAuthorisation != null)
				.Any(c => c.OrganisationUser.OrganisationAuthorisation.OrganisationId.Equals(organisationId));

		}

        public List<string> GetEmailAddressForCommentsAndAnswers(CommentsAndQuestions commentsAndQuestions)
        {
            var commentIds = commentsAndQuestions.Comments.Select(c => c.CommentId).ToList();
            var answerIds = commentsAndQuestions.Questions.SelectMany(q => q.Answers).Select(a => a.AnswerId).ToList();

            var comments = Comment.Where(c => commentIds.Contains(c.CommentId)).ToList();
            var answers = Answer.Where(a => answerIds.Contains(a.AnswerId)).ToList();

            var emailAddresses = OrganisationUser.Where(o => comments.Select(c => c.OrganisationUserId).Contains(o.OrganisationUserId)
                                                || answers.Select(a => a.OrganisationUserId).Contains(o.OrganisationUserId))
                                                .Select(o => o.EmailAddress)
                                                .Distinct()
                                                .ToList();
            return emailAddresses;

        }
    }
}
