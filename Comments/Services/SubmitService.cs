using System;
using Comments.Models;
using Comments.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Comments.Services.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NICE.Feeds;

namespace Comments.Services
{
	public interface ISubmitService
	{
		(int rowsUpdated, Validate validate) Submit(ViewModels.Submission submission);
	}

	public class SubmitService : ISubmitService
	{
		private readonly ConsultationsContext _context;
		private readonly IConsultationService _consultationService;
		private readonly User _currentUser;

		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly IAnalysisService _analysisService;
		private readonly ILogger<SubmitService> _logger;
		private readonly IServiceProvider _services;
		public IBackgroundTaskQueue Queue { get; }

		public SubmitService(ConsultationsContext context, IUserService userService, IConsultationService consultationService,
			IServiceScopeFactory serviceScopeFactory, IAnalysisService analysisService, IBackgroundTaskQueue queue, ILogger<SubmitService> logger, IServiceProvider services)
		{
			_context = context;
			_consultationService = consultationService;
			_currentUser = userService.GetCurrentUser();
			_serviceScopeFactory = serviceScopeFactory;
			_analysisService = analysisService;
			_logger = logger;
			_services = services;
			Queue = queue;
		}

		public (int rowsUpdated, Validate validate) Submit(ViewModels.Submission submission)
		{
			if (!_currentUser.IsAuthorised || !_currentUser.UserId.HasValue)
				return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in submitting comments and answers"));

			//if a user is submitting a different users comment, the context will throw an exception.

			var anySourceURI = submission.SourceURIs.FirstOrDefault();
			if (anySourceURI == null)
				return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: false, message: "Could not find SourceURI"));

			var consultationState = _consultationService.GetConsultationState(anySourceURI, PreviewState.NonPreview);

			if (!consultationState.ConsultationIsOpen)
				return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: false, message: "Consultation is not open for submissions"));

			var hasSubmitted = _consultationService.HasSubmittedCommentsOrQuestions(anySourceURI, _currentUser.UserId.Value);
			if (hasSubmitted)
				return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: false, message: "User has already submitted."));

			var submissionToSave = _context.InsertSubmission(_currentUser.UserId.Value, submission.RespondingAsOrganisation, submission.OrganisationName, submission.HasTobaccoLinks, submission.TobaccoDisclosure);

			var submittedStatus = _context.GetStatus(StatusName.Submitted);

			UpdateCommentsModel(submission.Comments, submissionToSave, submittedStatus);
			UpdateAnswersModel(submission.Answers, submissionToSave, submittedStatus);

			//now for analytics calculate the number of seconds between the user's first comment or answer and the submission date
			var earliestDate = submissionToSave.SubmissionComment.Any() ? submissionToSave.SubmissionComment.Min(sc => sc.Comment.CreatedDate) : DateTime.MaxValue;
			var earliestAnswer = submissionToSave.SubmissionAnswer.Any() ? submissionToSave.SubmissionAnswer.Min(sa => sa.Answer.CreatedDate) : DateTime.MaxValue;
			if (earliestAnswer < earliestDate)
			{
				earliestDate = earliestAnswer;
			}
			submission.DurationBetweenFirstCommentOrAnswerSavedAndSubmissionInSeconds = (submissionToSave.SubmissionDateTime - earliestDate).TotalSeconds;

			//now analyse the data using Machine learning at AWS. this bit runs in a background task so the user doesn't have to wait.
			Queue.QueueBackgroundWorkItem(async token =>
			{
				using (var scope = _serviceScopeFactory.CreateScope())
				{
					var context = scope.ServiceProvider.GetRequiredService<ConsultationsContext>();
					//var context = scopedServices.GetRequiredService<ConsultationsContext>();

					try
					{
						var analysisTimer = Stopwatch.StartNew();
						await _analysisService.AnalyseAndUpdateDatabase(context, submission.Comments, submission.Answers);
						analysisTimer.Stop();

						_logger.LogInformation(
							$"Successfully analysed {submission.Comments.Count} comment(s) and {submission.Answers.Count} answer(s) in {analysisTimer.ElapsedMilliseconds} milliseconds.");
					}
					catch (Exception ex)
					{
						_logger.LogError(ex,
							$"Failure to analyse comments and answers for submission {submissionToSave.SubmissionId}");
					}
				}
			});

			return (rowsUpdated: _context.SaveChanges(), validate: null);
		}

		private void UpdateCommentsModel(IList<ViewModels.Comment> comments, Models.Submission submission, Models.Status status)
		{
			var commentIds = comments.Select(c => c.CommentId).ToList();
			_context.UpdateCommentStatus(commentIds, status);

			foreach (var commentInViewModel in comments)
			{
				commentInViewModel.UpdateStatusFromDBModel(status);
			}
			_context.AddSubmissionComments(commentIds, submission.SubmissionId);
		}

		private void UpdateAnswersModel(IList<ViewModels.Answer> answers, Models.Submission submission, Models.Status status)
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
