using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.ViewModels;
using DocumentFormat.OpenXml.Wordprocessing;
using NICE.Feeds;

namespace Comments.Services
{
	public class ConsultationListService
	{
		private readonly IFeedService _feedService;

		public ConsultationListService(IFeedService feedService)
		{
			_feedService = feedService;
		}
		public ConsultationListViewModel GetConsultationListViewModel()
		{
			var consultations = _feedService.GetConsultationList();
			return null;
		}
	}
}
