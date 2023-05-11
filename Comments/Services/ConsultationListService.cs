using Comments.Common;
using Comments.Configuration;
using Comments.Models;
using Comments.ViewModels;
using Microsoft.FeatureManagement;
using NICE.Feeds.Indev;
using NICE.Feeds.Indev.Models.List;
using NICE.Identity.Authentication.Sdk.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Services
{
	public interface IConsultationListService
	{
		Task<(ConsultationListViewModel consultationListViewModel, Validate validate)>  GetConsultationListViewModel(ConsultationListViewModel model);
	}

	public class ConsultationListService : IConsultationListService
	{
		private readonly ConsultationsContext _context;
		private readonly IIndevFeedService _feedService;
		private readonly IUserService _userService;
		private readonly IFeatureManager _featureManager;
		private readonly IOrganisationService _organisationService;

		public ConsultationListService(ConsultationsContext consultationsContext, IIndevFeedService feedService, IUserService userService, IFeatureManager featureManager,
			IOrganisationService organisationService)
		{
			_context = consultationsContext;
			_feedService = feedService;
			_userService = userService;
			_featureManager = featureManager;
			_organisationService = organisationService;
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
		public async Task<(ConsultationListViewModel consultationListViewModel, Validate validate)> GetConsultationListViewModel(ConsultationListViewModel model)
		{
			var currentUser = _userService.GetCurrentUser();
			if (!currentUser.IsAuthenticatedByAnyMechanism)
				return (model, new Validate(valid: false, unauthenticated: true));

			var userRoles = _userService.GetUserRoles().ToList();
			var isAdminUser = userRoles.Any(role => AppSettings.ConsultationListConfig.DownloadRoles.AdminRoles.Contains(role));
			var teamRoles = userRoles.Where(role => AppSettings.ConsultationListConfig.DownloadRoles.TeamRoles.Contains(role)).Select(role => role).ToList();
			var isTeamUser = !isAdminUser && teamRoles.Any(); //an admin with team roles is still just considered an admin.
			var hasAccessToViewUpcomingConsultations = isAdminUser || isTeamUser;
			var hasAccessToViewHiddenConsultations = hasAccessToViewUpcomingConsultations || userRoles.Where(r => r == "IndevUser").Count()  > 0;
			var currentUserIsAuthorisedToViewOrganisationCodes = isAdminUser || currentUser.OrganisationsAssignedAsLead.Any();

			var canSeeAnySubmissionCounts = isAdminUser || isTeamUser;

			var consultationsFromIndev = (await _feedService.GetConsultationList()).ToList();

			var submittedCommentsAndAnswerCounts = canSeeAnySubmissionCounts ? _context.GetSubmittedCommentsAndAnswerCounts() : null;
			var sourceURIsCommentedOrAnswered = _context.GetAllSourceURIsTheCurrentUserHasCommentedOrAnsweredAQuestion();

            if (model.InitialPageView) //set defaults here for the filters - this code might be called by the SSR or by the client-side.
			{
				//if unset (by querystring) set the default for the contribution filter. - the filter should be enabled (HasContributed), for regular users (i.e. not admins or team users) and the user has contributed to any.
				//if (model.Contribution == null)
				//{
				//	var userHasCommentedOrAnswered = sourceURIsCommentedOrAnswered.Any();
				//	if ((!isAdminUser && !isTeamUser) && userHasCommentedOrAnswered)
				//	{
				//		model.Contribution = new List<ContributionStatus> {ContributionStatus.HasContributed};
				//	}
				//}

				//if unset (by querystring) set the default for the team filter - only for team users. if a user has admin and team rights, then admin overrules team and the filter will be unset.
				if (model.Team == null && isTeamUser)
				{
					model.Team = new List<TeamStatus> { TeamStatus.MyTeam };
				}
			}
			
			var	allOrganisationCodes = await GetConsultationCodesForAllConsultations(consultationsFromIndev.Select(c => c.ConsultationId).ToList(), isAdminUser, currentUser.OrganisationsAssignedAsLead.ToList());

            var submittedToLeadCommentsAndAnswerCounts = _context.GetSubmittedCommentsAndAnswerCounts(true);

            var consultationListRows = new List<ConsultationListRow>();

            foreach (var consultation in consultationsFromIndev)
			{
				var sourceURI = ConsultationsUri.CreateConsultationURI(consultation.ConsultationId);

				var (hasCurrentUserEnteredCommentsOrAnsweredQuestions, hasCurrentUserSubmittedCommentsOrAnswers)
					= GetFlagsForWhetherTheCurrentUserHasCommentedOrAnsweredThisConsultation(sourceURIsCommentedOrAnswered, sourceURI);

				var canSeeSubmissionCountForThisConsultation = (isAdminUser || (isTeamUser && teamRoles.Contains(consultation.AllowedRole)));

                int? responseCount = null;
                if (canSeeSubmissionCountForThisConsultation && submittedCommentsAndAnswerCounts != null)
                    responseCount = submittedCommentsAndAnswerCounts.Where(s => s.SourceURI.Equals(sourceURI))
                                                                    .Sum(s => s.TotalCount);

                int? responseToLeadCount = null;
                if (submittedToLeadCommentsAndAnswerCounts != null && currentUser.OrganisationsAssignedAsLead.FirstOrDefault() != null)
                    responseToLeadCount = submittedToLeadCommentsAndAnswerCounts
                           .FirstOrDefault(s => s.SourceURI.Equals(sourceURI) && 
                                                s.OrganisationId.Equals(currentUser.OrganisationsAssignedAsLead.FirstOrDefault()?.OrganisationId))?.TotalCount ?? 0;

                consultationListRows.Add(
					new ConsultationListRow(consultation.Title,
						consultation.StartDate, consultation.EndDate, responseCount, consultation.ConsultationId,
						consultation.FirstConvertedDocumentId, consultation.FirstChapterSlugOfFirstConvertedDocument, consultation.Reference,
						consultation.ProductTypeName, hasCurrentUserEnteredCommentsOrAnsweredQuestions, hasCurrentUserSubmittedCommentsOrAnswers, consultation.AllowedRole,
						allOrganisationCodes[consultation.ConsultationId], currentUserIsAuthorisedToViewOrganisationCodes, responseToLeadCount, consultation.Hidden));
			}

			model.OptionFilters = GetOptionFilterGroups(model.Status?.ToList(), consultationListRows, hasAccessToViewUpcomingConsultations);
			model.TextFilter = GetTextFilterGroups(model.Keyword, consultationListRows);
			model.ContributionFilter = GetContributionFilter(model.Contribution?.ToList(), consultationListRows);
			model.TeamFilter = isTeamUser ? GetTeamFilter(model.Team?.ToList(), consultationListRows, teamRoles) : null;
            model.HiddenConsultationsFilter = GetHiddenConsultationsFilter(model.HiddenConsultations?.ToList(), consultationListRows, hasAccessToViewHiddenConsultations);
            model.Consultations = FilterAndOrderConsultationList(consultationListRows, model.Status, model.Keyword, model.Contribution, model.Team, model.HiddenConsultations, (isTeamUser ? teamRoles : null), hasAccessToViewUpcomingConsultations, hasAccessToViewHiddenConsultations);
			model.User = new DownloadUser(isAdminUser, isTeamUser, currentUser, teamRoles);

			return (model, new Validate(valid: true));
		}

        private (bool hasEnteredCommentsOrAnsweredQuestions, bool hasSubmittedCommentsOrAnswers)
			GetFlagsForWhetherTheCurrentUserHasCommentedOrAnsweredThisConsultation(IEnumerable<KeyValuePair<string, Models.Status>> sourceURIsCommentedOrAnswered, string consultationSourceURI)
		{
			var foundCommentOrAnswer = sourceURIsCommentedOrAnswered.FirstOrDefault(s =>
				s.Key.Equals(consultationSourceURI) || s.Key.StartsWith(consultationSourceURI + "/"));

			if (foundCommentOrAnswer.Equals(default(KeyValuePair<string, Models.Status>)))
				return (hasEnteredCommentsOrAnsweredQuestions: false, hasSubmittedCommentsOrAnswers: false);

			if (!foundCommentOrAnswer.Value.StatusId.Equals((int)StatusName.Draft))
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

        private IEnumerable<OptionFilterGroup> GetHiddenConsultationsFilter(List<HiddenConsultationStatus> hiddenConsultation, List<ConsultationListRow> consultationListRows, bool hasAccessToViewHiddenConsultations)
        {
            if (hasAccessToViewHiddenConsultations)
            {
                var hiddenConsultationsFilter = AppSettings.ConsultationListConfig.HiddenConsultationsFilter.ToList();

                var ShowHiddenConsultationsOption = hiddenConsultationsFilter
                    .Single(option => option.Id.Equals("HiddenConsultations", StringComparison.OrdinalIgnoreCase))
                    .Options
                    .Single(option => option.Id.Equals("ShowHiddenConsultations", StringComparison.OrdinalIgnoreCase));

                ShowHiddenConsultationsOption.IsSelected = hiddenConsultation != null && hiddenConsultation.Contains(HiddenConsultationStatus.ShowHiddenConsultations);
                ShowHiddenConsultationsOption.UnfilteredResultCount = consultationListRows.Count;
                ShowHiddenConsultationsOption.FilteredResultCount = consultationListRows.Count(c => c.Hidden);

                return hiddenConsultationsFilter;
            }

            return null;
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
			IEnumerable<ContributionStatus> contribution, IEnumerable<TeamStatus> team, IEnumerable<HiddenConsultationStatus> hiddenConsultations, List<string> currentUsersTeamRoles, bool hasAccessToViewUpcomingConsultations, bool hasAccessToViewHiddenConsultations)
		{
			if (!hasAccessToViewUpcomingConsultations)
				consultationListRows.RemoveAll(clr => clr.IsUpcoming);

            if(!hasAccessToViewHiddenConsultations)
                consultationListRows.RemoveAll(clr => clr.Hidden);

            hiddenConsultations = hiddenConsultations?.ToList() ?? new List<HiddenConsultationStatus>();
            if (hasAccessToViewHiddenConsultations)
            {
                if (hiddenConsultations.Any())
                {
                    consultationListRows.RemoveAll(clr => !clr.Hidden);
                    consultationListRows.ForEach(clr => clr.Show = clr.Hidden);
                }
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

		private async Task<Dictionary<int, List<OrganisationCode>>> GetConsultationCodesForAllConsultations(IList<int> consultationIds, bool isAdminUser, IList<Organisation> organisationsAssignedAsLead)
		{
			var consultationSourceURIs = consultationIds.Select(consultationId => ConsultationsUri.CreateConsultationURI(consultationId)).ToList();

			var organisationAuthorisationsInDB = _context.GetOrganisationAuthorisations(consultationSourceURIs).ToList();

			var organsationsFromClaims = organisationsAssignedAsLead.ToDictionary(k => k.OrganisationId, v => v.OrganisationName);
			var organisationsThatNeedLookingUp = organisationAuthorisationsInDB.Select(oa => oa.OrganisationId).Distinct().Where(orgId => !organsationsFromClaims.ContainsKey(orgId)).ToList();

			var lookedUpOrganisations = await _organisationService.GetOrganisationNames(organisationsThatNeedLookingUp);

			var allOrganisationsThatMightBeShown = organsationsFromClaims.Merge(lookedUpOrganisations);

			var codesPerConsultation = new Dictionary<int, List<OrganisationCode>>();
			foreach (var consultationId in consultationIds)
			{
				var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);

				List<OrganisationAuthorisation> organisationAuthorisationsToShowForUser;
				if (isAdminUser) //admin's can see all collation codes. 
				{
					organisationAuthorisationsToShowForUser = organisationAuthorisationsInDB
						.Where(oa => oa.Location.SourceURI.Equals(sourceURI, StringComparison.OrdinalIgnoreCase))
						.ToList();
				}
				else //non-admin's who are organisation leads 
				{
					organisationAuthorisationsToShowForUser = organisationAuthorisationsInDB
						.Where(oa => oa.Location.SourceURI.Equals(sourceURI, StringComparison.OrdinalIgnoreCase) &&
						             organisationsAssignedAsLead.Any(org => org.OrganisationId.Equals(oa.OrganisationId)))
						.ToList();
				}

				//now to add blank rows.
				if (organisationsAssignedAsLead.Any())
				{
					var eachOrganisationTheUserIsLinkedToHasACode = organisationAuthorisationsToShowForUser.Count >= organisationsAssignedAsLead.Count();
					if (!eachOrganisationTheUserIsLinkedToHasACode)
					{
						var extraRowsToAdd = organisationsAssignedAsLead.Where(oa =>
								!organisationAuthorisationsToShowForUser.Exists(o => o.OrganisationId == oa.OrganisationId))
							.Select(oa => new OrganisationAuthorisation(null, DateTime.UtcNow, oa.OrganisationId, 0, collationCode: null));

						organisationAuthorisationsToShowForUser.AddRange(extraRowsToAdd);
					}
				}

				codesPerConsultation.Add(consultationId,
					organisationAuthorisationsToShowForUser.Select(oa =>
						new OrganisationCode(oa.OrganisationAuthorisationId,
							oa.OrganisationId,
							allOrganisationsThatMightBeShown.ContainsKey(oa.OrganisationId) ? allOrganisationsThatMightBeShown[oa.OrganisationId] : null,
							oa.CollationCode,
							organisationsAssignedAsLead.Any(leadOrgs => leadOrgs.OrganisationId.Equals(oa.OrganisationId) && leadOrgs.IsLead)
							))
						.OrderBy(org => org.OrganisationName)
						.ToList());
			}

			return codesPerConsultation;
		}
	}
}
