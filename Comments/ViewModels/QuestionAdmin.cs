using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
	public class QuestionAdmin
	{
		public QuestionAdmin(string consultationTitle, bool consultationSupportsQuestions, IEnumerable<Question> consultationQuestions, IEnumerable<QuestionAdminDocument> documents)
		{
			ConsultationTitle = consultationTitle;
			ConsultationSupportsQuestions = consultationSupportsQuestions;
			ConsultationQuestions = consultationQuestions;
			Documents = documents;
		}

		public string ConsultationTitle{ get; private set; }
		public bool ConsultationSupportsQuestions { get; private set; }

		public IEnumerable<Question> ConsultationQuestions { get; private set; }
		public int ConsultationQuestionsCount => ConsultationQuestions.Count();

		//public IEnumerable<BreadcrumbLink> Breadcrumbs { get; private set; }
		public IEnumerable<QuestionAdminDocument> Documents { get; private set; }
		
	}
}
