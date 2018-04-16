using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Models;
using Comments.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SQLitePCL;

namespace Comments.Services
{
    public interface IQuestionService
    {
        (ViewModels.Question question, Validate validate) GetQuestion(int questionId);
        int EditQuestion(int questionId, ViewModels.Question question);
        (int rowsUpdated, Validate validate) DeleteQuestion(int questionId);
        ViewModels.Question CreateQuestion(ViewModels.Question question);
    }
    public class QuestionService : IQuestionService
    {
        private readonly ConsultationsContext _context;
        private readonly IUserService _userService;

        public QuestionService(ConsultationsContext consultationsContext, IUserService userService)
        {
            _context = consultationsContext;
            _userService = userService;
        }

        public (ViewModels.Question question, Validate validate) GetQuestion(int questionId)
        {
            var questionInDatabase = _context.GetQuestion(questionId);

            if (questionInDatabase == null)
                return (question: null, validate: new Validate(valid: false, notFound: true, message: $"Question id:{questionId} not found trying to get question"));


            return (question: (questionInDatabase == null) ? null : new ViewModels.Question(questionInDatabase.Location, questionInDatabase), validate: null);
        }

        public int EditQuestion(int questionId, ViewModels.Question question)
        {
            var questionInDatabase = _context.GetQuestion(questionId);
            questionInDatabase.UpdateFromViewModel(question);
            return _context.SaveChanges();
        }

        public (int rowsUpdated, Validate validate) DeleteQuestion(int questionId)
        {
            var questionInDatabase = _context.GetQuestion(questionId);

            if (questionInDatabase == null)
                return (rowsUpdated: 0, validate: new Validate(valid: false, notFound: true, message: $"Question id:{questionId} not found trying to delete question"));
            
            questionInDatabase.IsDeleted = true;
            return (rowsUpdated: _context.SaveChanges(), validate: null);
        }

        public ViewModels.Question CreateQuestion(ViewModels.Question question) 
        {
            var locationToSave = new Models.Location(question as ViewModels.Location);
            _context.Location.Add(locationToSave);

            var questionTypeToSave = new Models.QuestionType(question.QuestionType.Description, question.QuestionType.HasTextAnswer, question.QuestionType.HasBooleanAnswer, null);
            var questionToSave = new Models.Question(question.LocationId, question.QuestionText, question.QuestionTypeId, question.QuestionOrder, locationToSave, questionTypeToSave, null);

            _context.Question.Add(questionToSave);
            _context.SaveChanges();
            return new ViewModels.Question(locationToSave, questionToSave);
        }
    }
}
