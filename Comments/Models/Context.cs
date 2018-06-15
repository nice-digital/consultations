using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Comments.Services;
using Comments.ViewModels;
using Microsoft.Extensions.Configuration;

namespace Comments.Models
{
    public partial class ConsultationsContext : DbContext
    {
	    private readonly IConfiguration _configuration;

		//these commented out constructors are just here for use when creating scaffolding with EF core. without them it won't work.
		//don't leave them in uncommented though. and don't set that connection string to a valid value and commit it.
		//public ConsultationsContext(DbContextOptions options)
		//	: base(options) {
		//    _createdByUserID = Guid.Empty;
		//}
	 //   public ConsultationsContext() : base()
	 //   {
		//    _createdByUserID = Guid.Empty;
	 //   }
	 //   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	 //   {
		//	optionsBuilder.UseSqlServer("[snip]");
		//}


		public ConsultationsContext(DbContextOptions options, IUserService userService) : base(options)
        {
	        _userService = userService;
            _createdByUserID = _userService.GetCurrentUser().UserId;
        }

        /// <summary>
        /// It's not obvious from this code, but this it actually filtering on more than it looks like. There's global filters defined in the context, specifically
        /// for the IsDeleted flag and the CreatedByUserId. So, this is only going to return data that isn't deleted and belongs to the current user.
        /// This behaviour can be overridden with the IgnoreQueryFilters command. See the ConsultationContext.Tests for example usage.
        /// </summary>
        /// <param name="sourceURIs"></param>
        /// <returns></returns>
        public IEnumerable<Location> GetAllCommentsAndQuestionsForDocument(IEnumerable<string> sourceURIs)
        {

            if (!_userService.GetCurrentUser().IsAuthorised)
                throw new Exception("trying to return comments and questions when not logged in. this should have been trapped in the service.");
            
            var data = Location.Where(l => sourceURIs.Contains(l.SourceURI))
                .Include(l => l.Comment)
                .Include(l => l.Question)
                .ThenInclude(q => q.QuestionType)
                .Include(l => l.Question)
                .ThenInclude(q => q.Answer)
                .OrderByDescending(l => l.Comment
                    .OrderByDescending(c => c.LastModifiedDate).Select(c => c.LastModifiedDate).FirstOrDefault());

            return data;
        }

        public IEnumerable<Location> GetAllCommentsAndQuestionsForConsultation(string sourceURI)
        {
            if (!_userService.GetCurrentUser().IsAuthorised)
                throw new Exception("trying to return comments and questions when not logged in. this should have been trapped in the service.");

            var data = Location.Where(l => l.SourceURI.Contains(sourceURI))
                .Include(l => l.Comment)
                .Include(l => l.Question)
                .ThenInclude(q => q.QuestionType)
                .Include(l => l.Question)
                .ThenInclude(q => q.Answer)
                .OrderByDescending(l => l.Comment
                    .OrderByDescending(c => c.LastModifiedDate).Select(c => c.LastModifiedDate).FirstOrDefault());

            return data;
        }

        public Comment GetComment(int commentId)
        {
            var comment = Comment.Where(c => c.CommentId.Equals(commentId))
                            .Include(c => c.Location)
                            .FirstOrDefault();

            return comment;
        }

        public Answer GetAnswer(int answerId)
        {
            return Answer
                .FirstOrDefault(a => a.AnswerId.Equals(answerId));
        }

        public Question GetQuestion(int questionId)
        {
            return Question.Where(q => q.QuestionId.Equals(questionId))
                .Include(q => q.Location)
                .Include(q => q.QuestionType)
                .FirstOrDefault();
        }
	}
}
