using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Common;
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
			var consultations = _feedService.GetConsultationList().ToList();
			var consultationListRows = new List<ConsultationListRow>();

			foreach (var consultation in consultations)
			{
				var sourceURI = ConsultationsUri.CreateConsultationURI(consultation.ConsultationId);
				var responseCount = _context.GetAllSubmittedResponses(sourceURI);
				consultationListRows.Add(new ConsultationListRow(consultation.Title, consultation.StartDate, consultation.EndDate, responseCount, consultation.ConsultationId));
			}
	
			return new ConsultationListViewModel(consultationListRows);
		}
	}
}
