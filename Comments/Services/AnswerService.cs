using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.ViewModels;
using Comments.Models;

namespace Comments.Services
{
    public interface IAnswerService
    {
        ViewModels.Answer GetAnswer(int answerId);
        int EditAnswer(int answerId, ViewModels.Answer answer);
        int DeleteAnswer(int answerId);
        ViewModels.Answer CreateAnswer(ViewModels.Answer answer, int questionId);
    }
    public class AnswerService : IAnswerService
    {
        private readonly ConsultationsContext _context;
        public AnswerService(ConsultationsContext consultationsContext)
        {
            _context = consultationsContext;
        }
        public ViewModels.Answer GetAnswer(int answerId)
        {
            var answer = _context.GetAnswer(answerId);
            return (answer == null) ? null : new ViewModels.Answer(answer);
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

        public ViewModels.Answer CreateAnswer(ViewModels.Answer answer, int questionId)
        {
            var currentlyLoggedOnUserId = Guid.NewGuid();
            var answerToSave = new Models.Answer(questionId, currentlyLoggedOnUserId,answer.AnswerText, answer.AnswerBoolean, null);

            _context.Answer.Add(answerToSave);
            _context.SaveChanges();
            return new ViewModels.Answer(answerToSave);
        }
    }
}
