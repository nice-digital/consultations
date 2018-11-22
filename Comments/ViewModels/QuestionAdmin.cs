using System.Collections.Generic;

namespace Comments.ViewModels
{
	public class QuestionAdmin
	{
		public QuestionAdmin(string consultationTitle, IEnumerable<QuestionAdminDocument> documents, IEnumerable<Question> consultationQuestions)
		{
			ConsultationTitle = consultationTitle;
			Documents = documents;
			ConsultationQuestions = consultationQuestions;
		}

		public string ConsultationTitle{ get; private set; }
		//public IEnumerable<BreadcrumbLink> Breadcrumbs { get; private set; }
		public IEnumerable<QuestionAdminDocument> Documents { get; private set; }
		public IEnumerable<Question> ConsultationQuestions { get; private set; }
	}
}
