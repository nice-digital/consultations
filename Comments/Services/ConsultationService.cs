using System.Linq;
using Comments.Models;
using Comments.ViewModels;

namespace Comments.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly ConsultationsContext _consultationsContext;

        public ConsultationService(ConsultationsContext consultationsContext)
        {
            _consultationsContext = consultationsContext;
        }

        public DocumentViewModel GetAllComments()
        {
            return new DocumentViewModel(_consultationsContext.Comment.ToList());
        }
    }

    public interface IConsultationService
    {
        DocumentViewModel GetAllComments();
    }
}
