using System;
using System.Linq;
using Comments.Common;
using Comments.ViewModels;
using Comments.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Comments.Services
{
    public interface IAnswerService
    {
        (ViewModels.Answer answer, Validate validate) GetAnswer(int answerId);
        (int rowsUpdated, Validate validate) EditAnswer(int answerId, ViewModels.Answer answer);
        (int rowsUpdated, Validate validate) DeleteAnswer(int answerId);
        (ViewModels.Answer answer, Validate validate) CreateAnswer(ViewModels.Answer answer);
    }
    public class AnswerService : IAnswerService
    {
        private readonly ConsultationsContext _context;
        private readonly IUserService _userService;
        private readonly User _currentUser;

        public AnswerService(ConsultationsContext consultationsContext, IUserService userService)
        {
            _context = consultationsContext;
            _userService = userService;
            _currentUser = _userService.GetCurrentUser();
        }
        public (ViewModels.Answer answer, Validate validate) GetAnswer(int answerId)
        {
            if (!_currentUser.IsAuthenticatedByAnyMechanism)
                return (answer: null, validate: new Validate(valid: false, unauthenticated: true, message: $"Not logged in accessing answer id:{answerId}"));

            var answerInDatabase = _context.GetAnswer(answerId);

            if (answerInDatabase == null)
                return (answer: null, validate: new Validate(valid: false, notFound: true, message: $"Answer id:{answerId} not found trying to get answer for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));

            var consultationId = ConsultationsUri.ParseConsultationsUri(answerInDatabase.Question.Location.SourceURI).ConsultationId;
            if (_currentUser.IsAuthenticatedByOrganisationCookieForThisConsultation(consultationId))
            {
	            if (!answerInDatabase.OrganisationUserId.HasValue || !_currentUser.IsAuthorisedByOrganisationUserId(answerInDatabase.OrganisationUserId.Value))
		            return (answer: null, validate: new Validate(valid: false, unauthenticated: true, message: $"Organisation cookie user tried to access answer id: {answerId}, but it's not their answer"));
            }
            else
            {
				if (!answerInDatabase.CreatedByUserId.Equals(_currentUser.UserId, StringComparison.OrdinalIgnoreCase))
					return (answer: null, validate: new Validate(valid: false, unauthenticated: true, message: $"User id: {_currentUser.UserId} display name: {_currentUser.DisplayName} tried to access answer id: {answerId}, but it's not their answer"));
            }
			return (answer: new ViewModels.Answer(answerInDatabase), validate: null);
		}

        public (int rowsUpdated, Validate validate) EditAnswer(int answerId, ViewModels.Answer answer)
        {
            if (!_currentUser.IsAuthenticatedByAnyMechanism)
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: true, message: $"Not logged in editing answer id:{answerId}"));

            var answerInDatabase = _context.GetAnswer(answerId);

            if (answerInDatabase == null)
                return (rowsUpdated: 0, validate: new Validate(valid: false, notFound: true, message: $"Answer id:{answerId} not found trying to edit answer for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));

            // Answers created by individal commenters can only be edited by that commenter.
            if (answerInDatabase.AnswerByUserType == UserType.IndividualCommenter && !_currentUser.UserId.Equals(answerInDatabase.CreatedByUserId))
	            return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: true, message: $"User id: {_currentUser.UserId} display name: {_currentUser.DisplayName} tried to edit answer id: {answerId}, but it's not their answer"));

            // Answers created by organisational commenters and not yet submitted to a lead can only be edited by that commenter
            if (answerInDatabase.AnswerByUserType == UserType.OrganisationalCommenter && !_currentUser.ValidatedOrganisationUserIds.Any(o => o.Equals(answerInDatabase.OrganisationUserId)))
	            return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: false, unauthorised: true, message: $"Organisation commenter tried to edit answer id: {answerId}, but it's not their answer"));

            // Answers submitted to organisational leads or created by organisational leads can be edited by any leads for that organisation
            if (answerInDatabase.AnswerByUserType == UserType.OrganisationLead && !_currentUser.OrganisationsAssignedAsLead.Any(o => o.OrganisationId.Equals(answerInDatabase.OrganisationId)))
	            return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: false, unauthorised: true, message: $"Organisation lead tried to edit answer id: {answerId}, but it's not from their organisation"));


			answer.LastModifiedByUserId = _currentUser.UserId;
	        answer.LastModifiedDate = DateTime.UtcNow;
			answerInDatabase.UpdateFromViewModel(answer);
            return (rowsUpdated: _context.SaveChanges(), validate: null);
        }

        public (int rowsUpdated, Validate validate) DeleteAnswer(int answerId)
        {
            if (!_currentUser.IsAuthenticatedByAnyMechanism)
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: true, message: $"Not logged in deleting answer id:{answerId}"));

            var answerInDatabase = _context.GetAnswer(answerId);

            if (answerInDatabase == null)
                return (rowsUpdated: 0, validate: new Validate(valid: false, notFound: true, message: $"Answer id:{answerId} not found trying to delete answer for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));

            // Answers created by individal commenters can only be deleted by that commenter.
			if (answerInDatabase.AnswerByUserType == UserType.IndividualCommenter && !_currentUser.UserId.Equals(answerInDatabase.CreatedByUserId))
				return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: true, message: $"User id: {_currentUser.UserId} display name: {_currentUser.DisplayName} tried to delete answer id: {answerId}, but it's not their answer"));

			// Answers created by organisational commenters and not yet submitted to a lead can only be deleted by that commenter
			if (answerInDatabase.AnswerByUserType == UserType.OrganisationalCommenter && !_currentUser.ValidatedOrganisationUserIds.Any(o => o.Equals(answerInDatabase.OrganisationUserId)))
				return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: false, unauthorised: true, message: $"Organisation commenter tried to delete answer id: {answerId}, but it's not their answer"));

			// Answers submitted to organisational leads or created by organisational leads can be deleted by any leads for that organisation
			if (answerInDatabase.AnswerByUserType == UserType.OrganisationLead && !_currentUser.OrganisationsAssignedAsLead.Any(o => o.OrganisationId.Equals(answerInDatabase.OrganisationId)))
				return (rowsUpdated: 0, validate: new Validate(valid: false, unauthenticated: false, unauthorised: true, message: $"Organisation lead tried to delete answer id: {answerId}, but it's not from their organisation"));

			_context.Answer.Remove(answerInDatabase);

			return (rowsUpdated: _context.SaveChanges(), validate: null);
		}

        public (ViewModels.Answer answer, Validate validate) CreateAnswer(ViewModels.Answer answer)
        {
            if (!_currentUser.IsAuthenticatedByAnyMechanism)
                return (answer: null, validate: new Validate(valid: false, unauthenticated: true, message: "Not logged in creating answer"));

            var question = _context.GetQuestion(answer.QuestionId);
            var consultationId = ConsultationsUri.ParseConsultationsUri(question.Location.SourceURI).ConsultationId;
			if (!_currentUser.IsAuthorisedByConsultationId(consultationId))
				return (answer: null, validate: new Validate(valid: false, unauthenticated: false, unauthorised: true, message: "Not authorised to create answer on this consultation"));

			var status = _context.GetStatus(StatusName.Draft);

	        int? organisationUserId = null;
	        int? organisationId;
	        var userId = _currentUser.UserId;
			if (_currentUser.IsAuthenticatedByOrganisationCookieForThisConsultation(consultationId))
			{
				organisationUserId = _currentUser.ValidatedSessions.FirstOrDefault(session => session.ConsultationId.Equals(consultationId))?.OrganisationUserId;
				organisationId = _currentUser.ValidatedSessions.FirstOrDefault(session => session.ConsultationId.Equals(consultationId))?.OrganisationId;
				userId = null;
			}
			else
			{
				organisationId = _currentUser.OrganisationsAssignedAsLead?.FirstOrDefault()?.OrganisationId;
			}
			
			var answerToSave = new Models.Answer(answer.QuestionId, userId, answer.AnswerText, answer.AnswerBoolean, question, status.StatusId, null, organisationUserId, null, organisationId);
	        answerToSave.LastModifiedByUserId = _currentUser.UserId;
	        answerToSave.LastModifiedDate = DateTime.UtcNow;
			_context.Answer.Add(answerToSave);
            _context.SaveChanges();

            return (answer: new ViewModels.Answer(answerToSave, question), validate: null);
        }
    }
}
