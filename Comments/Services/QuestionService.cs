using System.Security.Authentication;
using Comments.Models;
using Comments.ViewModels;

namespace Comments.Services
{
	public interface IQuestionService
    {
        (ViewModels.Question question, Validate validate) GetQuestion(int questionId);
        (int rowsUpdated, Validate validate) EditQuestion(int questionId, ViewModels.Question question);
        (int rowsUpdated, Validate validate) DeleteQuestion(int questionId);
        (ViewModels.Question question, Validate validate) CreateQuestion(ViewModels.Question question);
	    int InsertQuestionsForAdmin(int consultationId);

    }
    public class QuestionService : IQuestionService
    {
        private readonly ConsultationsContext _context;
        private readonly IUserService _userService;
        private readonly User _currentUser;

        public QuestionService(ConsultationsContext consultationsContext, IUserService userService)
        {
            _context = consultationsContext;
            _userService = userService;
            _currentUser = _userService.GetCurrentUser();
        }

        public (ViewModels.Question question, Validate validate) GetQuestion(int questionId)
        {
            var questionInDatabase = _context.GetQuestion(questionId);

            if (questionInDatabase == null)
                return (question: null, validate: new Validate(valid: false, notFound: true, message: $"Question id:{questionId} not found trying to get question"));

            return (question: (questionInDatabase == null) ? null : new ViewModels.Question(questionInDatabase.Location, questionInDatabase), validate: null);
        }

        public (int rowsUpdated, Validate validate) EditQuestion(int questionId, ViewModels.Question question)
        {
            if (!_currentUser.IsAuthorised)
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in editing question id:{questionId}"));

            var questionInDatabase = _context.GetQuestion(questionId);

            if (questionInDatabase == null)
                return (rowsUpdated: 0, validate: new Validate(valid: false, notFound: true, message: $"Question id:{questionId} not found trying to edit question for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));

            questionInDatabase.UpdateFromViewModel(question);
            return (rowsUpdated: _context.SaveChanges(), validate: null);
        }

        public (int rowsUpdated, Validate validate) DeleteQuestion(int questionId)
        {
            if (!_currentUser.IsAuthorised)
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in deleting question id:{questionId}"));

            var questionInDatabase = _context.GetQuestion(questionId);

            if (questionInDatabase == null)
                return (rowsUpdated: 0, validate: new Validate(valid: false, notFound: true, message: $"Question id:{questionId} not found trying to delete question"));
            
            questionInDatabase.IsDeleted = true;
            return (rowsUpdated: _context.SaveChanges(), validate: null);
        }

        public (ViewModels.Question question, Validate validate) CreateQuestion(ViewModels.Question question) 
        {
            if (!_currentUser.IsAuthorised)
                return (question: null, validate: new Validate(valid: false, unauthorised: true, message: "Not logged in creating question"));

            var locationToSave = new Models.Location(question as ViewModels.Location);
            _context.Location.Add(locationToSave);

            var questionTypeToSave = new Models.QuestionType(question.QuestionType.Description, question.QuestionType.HasTextAnswer, question.QuestionType.HasBooleanAnswer, null);
            var questionToSave = new Models.Question(question.LocationId, question.QuestionText, question.QuestionTypeId, question.QuestionOrder, locationToSave, questionTypeToSave, null);

            _context.Question.Add(questionToSave);
            _context.SaveChanges();
            
            return (question: new ViewModels.Question(locationToSave, questionToSave), validate: null);
        }

	    public int InsertQuestionsForAdmin(int consultationId)
	    {
			if (!_currentUser.IsAuthorised)
				throw new AuthenticationException();

		    return _context.InsertQuestionsWithScript(consultationId);
	    }
	}
}
