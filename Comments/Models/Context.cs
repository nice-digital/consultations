using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Comments.Models
{
    public partial class ConsultationsContext : DbContext
    {
        public ConsultationsContext(DbContextOptions options) : base(options)
        {
        }

        public IEnumerable<Location> GetAllCommentsAndQuestionsForDocument(int consultationId, int documentId)
        {
            return Location.Where(l => l.ConsultationId.Equals(consultationId) &&
                                       (!l.DocumentId.HasValue || l.DocumentId.Equals(documentId)))
                                            .Include(l => l.Comment)
                                            .Include(l => l.Question)
                                                .ThenInclude(q => q.QuestionType)
                                            .Include(l => l.Question)
                                                .ThenInclude(q => q.Answer)
                                            .ToList();

        }
    }
}
