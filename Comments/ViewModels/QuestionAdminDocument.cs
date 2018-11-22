using System.Collections.Generic;
using System.Linq;
using Comments.Common;
using Newtonsoft.Json;
using NICE.Feeds.Models.Indev;
using NICE.Feeds.Models.Indev.Detail;

namespace Comments.ViewModels
{
    public class QuestionAdminDocument
    {
		public QuestionAdminDocument(int documentId, bool supportsQuestions, string title, IEnumerable<Question> documentQuestions)
		{
			DocumentId = documentId;
			SupportsQuestions = supportsQuestions;
			Title = title;
			DocumentQuestions = documentQuestions;
		}

		public int DocumentId { get; private set; }
	    public bool SupportsQuestions { get; private set; }
		public string Title { get; private set; }
	    public IEnumerable<Question> DocumentQuestions { get; set; }

	    public int DocumentQuestionCount => DocumentQuestions.Count();
    }
}
