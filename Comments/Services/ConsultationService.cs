using Comments.Models;
using Comments.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Comments.Services
{
    public interface IConsultationService
    {
        DocumentViewModel GetAllCommentsAndQuestionsForDocument(int consultationId, int documentId);
    }

    public class ConsultationService : IConsultationService
    {
        private readonly ConsultationsContext _context;

        public ConsultationService(ConsultationsContext consultationsContext)
        {
            _context = consultationsContext;
        }

        public DocumentViewModel GetAllCommentsAndQuestionsForDocument(int consultationId, int documentId)
        {
            var title = "todo: title (and a bunch of other data) comes from the deserialised indev consultation feed";
            var consultation = new Consultation(consultationId, title, null);


            var locations = _context.GetAllCommentsAndQuestionsForDocument(consultationId, documentId);

            var commentsData = new List<ViewModels.Comment>();
            var questionsData = new List<ViewModels.Question>();
            foreach (var location in locations)
            {
                commentsData.AddRange(location.Comment.Select(comment => new ViewModels.Comment(location, comment)));
                questionsData.AddRange(location.Question.Select(question => new ViewModels.Question(location, question)));
            }

            return new DocumentViewModel(consultation, commentsData, questionsData);
        }
    }
}