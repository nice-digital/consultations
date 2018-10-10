using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Common;
using Comments.Configuration;
using Comments.Models;
using Comments.ViewModels;
using DocumentFormat.OpenXml.Wordprocessing;
using NICE.Feeds;
using NICE.Feeds.Models.Indev.List;

namespace Comments.Services
{
	public interface IConsultationListService
	{
		ConsultationListViewModel GetConsultationListViewModel(ConsultationListViewModel model);
	}

	public class ConsultationListService : IConsultationListService
	{
		private readonly ConsultationsContext _context;
		private readonly IFeedService _feedService;
		private readonly IConsultationService _consultationService;

		public ConsultationListService(ConsultationsContext consultationsContext, IFeedService feedService, IConsultationService consultationService)
		{
			_context = consultationsContext;
			_feedService = feedService;
			_consultationService = consultationService;
		}

		public ConsultationListViewModel GetConsultationListViewModel(ConsultationListViewModel model)
		{
			var consultations = _feedService.GetConsultationList().ToList();
			var consultationListRows = new List<ConsultationListRow>();

			foreach (var consultation in consultations)
			{
				var sourceURI = ConsultationsUri.CreateConsultationURI(consultation.ConsultationId);
				var responseCount = _context.GetAllSubmittedResponses(sourceURI);
				var documentAndChapterSlug =  _consultationService.GetFirstConvertedDocumentAndChapterSlug(consultation.ConsultationId);
				consultationListRows.Add(
					new ConsultationListRow(consultation.Title,
						consultation.StartDate, consultation.EndDate, responseCount, consultation.ConsultationId,
						documentAndChapterSlug.documentId, documentAndChapterSlug.chapterSlug, consultation.Reference,
						consultation.ConsultationType));
			}
			
			model.OptionFilters = GetOptionFilterGroups(model.Status?.ToList(), consultationListRows);
			model.TextFilters = GetTextFilterGroups(model.Reference, consultationListRows);
			model.Consultations = consultationListRows.OrderByDescending(c => c.EndDate).ToList();
			return model;
		}

		private static IEnumerable<OptionFilterGroup> GetOptionFilterGroups(IList<ConsultationStatus> status, List<ConsultationListRow> consultationListRows)
		{
			var optionFilters = AppSettings.ConsultationListConfig.OptionFilters.ToList();

			var consultationListFilter = optionFilters.Single(f => f.Id.Equals("Status", StringComparison.OrdinalIgnoreCase));
			var openOption = consultationListFilter.Options.Single(o => o.Id.Equals("Open", StringComparison.OrdinalIgnoreCase));
			var closedOption = consultationListFilter.Options.Single(o => o.Id.Equals("Closed", StringComparison.OrdinalIgnoreCase));
			var upcomingOption = consultationListFilter.Options.Single(o => o.Id.Equals("Upcoming", StringComparison.OrdinalIgnoreCase));

			//status - open
			openOption.IsSelected = status != null && status.Contains(ConsultationStatus.Open);
			openOption.UnfilteredResultCount = consultationListRows.Count;
			openOption.FilteredResultCount = consultationListRows.Count(c => c.IsOpen);

			//status - closed
			closedOption.IsSelected = status != null && status.Contains(ConsultationStatus.Closed);
			closedOption.UnfilteredResultCount = consultationListRows.Count;
			closedOption.FilteredResultCount = consultationListRows.Count(c => c.IsClosed);

			//status - upcoming
			upcomingOption.IsSelected = status != null && status.Contains(ConsultationStatus.Upcoming);
			upcomingOption.UnfilteredResultCount = consultationListRows.Count;
			upcomingOption.FilteredResultCount = consultationListRows.Count(c => c.IsUpcoming);
			return optionFilters;
		}

		public List<TextFilterGroup> GetTextFilterGroups(string reference, List<ConsultationListRow> consultationListRows)
		{
			var textFilters = AppSettings.ConsultationListConfig.TextFilters.ToList();
			var referenceFilter = textFilters.Single(f => f.Id.Equals("Reference", StringComparison.OrdinalIgnoreCase));
			referenceFilter.IsSelected = !string.IsNullOrWhiteSpace(reference);
			var unfilteredCount = consultationListRows.Count;
			referenceFilter.FilteredResultCount = string.IsNullOrWhiteSpace(reference) ? unfilteredCount : consultationListRows.Count(c => c.GidReference.IndexOf(reference, StringComparison.OrdinalIgnoreCase) != -1);
			referenceFilter.UnfilteredResultCount = unfilteredCount;
			return textFilters;
		}
		
	}
}
