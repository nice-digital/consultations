using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Comments.Services;

namespace Comments.Models
{
    public partial class ConsultationsContext : DbContext
    {
        public ConsultationsContext(DbContextOptions options, IUserService userService) : base(options)
        {
            _userService = userService;
            _createdByUserID = _userService.GetCurrentUser().UserId;
        }

        /// <summary>
        /// It's not obvious from this code, but this it actually filtering on more than you think. There's global filters defined in the context, specifically
        /// for the IsDeleted flag and the CreatedByUserId. So, this is only going to return data that isn't deleted and belongs to the current user.
        /// This behaviour can be overridden with the IgnoreQueryFilters command. See the ConsultationContext.Tests for example usage.
        /// </summary>
        /// <param name="sourceURI"></param>
        /// <returns></returns>
        public IEnumerable<Location> GetAllCommentsAndQuestionsForDocument(string sourceURI)
        {

            if (!_userService.GetCurrentUser().IsLoggedIn)
                throw new Exception("trying to return comments and questions when not logged in. it shouldn't have gone this far.");

            var data = Location.Where(l => l.SourceURI.Equals(sourceURI))
                        .Include(l => l.Comment)
                        .Include(l => l.Question)
                            .ThenInclude(q => q.QuestionType)
                        .Include(l => l.Question)
                            .ThenInclude(q => q.Answer);

            return data;
        }

        public Comment GetComment(int commentId)
        {
            var comment = Comment.Where(c => c.CommentId.Equals(commentId))
                            .Include(c => c.Location)
                            .FirstOrDefault();

            return comment;
        }
    }
}
