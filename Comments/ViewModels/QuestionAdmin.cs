using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.ViewModels
{
	public class QuestionAdmin
	{
		public string ConsultationTitle{ get; private set; }
		public IEnumerable<BreadcrumbLink> Breadcrumbs { get; private set; }
		public IEnumerable<QuestionAdminDocument> Documents { get; private set; }
		public IEnumerable<Question> ConsultationQuestions { get; private set; }
	}
}
