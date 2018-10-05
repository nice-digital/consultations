using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Models
{
	public class ConsultationListRow
	{
		public ConsultationListRow(string title, DateTime startDate, DateTime endDate, int responses, int consultationId, int? documentId, string chapterSlug)
		{
			Title = title;
			StartDate = startDate;
			EndDate = endDate;
			Responses = responses;
			ConsultationId = consultationId;
			DocumentId = documentId;
			ChapterSlug = chapterSlug;
		}

		public string Title { get; private set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }
		public int Responses { get; private set; }
		public int ConsultationId { get; private set; }
		public int? DocumentId { get; private set; }
		public string ChapterSlug { get; private set; }
	}
}
