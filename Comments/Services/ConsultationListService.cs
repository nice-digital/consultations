using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Models;
using Comments.ViewModels;
using DocumentFormat.OpenXml.Wordprocessing;
using NICE.Feeds;
using NICE.Feeds.Models.Indev.List;

namespace Comments.Services
{
	public interface IConsultationListService
	{
		ConsultationListViewModel GetConsultationListViewModel();
	}

	public class ConsultationListService : IConsultationListService
	{
		private readonly ConsultationsContext _context;
		private readonly IFeedService _feedService;

		public ConsultationListService(ConsultationsContext consultationsContext, IFeedService feedService)
		{
			_context = consultationsContext;
			_feedService = feedService;
		}

		public ConsultationListViewModel GetConsultationListViewModel()
		{
			IEnumerable<ConsultationList> consultations = _feedService.GetConsultationList();
			

			foreach (var consultation in consultations)
			{
				var consultationList = new ConsultationListRow(consultation.Title, consultation.StartDate, consultation.EndDate, 0, consultation.ConsultationId);
				CreateConsultationURI(consultation.ConsultationIf)

			}

			var responses = _context.GetAllCommentsAndQuestionsForDocument(sourceURI, true);
			

			return new ConsultationListViewModel(consultationList);
		}
	}
}
