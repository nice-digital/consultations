using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SQLitePCL;

namespace Comments.Services
{
    public interface IQuestionService
    {
        ViewModels.Question GetQuestion(int questionId);
        int EditQuestion(int questionId, ViewModels.Question question);
        int DeleteQuestion(int questionId);
        ViewModels.Question CreateQuestion(ViewModels.Question question, int questionTypeId);
    }
    public class QuestionService : IQuestionService
    {
        private readonly ConsultationsContext _context;

        private readonly IUserService _userService;
        private readonly IDbContextOptions _options;
        private readonly IDbContextOptionsBuilderInfrastructure _builderInfrastructure;
        public QuestionService(ConsultationsContext consultationsContext, IUserService userService, IDbContextOptions options, IDbContextOptionsBuilderInfrastructure builderInfrastructure)
        {
            _context = consultationsContext;
            _userService = userService;
            _options = options;
            _builderInfrastructure = builderInfrastructure;
        }

        public ViewModels.Question GetQuestion(int questionId)
        {
            var question = _context.GetQuestion(questionId);
            return (question == null) ? null : new ViewModels.Question(question.Location, question);
        }

        public int EditQuestion(int questionId, ViewModels.Question question)
        {
            var questionInDatabase = _context.GetQuestion(questionId);
            questionInDatabase.UpdateFromViewModel(question);
            return _context.SaveChanges();
        }

        public int DeleteQuestion(int questionId)
        {
            var question = _context.GetQuestion(questionId);
            if (question == null)
                return 0;

            question.IsDeleted = true;
            return _context.SaveChanges();
        }

        public ViewModels.Question CreateQuestion(ViewModels.Question question, int questionTypeId) //TODO: Where does questionTypeId come from?
        {
            var locationToSave = new Models.Location(question as ViewModels.Location);
            _context.Location.Add(locationToSave);

            var questionTypeToSave = new Models.QuestionType(question.QuestionType.Description, question.QuestionType.HasTextAnswer, question.QuestionType.HasBooleanAnswer, null);
            var questionToSave = new Models.Question(question.LocationId, question.QuestionText, questionTypeId, question.QuestionOrder, locationToSave, questionTypeToSave, null);

            _context.Question.Add(questionToSave);
            _context.SaveChanges();
            return new ViewModels.Question(locationToSave, questionToSave);
        }
    }
}
