using Comments.Common;
using Comments.Configuration;
using Comments.Models;
using Comments.ViewModels;
using NICE.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;

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

		/// <summary>
		/// returns the model to be used by the download page.
		///
		/// the page supports being used by regular signed in users (i.e. commenters)
		///
		/// also administrators - those who have a role in AppSettings.ConsultationListConfig.DownloadRoles.AdminRoles
		///
		/// also "Team users". these are people who don't have an administrator role, but do have one from the list available in AppSettings.ConsultationListConfig.DownloadRoles.TeamRoles
		///
		/// only admins + team users will be able to see the number of comments/answers submitted and be able to download them.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public (ConsultationListViewModel consultationListViewModel, Validate validate) GetConsultationListViewModel(ConsultationListViewModel model)
		{
			var currentUser = _userService.GetCurrentUser();
			if (!currentUser.IsAuthorised)
				return (model, new Validate(valid: false, unauthorised: true));


			var userRoles = _userService.GetUserRoles().ToList();
			var isAdminUser = userRoles.Any(role => AppSettings.ConsultationListConfig.DownloadRoles.AdminRoles.Contains(role));
			var teamRoles = userRoles.Where(role => AppSettings.ConsultationListConfig.DownloadRoles.TeamRoles.Contains(role)).Select(role => role).ToList();
			var isTeamUser = !isAdminUser && teamRoles.Any(); //an admin with team roles is still just considered an admin.
			var hasAccessToViewUpcomingConsultations = isAdminUser || isTeamUser;

			var canSeeAnySubmissionCounts = isAdminUser || isTeamUser;

			var consultationsFromIndev = _feedService.GetConsultationList().ToList();
			var submittedCommentsAndAnswerCounts = canSeeAnySubmissionCounts ? _context.GetSubmittedCommentsAndAnswerCounts() : null;
			var sourceURIsCommentedOrAnswered = _context.GetAllSourceURIsTheCurrentUserHasCommentedOrAnsweredAQuestion();

			if (model.InitialPageView) //set defaults here for the filters - this code might be called by the SSR or by the client-side.
			{
				//if unset (by querystring) set the default for the contribution filter. - the filter should be enabled (HasContributed), for regular users (i.e. not admins or team users) and the user has contributed to any.
				if (model.Contribution == null)
				{
					var userHasCommentedOrAnswered = sourceURIsCommentedOrAnswered.Any();
					if ((!isAdminUser && !isTeamUser) && userHasCommentedOrAnswered)
					{
						model.Contribution = new List<ContributionStatus> {ContributionStatus.HasContributed};
					}
				}

				//if unset (by querystring) set the default for the team filter - only for team users. if a user has admin and team rights, then admin overrules team and the filter will be unset.
				if (model.Team == null && isTeamUser)
				{
					model.Team = new List<TeamStatus> { TeamStatus.MyTeam };
				}
			}

			var allOrganisationCodes = GetConsultationCodesForAllConsultations(consultationsFromIndev.Select(c => c.ConsultationId).ToList());

			var consultationListRows = new List<ConsultationListRow>();

			foreach (var consultation in consultationsFromIndev)
			{
				var sourceURI = ConsultationsUri.CreateConsultationURI(consultation.ConsultationId);

				var (hasCurrentUserEnteredCommentsOrAnsweredQuestions, hasCurrentUserSubmittedCommentsOrAnswers)
					= GetFlagsForWhetherTheCurrentUserHasCommentedOrAnsweredThisConsultation(sourceURIsCommentedOrAnswered, sourceURI);

				var canSeeSubmissionCountForThisConsultation = (isAdminUser || (isTeamUser && teamRoles.Contains(consultation.AllowedRole)));

				var responseCount = canSeeSubmissionCountForThisConsultation ? submittedCommentsAndAnswerCounts.FirstOrDefault(s => s.SourceURI.Equals(sourceURI))?.TotalCount ?? 0 : (int?)null;

				consultationListRows.Add(
					new ConsultationListRow(consultation.Title,
						consultation.StartDate, consultation.EndDate, responseCount, consultation.ConsultationId,
						consultation.FirstConvertedDocumentId, consultation.FirstChapterSlugOfFirstConvertedDocument, consultation.Reference,
						consultation.ProductTypeName, hasCurrentUserEnteredCommentsOrAnsweredQuestions, hasCurrentUserSubmittedCommentsOrAnswers, consultation.AllowedRole, allOrganisationCodes[consultation.ConsultationId]));
			}

			model.OptionFilters = GetOptionFilterGroups(model.Status?.ToList(), consultationListRows, hasAccessToViewUpcomingConsultations);
			model.TextFilter = GetTextFilterGroups(model.Keyword, consultationListRows);
			model.ContributionFilter = GetContributionFilter(model.Contribution?.ToList(), consultationListRows);
			model.TeamFilter = isTeamUser ? GetTeamFilter(model.Team?.ToList(), consultationListRows, teamRoles) : null;
			model.Consultations = FilterAndOrderConsultationList(consultationListRows, model.Status, model.Keyword, model.Contribution, model.Team, (isTeamUser ? teamRoles : null), hasAccessToViewUpcomingConsultations);
			model.User = new DownloadUser(isAdminUser, isTeamUser, _userService.GetCurrentUser(), teamRoles);

			return (model, new Validate(valid: true));
		}

		private (bool hasEnteredCommentsOrAnsweredQuestions, bool hasSubmittedCommentsOrAnswers)
			GetFlagsForWhetherTheCurrentUserHasCommentedOrAnsweredThisConsultation(IEnumerable<KeyValuePair<string, Models.Status>> sourceURIsCommentedOrAnswered, string consultationSourceURI)
		{
			var foundCommentOrAnswer = sourceURIsCommentedOrAnswered.FirstOrDefault(s =>
				s.Key.Equals(consultationSourceURI) || s.Key.StartsWith(consultationSourceURI + "/"));

			if (foundCommentOrAnswer.Equals(default(KeyValuePair<string, Models.Status>)))
				return (hasEnteredCommentsOrAnsweredQuestions: false, hasSubmittedCommentsOrAnswers: false);

			if (foundCommentOrAnswer.Value.StatusId.Equals((int)StatusName.Submitted))
			{
				return (hasEnteredCommentsOrAnsweredQuestions: true, hasSubmittedCommentsOrAnswers: true);
			}

			return (hasEnteredCommentsOrAnsweredQuestions: true, hasSubmittedCommentsOrAnswers: false);
		}


		private static IEnumerable<OptionFilterGroup> GetOptionFilterGroups(IList<ConsultationStatus> status, IList<ConsultationListRow> consultationListRows, bool hasAccessToViewUpcomingConsultations)
		{
			var defaultOptionFilter = AppSettings.ConsultationListConfig.OptionFilters.Single(); //there should always just be 1 optionfilter. if there's not, throwing an exception is valid.
			var optionFilters = new List<OptionFilterGroup> { new OptionFilterGroup(defaultOptionFilter) };

			var consultationListFilter = optionFilters.Single(f => f.Id.Equals("Status", StringComparison.OrdinalIgnoreCase));
			
			//status - open
			var openOption = consultationListFilter.Options.Single(o => o.Id.Equals("Open", StringComparison.OrdinalIgnoreCase));
			openOption.IsSelected = status != null && status.Contains(ConsultationStatus.Open);
			openOption.UnfilteredResultCount = consultationListRows.Count;
			openOption.FilteredResultCount = consultationListRows.Count(c => c.IsOpen);

			//status - closed
			var closedOption = consultationListFilter.Options.Single(o => o.Id.Equals("Closed", StringComparison.OrdinalIgnoreCase));
			closedOption.IsSelected = status != null && status.Contains(ConsultationStatus.Closed);
			closedOption.UnfilteredResultCount = consultationListRows.Count;
			closedOption.FilteredResultCount = consultationListRows.Count(c => c.IsClosed);

			//status - upcoming
			var upcomingOption = consultationListFilter.Options.SingleOrDefault(o => o.Id.Equals("Upcoming", StringComparison.OrdinalIgnoreCase));
			if (upcomingOption != null)
			{
				if (hasAccessToViewUpcomingConsultations)
				{
					upcomingOption.IsSelected = status != null && status.Contains(ConsultationStatus.Upcoming);
					upcomingOption.UnfilteredResultCount = consultationListRows.Count;
					upcomingOption.FilteredResultCount = consultationListRows.Count(c => c.IsUpcoming);
				}
				else
				{
					consultationListFilter.Options.Remove(upcomingOption);
				}
			}

			return optionFilters;
		}

		private IEnumerable<OptionFilterGroup> GetContributionFilter(List<ContributionStatus> contribution, IList<ConsultationListRow> consultationListRows)
		{
			var contributionFilter = AppSettings.ConsultationListConfig.ContributionFilter.ToList();

			var hasContributedOption = contributionFilter
				.Single(option => option.Id.Equals("Contribution", StringComparison.OrdinalIgnoreCase))
				.Options
				.Single(option => option.Id.Equals("HasContributed", StringComparison.OrdinalIgnoreCase));

			hasContributedOption.IsSelected = contribution != null && contribution.Contains(ContributionStatus.HasContributed);
			hasContributedOption.UnfilteredResultCount = consultationListRows.Count;
			hasContributedOption.FilteredResultCount = consultationListRows.Count(c => c.HasCurrentUserEnteredCommentsOrAnsweredQuestions);

			return contributionFilter;
		}

		private IEnumerable<OptionFilterGroup> GetTeamFilter(List<TeamStatus> team, IList<ConsultationListRow> consultationListRows, List<string> currentUsersTeamRoles)
		{
			var teamFilters = AppSettings.ConsultationListConfig.TeamFilter.ToList();

			var myTeamOption = teamFilters
				.Single(option => option.Id.Equals("Team", StringComparison.OrdinalIgnoreCase))
				.Options
				.Single(option => option.Id.Equals("MyTeam", StringComparison.OrdinalIgnoreCase));

			myTeamOption.IsSelected = team != null && team.Contains(TeamStatus.MyTeam);
			myTeamOption.UnfilteredResultCount = consultationListRows.Count;
			if (currentUsersTeamRoles != null && currentUsersTeamRoles.Any())
			{
				myTeamOption.FilteredResultCount = consultationListRows.Count(c => currentUsersTeamRoles.Contains(c.AllowedRole, StringComparer.OrdinalIgnoreCase));
			}
			else //it shouldn't be shown on the front end if there's no team filter
			{
				myTeamOption.FilteredResultCount = consultationListRows.Count;
			}
			return teamFilters;
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

		private static IEnumerable<ConsultationListRow> FilterAndOrderConsultationList(List<ConsultationListRow> consultationListRows, IEnumerable<ConsultationStatus> status, string keyword,
			IEnumerable<ContributionStatus> contribution, IEnumerable<TeamStatus> team, List<string> currentUsersTeamRoles, bool hasAccessToViewUpcomingConsultations)
		{
			if (!hasAccessToViewUpcomingConsultations)
			{
				consultationListRows.RemoveAll(clr => clr.IsUpcoming);
			}

			var statuses = status?.ToList() ?? new List<ConsultationStatus>();
			if (statuses.Any())
			{
				consultationListRows.ForEach(clr => clr.Show = (clr.IsOpen && statuses.Contains(ConsultationStatus.Open)) ||
				                                               (clr.IsClosed && statuses.Contains(ConsultationStatus.Closed)) ||
				                                               (clr.IsUpcoming && statuses.Contains(ConsultationStatus.Upcoming) && hasAccessToViewUpcomingConsultations));
			}

			if (!string.IsNullOrWhiteSpace(keyword))
			{
				consultationListRows.ForEach(c => c.Show = c.Show && (  c.Title.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) != -1 ||
																		c.GidReference.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) != -1));
			}

			var contributions = contribution?.ToList() ?? new List<ContributionStatus>();
			if (contributions.Any())
			{
				consultationListRows.ForEach(c => c.Show = c.Show && c.HasCurrentUserEnteredCommentsOrAnsweredQuestions);
			}

			if (currentUsersTeamRoles != null && currentUsersTeamRoles.Any())
			{
				var teams = team?.ToList() ?? new List<TeamStatus>();
				if (teams.Any()) //the user has a team role, and there's a filter coming in, meaning only display own teams roles
				{
					consultationListRows.ForEach(c => c.Show = c.Show && currentUsersTeamRoles.Contains(c.AllowedRole, StringComparer.OrdinalIgnoreCase));
				}
			}

			return consultationListRows.OrderByDescending(c => c.EndDate).ToList();
		}

		private Dictionary<int, IList<OrganisationCode>> GetConsultationCodesForAllConsultations(IList<int> consultationIds)
		{
			var consultationSourceURIs = consultationIds.Select(consultationId => ConsultationsUri.CreateConsultationURI(consultationId)).ToList();

			var organisationAuthorisations = _context.GetOrganisationAuthorisations(consultationSourceURIs);

			var distinctOrganisationIds = organisationAuthorisations.Select(oa => oa.OrganisationId).Distinct();

			//todo: get organisation name + ids from idam that the current user has access to.


			var codesPerConsultation = new Dictionary<int, IList<OrganisationCode>>();
			foreach (var consultationId in consultationIds)
			{
				var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);

				var organisationAuthorisationsForThisConsultation = organisationAuthorisations
					.Where(oa => oa.Location.SourceURI.Equals(sourceURI, StringComparison.OrdinalIgnoreCase)).ToList();

				codesPerConsultation.Add(consultationId,
					organisationAuthorisationsForThisConsultation.Select(oa =>
						new OrganisationCode(oa.OrganisationId, "TODO: get organisation name from the list from idam", oa.CollationCode))
					.ToList());
			}

			return codesPerConsultation;
		}
	}
}
