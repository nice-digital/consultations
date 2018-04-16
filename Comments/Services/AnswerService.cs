using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.ViewModels;
using Comments.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

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
            if (!_currentUser.IsLoggedIn)
                return (answer: null, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in accessing answer id:{answerId}"));

            var answerInDatabase = _context.GetAnswer(answerId);

            if (answerInDatabase == null)
                return (answer: null, validate: new Validate(valid: false, notFound: true, message: $"Answer id:{answerId} not found trying to get answer for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));

            if (!answerInDatabase.CreatedByUserId.Equals(_currentUser.UserId.Value))
                return (answer: null, validate: new Validate(valid: false, unauthorised: true, message: $"User id: {_currentUser.UserId} display name: {_currentUser.DisplayName} tried to access answer id: {answerId}, but it's not their answer"));


            return (answer: (answerInDatabase == null) ? null : new ViewModels.Answer(answerInDatabase), validate: null);
        }

        public (int rowsUpdated, Validate validate) EditAnswer(int answerId, ViewModels.Answer answer)
        {
            if (!_currentUser.IsLoggedIn)
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in editing answer id:{answerId}"));

            var answerInDatabase = _context.GetAnswer(answerId);

            if (answerInDatabase == null)
                return (rowsUpdated: 0, validate: new Validate(valid: false, notFound: true, message: $"Answer id:{answerId} not found trying to edit answer for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));

            if (!answerInDatabase.CreatedByUserId.Equals(_currentUser.UserId.Value))
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"User id: {_currentUser.UserId} display name: {_currentUser.DisplayName} tried to edit answer id: {answerId}, but it's not their answer"));


            answerInDatabase.UpdateFromViewModel(answer);

            return (rowsUpdated: _context.SaveChanges(), validate: null);
        }

        public (int rowsUpdated, Validate validate) DeleteAnswer(int answerId)
        {
            if (!_currentUser.IsLoggedIn)
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in deleting answer id:{answerId}"));

            var answerInDatabase = _context.GetAnswer(answerId);

            if (answerInDatabase == null)
                return (rowsUpdated: 0, validate: new Validate(valid: false, notFound: true, message: $"Answer id:{answerId} not found trying to delete answer for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));

            if (!answerInDatabase.CreatedByUserId.Equals(_currentUser.UserId.Value))
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"User id: {_currentUser.UserId} display name: {_currentUser.DisplayName} tried to delete answer id: {answerId}, but it's not their answer"));

            answerInDatabase.IsDeleted = true;
            return (rowsUpdated: _context.SaveChanges(), validate: null);
        }

        public (ViewModels.Answer answer, Validate validate) CreateAnswer(ViewModels.Answer answer)
        {
            var currentlyLoggedOnUserId = Guid.NewGuid();
            var answerToSave = new Models.Answer(answer.QuestionId, currentlyLoggedOnUserId, answer.AnswerText, answer.AnswerBoolean, null);

            _context.Answer.Add(answerToSave);
            _context.SaveChanges();

            return (answer: new ViewModels.Answer(answerToSave), validate: null);
        }
    }
}
