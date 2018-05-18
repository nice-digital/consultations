using Comments.ViewModels;
using Comments.Models;

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
            if (!_currentUser.IsAuthorised)
                return (answer: null, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in accessing answer id:{answerId}"));

            var answerInDatabase = _context.GetAnswer(answerId);

            if (answerInDatabase == null)
                return (answer: null, validate: new Validate(valid: false, notFound: true, message: $"Answer id:{answerId} not found trying to get answer for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));
            
            return (answer: (answerInDatabase == null) ? null : new ViewModels.Answer(answerInDatabase), validate: null);
        }

        public (int rowsUpdated, Validate validate) EditAnswer(int answerId, ViewModels.Answer answer)
        {
            if (!_currentUser.IsAuthorised)
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in editing answer id:{answerId}"));

            var answerInDatabase = _context.GetAnswer(answerId);

            if (answerInDatabase == null)
                return (rowsUpdated: 0, validate: new Validate(valid: false, notFound: true, message: $"Answer id:{answerId} not found trying to edit answer for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));
            
            answerInDatabase.UpdateFromViewModel(answer);

            return (rowsUpdated: _context.SaveChanges(), validate: null);
        }

        public (int rowsUpdated, Validate validate) DeleteAnswer(int answerId)
        {
            if (!_currentUser.IsAuthorised)
                return (rowsUpdated: 0, validate: new Validate(valid: false, unauthorised: true, message: $"Not logged in deleting answer id:{answerId}"));

            var answerInDatabase = _context.GetAnswer(answerId);

            if (answerInDatabase == null)
                return (rowsUpdated: 0, validate: new Validate(valid: false, notFound: true, message: $"Answer id:{answerId} not found trying to delete answer for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));
            
            answerInDatabase.IsDeleted = true;
            return (rowsUpdated: _context.SaveChanges(), validate: null);
        }

        public (ViewModels.Answer answer, Validate validate) CreateAnswer(ViewModels.Answer answer)
        {
            if (!_currentUser.IsAuthorised)
                return (answer: null, validate: new Validate(valid: false, unauthorised: true, message: "Not logged in creating answer"));

            var answerToSave = new Models.Answer(answer.QuestionId, _currentUser.UserId.Value, answer.AnswerText, answer.AnswerBoolean, null);
            _context.Answer.Add(answerToSave);
            _context.SaveChanges();

            return (answer: new ViewModels.Answer(answerToSave), validate: null);
        }
    }
}
