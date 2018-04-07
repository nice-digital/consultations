using Comments.Models;
using Comments.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Comment = Comments.ViewModels.Comment;
using Question = Comments.ViewModels.Question;

namespace Comments.Services
{
    public interface ICommentService
    {
        CommentsAndQuestions GetCommentsAndQuestions(string sourceURI);
        ViewModels.Comment GetComment(int commentId);
        int EditComment(int commentId, ViewModels.Comment comment);
        ViewModels.Comment CreateComment(ViewModels.Comment comment);
        int DeleteComment(int commentId);
    }

    public class CommentService : ICommentService
    {
        private readonly ConsultationsContext _context;
        private readonly IUserService _userService;

        public CommentService(ConsultationsContext consultationsContext, IUserService userService)
        {
            _context = consultationsContext;
            _userService = userService;
        }

        public ViewModels.Comment GetComment(int commentId)
        {
            var comment = _context.GetComment(commentId);
            return (comment == null) ? null : new ViewModels.Comment(comment.Location, comment); 
        }

        public int EditComment(int commentId, ViewModels.Comment comment)
        {
            var commentInDatabase = _context.GetComment(commentId);
            commentInDatabase.UpdateFromViewModel(comment);
            return _context.SaveChanges();
        }

        public ViewModels.Comment CreateComment(ViewModels.Comment comment)
        {
            var currentlyLoggedOnUserId = Guid.NewGuid();

            var locationToSave = new Models.Location(comment as ViewModels.Location);
            _context.Location.Add(locationToSave);
            
            var commentToSave = new Models.Comment(comment.LocationId, currentlyLoggedOnUserId, comment.CommentText, comment.LastModifiedByUserId, locationToSave);
            _context.Comment.Add(commentToSave);
            _context.SaveChanges();

            return new ViewModels.Comment(locationToSave, commentToSave);
        }

        public int DeleteComment(int commentId)
        {
            var comment = _context.GetComment(commentId);
            if (comment == null)
                return 0;

            comment.IsDeleted = true;
            return _context.SaveChanges();
        }

        public CommentsAndQuestions GetCommentsAndQuestions(string sourceURI)
        {
            var user = _userService.GetCurrentUser();

            if (!user.IsLoggedIn) 
                return new CommentsAndQuestions(new List<Comment>(), new List<Question>(), user.IsLoggedIn);

            var locations = _context.GetAllCommentsAndQuestionsForDocument(sourceURI);

            var commentsData = new List<ViewModels.Comment>();
            var questionsData = new List<ViewModels.Question>();
            foreach (var location in locations)
            {
                commentsData.AddRange(location.Comment.Select(comment => new ViewModels.Comment(location, comment)));
                questionsData.AddRange(location.Question.Select(question => new ViewModels.Question(location, question)));
            }

            return new CommentsAndQuestions(commentsData, questionsData, user.IsLoggedIn);
        }
    }
}