using Comments.ViewModels;
using NICE.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Common;
using Comments.Models;
using Microsoft.Extensions.Logging;

namespace Comments.Services
{
    public interface IConsultationService
    {
        ConsultationDetail GetConsultationDetail(int consultationId);
        ChapterContent GetChapterContent(int consultationId, int documentId, string chapterSlug);
        IEnumerable<Document> GetDocuments(int consultationId);
        ViewModels.Consultation GetConsultation(int consultationId);
        IEnumerable<ViewModels.Consultation> GetConsultations();
	    ConsultationState GetConsultationState(string sourceURI, IEnumerable<Models.Location> locations = null);
	    (int rowsUpdated, Validate validate) SubmitCommentsAndAnswers(CommentsAndAnswers commentsAndAnswers);
	    bool HasSubmittedCommentsOrQuestions(string consultationSourceURI, Guid userId);
	    (IList<ViewModels.Comment> comments, IList<ViewModels.Question> questions) ConvertLocationsToCommentsAndQuestionsViewModels(IEnumerable<Models.Location> locations);
	}

	public class ConsultationService : IConsultationService
    {

        private readonly IFeedService _feedConverterService;
        private readonly ILogger<ConsultationService> _logger;
        private readonly IUserService _userService;
	    private readonly ConsultationsContext _context;
	    private readonly User _currentUser;

		public ConsultationService(ConsultationsContext context, IFeedService feedConverterService, ILogger<ConsultationService> logger, IUserService userService)
        {
	        _context = context;
			_feedConverterService = feedConverterService;
            _logger = logger;
            _userService = userService;
	        _currentUser = userService.GetCurrentUser();
		}

        public ConsultationDetail GetConsultationDetail(int consultationId)
        {
            var user = _userService.GetCurrentUser();
            return new ViewModels.ConsultationDetail(_feedConverterService.GetIndevConsultationDetailForPublishedProject(consultationId, PreviewState.NonPreview), user); 
        }

        public ChapterContent GetChapterContent(int consultationId, int documentId, string chapterSlug)
        {
            return new ViewModels.ChapterContent(
                _feedConverterService.GetConsultationChapterForPublishedProject(consultationId, documentId, chapterSlug));
        }

        public IEnumerable<Document> GetDocuments(int consultationId)
        {
            var consultationDetail = _feedConverterService.GetIndevConsultationDetailForPublishedProject(consultationId, PreviewState.NonPreview);
            return consultationDetail.Resources.Select(r => new ViewModels.Document(consultationId, r)).ToList();
        }

        public ViewModels.Consultation GetConsultation(int consultationId)
        {
            var user = _userService.GetCurrentUser();
            var consultation = _feedConverterService.GetIndevConsultationDetailForPublishedProject(consultationId, PreviewState.NonPreview);
            return new ViewModels.Consultation(consultation, user);
        }

        public IEnumerable<ViewModels.Consultation> GetConsultations()
        {
            var user = _userService.GetCurrentUser();
            var consultations = _feedConverterService.GetConsultationList();
            return consultations.Select(c => new ViewModels.Consultation(c, user)).ToList();
        }

	    public ConsultationState GetConsultationState(string sourceURI, IEnumerable<Models.Location> locations = null)
	    {
		    var currentUser = _userService.GetCurrentUser();
			var consultationsUriElements = ConsultationsUri.ParseConsultationsUri(sourceURI);
		    var consultationDetail = GetConsultationDetail(consultationsUriElements.ConsultationId);

		    if (locations == null)
		    {
			    locations = _context.GetAllCommentsAndQuestionsForDocument(new[]{ sourceURI }, isReview: true);
		    }

			var hasSubmitted = currentUser != null && currentUser.IsAuthorised && currentUser.UserId.HasValue ? HasSubmittedCommentsOrQuestions(sourceURI, currentUser.UserId.Value) : false;

		    var data = ConvertLocationsToCommentsAndQuestionsViewModels(locations);

		    var consultationState = new ConsultationState(consultationDetail.StartDate, consultationDetail.EndDate,
			    data.questions.Any(), data.questions.Any(q => q.Answers.Any()), data.comments.Any(), hasSubmitted);

		    return consultationState;
	    }

		public (int rowsUpdated, Validate validate) SubmitCommentsAndAnswers(CommentsAndAnswers commentsAndAnswers)
		{
			if (!_currentUser.IsAuthorised || !_currentUser.UserId.HasValue)
				return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in submitting comments and answers"));

			//if a user is submitting a different users comment, the context will throw an exception.

			var anySourceURI = commentsAndAnswers.SourceURIs.FirstOrDefault();
			if (anySourceURI == null)
				return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: false, message: "Could not find SourceURI"));

			var consultationState = GetConsultationState(anySourceURI);

			if (!consultationState.ConsultationIsOpen)
				return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: false, message: "Consultation is not open for submissions"));

			var hasSubmitted = HasSubmittedCommentsOrQuestions(anySourceURI, _currentUser.UserId.Value);
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

		public bool HasSubmittedCommentsOrQuestions(string anySourceURI, Guid userId)
		{
			if (string.IsNullOrWhiteSpace(anySourceURI))
				return false;

			var consultationsUriElements = ConsultationsUri.ParseConsultationsUri(anySourceURI);

			var consultationSourceURI = ConsultationsUri.CreateConsultationURI(consultationsUriElements.ConsultationId);

			return _context.HasSubmitted(consultationSourceURI, userId);
		}

	    public (IList<ViewModels.Comment> comments, IList<ViewModels.Question> questions) ConvertLocationsToCommentsAndQuestionsViewModels(IEnumerable<Models.Location> locations)
	    {
		    var commentsData = new List<ViewModels.Comment>();
		    var questionsData = new List<ViewModels.Question>();
		    foreach (var location in locations)
		    {
			    commentsData.AddRange(location.Comment.Select(comment => new ViewModels.Comment(location, comment)));
			    questionsData.AddRange(location.Question.Select(question => new ViewModels.Question(location, question)));
		    }
		    return (commentsData, questionsData);
	    }
	}
}
