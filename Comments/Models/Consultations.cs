using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Models
{
	public class Consultations
	{
		public Consultations(string title, DateTime startDate, DateTime endDate, int responses, int consultationId)
		{
			Title = title;
			StartDate = startDate;
			EndDate = endDate;
			Responses = responses;
			ConsultationId = consultationId;
		}

		public string Title { get; private set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }
		public int Responses { get; private set; }
		public int ConsultationId { get; private set; }
	}
}
