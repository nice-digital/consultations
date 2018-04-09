using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Comments.Models
{
    public partial class ConsultationsContext : DbContext
    {
        public ConsultationsContext(DbContextOptions options) : base(options)
        {
        }

        public IEnumerable<Location> GetAllCommentsAndQuestionsForDocument(string sourceURI)
        {
            return Location.Where(l => l.SourceURI.Equals(sourceURI))
                            .Include(l => l.Comment)
                            .Include(l => l.Question)
                                .ThenInclude(q => q.QuestionType)
                            .Include(l => l.Question)
                                .ThenInclude(q => q.Answer);

            //return Location.Where(l => l.ConsultationId.Equals(consultationId) &&
            //                           (!l.DocumentId.HasValue || l.DocumentId.Equals(documentId)))
            //                .Include(l => l.Comment)
            //                .Include(l => l.Question)
            //                    .ThenInclude(q => q.QuestionType)
            //                .Include(l => l.Question)
            //                    .ThenInclude(q => q.Answer);
        }

        public Comment GetComment(int commentId)
        {
            return Comment.Where(c => c.CommentId.Equals(commentId))
                    .Include(c => c.Location)
                    .FirstOrDefault();
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
