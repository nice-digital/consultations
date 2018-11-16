using Comments.Common;
using Comments.Configuration;
using Comments.Models;
using Comments.ViewModels;
using NICE.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using NICE.Feeds.Models.Indev.List;

namespace Comments.Services
{
	public interface IConsultationListService
	{
		(ConsultationListViewModel consultationListViewModel, Validate validate)  GetConsultationListViewModel(ConsultationListViewModel model);
	}

	public class ConsultationListService : IConsultationListService
	{
		private readonly ConsultationsContext _context;
		private readonly IFeedService _feedService;
		private readonly IConsultationService _consultationService;
		private readonly IUserService _userService;

		public ConsultationListService(ConsultationsContext consultationsContext, IFeedService feedService, IConsultationService consultationService, IUserService userService)
		{
			_context = consultationsContext;
			_feedService = feedService;
			_consultationService = consultationService;
			_userService = userService;
		}

		public (ConsultationListViewModel consultationListViewModel, Validate validate) GetConsultationListViewModel(ConsultationListViewModel model)
		{
			var validate = _userService.IsAllowedAccess(AppSettings.ConsultationListConfig.DownloadRoles.AllRoles);
			if (validate.Valid)
			{
				var consultationsFromIndev = _feedService.GetConsultationList().ToList();
				var submittedCommentsAndAnswerCounts = _context.GetSubmittedCommentsAndAnswerCounts();
				var consultationListRows = new List<ConsultationListRow>();
				var userRoles = _userService.GetUserRoles().ToList();
				var isAdminUser = userRoles.Any(role => AppSettings.ConsultationListConfig.DownloadRoles.AdminRoles.Contains(role));

				foreach (var consultation in consultationsFromIndev)
				{
					if (isAdminUser || userRoles.Contains(consultation.AllowedRole))
					{
						var sourceURI = ConsultationsUri.CreateConsultationURI(consultation.ConsultationId);
						var responseCount = submittedCommentsAndAnswerCounts.FirstOrDefault(s => s.SourceURI.Equals(sourceURI))?.TotalCount ?? 0;

						var documentAndChapterSlug = _consultationService.GetFirstConvertedDocumentAndChapterSlug(consultation.ConsultationId); //TODO: this might be slow. potentially refactor.

						consultationListRows.Add(
							new ConsultationListRow(consultation.Title,
								consultation.StartDate, consultation.EndDate, responseCount, consultation.ConsultationId,
								documentAndChapterSlug.documentId, documentAndChapterSlug.chapterSlug, consultation.Reference,
								consultation.ProductTypeName));
					}
				}

				model.OptionFilters = GetOptionFilterGroups(model.Status?.ToList(), consultationListRows);
				model.TextFilter = GetTextFilterGroups(model.Keyword, consultationListRows);
				model.Consultations = FilterAndOrderConsultationList(consultationListRows, model.Status, model.Keyword);
			}

			return (model, validate);
		}

		private static IEnumerable<OptionFilterGroup> GetOptionFilterGroups(IList<ConsultationStatus> status, IList<ConsultationListRow> consultationListRows)
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

		private static TextFilterGroup GetTextFilterGroups(string keyword, IList<ConsultationListRow> consultationListRows)
		{
			var textFilter = AppSettings.ConsultationListConfig.TextFilters;
			textFilter.IsSelected = !string.IsNullOrWhiteSpace(keyword);
			var unfilteredCount = consultationListRows.Count;
			textFilter.FilteredResultCount = string.IsNullOrWhiteSpace(keyword) ? unfilteredCount : consultationListRows.Count(c => (c.GidReference.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) != -1) || (c.Title.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) != -1));
			textFilter.UnfilteredResultCount = unfilteredCount;
			return textFilter;
		}

		private static IEnumerable<ConsultationListRow> FilterAndOrderConsultationList(List<ConsultationListRow> consultationListRows, IEnumerable<ConsultationStatus> status, string keyword)
		{
			var statuses = status?.ToList() ?? new List<ConsultationStatus>();
			if (statuses.Any())
			{
				consultationListRows.ForEach(clr => clr.Show = (clr.IsOpen && statuses.Contains(ConsultationStatus.Open)) ||
				                                               (clr.IsClosed && statuses.Contains(ConsultationStatus.Closed)) ||
				                                               (clr.IsUpcoming && statuses.Contains(ConsultationStatus.Upcoming)));
			}

			if (!string.IsNullOrWhiteSpace(keyword))
			{
				consultationListRows.ForEach(c => c.Show = c.Show && (  c.Title.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) != -1 ||
																		c.GidReference.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) != -1));
			}
				
			return consultationListRows.OrderByDescending(c => c.EndDate).ToList(); 
		}
	}
}
