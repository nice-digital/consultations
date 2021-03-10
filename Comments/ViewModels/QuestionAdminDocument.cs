using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
	public class QuestionAdminDocument
    {
		public QuestionAdminDocument(int documentId, string title, IEnumerable<Question> documentQuestions)
		{
			DocumentId = documentId;
			//SupportsQuestions = supportsQuestions;
			Title = title;
			DocumentQuestions = documentQuestions;
		}

		public int DocumentId { get; private set; }
	    //public bool SupportsQuestions { get; private set; }
		public string Title { get; private set; }
	    public IEnumerable<Question> DocumentQuestions { get; set; }

	    public int DocumentQuestionCount => DocumentQuestions.Count();
    }
}
