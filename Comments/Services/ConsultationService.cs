using Comments.Models;
using Comments.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Comments.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly ConsultationsContext _context;

        public ConsultationService(ConsultationsContext consultationsContext)
        {
            _context = consultationsContext;
        }

        public DocumentViewModel GetAllCommentsAndQuestionsForDocument(Guid consultationId, Guid documentId)
        {
            var title = "todo: title (and a bunch of other data) comes from the indev consultation feed";

            var consultationsData = _context.Location.Where(l => l.ConsultationId.Equals(consultationId) &&
                                                     (!l.DocumentId.HasValue || l.DocumentId.Equals(documentId)))
                                        .Include(l => l.Comment)
                                        .Include(l => l.Question)
                                            .ThenInclude(q => q.QuestionType)
                                        .Include(l => l.Question)
                                            .ThenInclude(q => q.Answer)
                                        .ToList();

            return new DocumentViewModel(title, consultationsData);
        }
    }

    public interface IConsultationService
    {
        DocumentViewModel GetAllCommentsAndQuestionsForDocument(Guid consultationId, Guid documentId);
    }
}
