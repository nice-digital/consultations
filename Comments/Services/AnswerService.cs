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
        int EditAnswer(int answerId, ViewModels.Answer answer);
        int DeleteAnswer(int answerId);
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

            return (answer: (answerInDatabase == null) ? null : new ViewModels.Answer(answerInDatabase), validate: null);
        }

        public int EditAnswer(int answerId, ViewModels.Answer answer)
        {
            var answerInDatabase = _context.GetAnswer(answerId);
            answerInDatabase.UpdateFromViewModel(answer);
            return _context.SaveChanges();
        }

        public int DeleteAnswer(int answerId)
        {
            var answer = _context.GetAnswer(answerId);
            if (answer == null)
                return 0;

            answer.IsDeleted = true;
            return _context.SaveChanges();
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
