using Comments.Common;
using Comments.Configuration;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Newtonsoft.Json;
using NICE.Feeds.Tests.Infrastructure;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NICE.Feeds.Indev.Models.List;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.UnitTests
{
	public class ConsultationListPageTests : TestBase
	{
		private readonly ConsultationsContext _consultationListContext;

		public ConsultationListPageTests() : base(TestUserType.Administrator, Feed.ConsultationCommentsPublishedDetailMulitpleDoc)
		{
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
			AppSettings.Feed.IndevBasePath = new Uri("http://www.nice.org.uk");
			_consultationListContext = new ConsultationListContext(_options, _fakeUserService, _fakeEncryption);
		}

		#region earlier tests

		[Fact]
		public async Task ConsultationListPageModel_HasConsultationsPopulated()
		{
			//Arrange
			var consultationListService = GetConsultationListService();

			//Act
			var viewModel = (await consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() })).consultationListViewModel;

			//Assert
			viewModel.Consultations.OrderBy(c => c.ConsultationId).First().ConsultationId.ShouldBe(123);
		}

		[Fact]
		public async Task ConsultationListPageModel_HasCorrectlySetResponseCount()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid().ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var context = CreateContext(userService);
			AddSubmittedCommentsAndAnswers(sourceURI, "Comment Label", "Question Label", "Answer Label", userId, context);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), GetFakeUserService(), _fakeFeatureManager, null);
			//Act
			var viewModel = await consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.consultationListViewModel.Consultations.First().SubmissionCount.ShouldBe(1);
		}

		[Fact]
		public async Task ConsultationListPageModel_HasCorrectlySetResponseCountWithMulitpleSubmissions()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid().ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var context = CreateContext(userService, 2);
			AddSubmittedCommentsAndAnswers(sourceURI, "Comment Label", "Question Label", "Answer Label", userId, context);
			AddSubmittedCommentsAndAnswers(sourceURI, "Second Comment Label", "Second Question Label", " Second Answer Label", userId, context);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), GetFakeUserService(), _fakeFeatureManager, null);

			//Act
			var viewModel = await consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.consultationListViewModel.Consultations.First().SubmissionCount.ShouldBe(2);
		}


		[Fact]
		public async Task ConsultationListPageModel_HasCorrectlySetResponseCountWithComments()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid().ToString();
			var context = CreateContext(_fakeUserService);
			var locationId = AddLocation(sourceURI, context);
			var commentId = AddComment(locationId, "Just a comment", userId, 2, context);
			var submissionId = AddSubmission(userId, context);
			AddSubmissionComments(submissionId, commentId, context);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), GetFakeUserService(), _fakeFeatureManager, null);

			//Act
			var viewModel = await consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.consultationListViewModel.Consultations.First().SubmissionCount.ShouldBe(1);
		}



		[Fact]
		public async Task ConsultationListPageModel_HasCorrectlySetResponseCountWithAnswers()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid().ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var context =  CreateContext(userService);

			var locationId = AddLocation(sourceURI, context);
			var questionTypeId = 99;
			var questionId = AddQuestion(locationId, questionTypeId, "Question Text", context);
			var AnswerId = AddAnswer(questionId, userId, "my answer", 2, context);
			var submissionId = AddSubmission(userId, context);
			AddSubmissionAnswers(submissionId, AnswerId, context);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), GetFakeUserService(), _fakeFeatureManager, null);

			//Act
			var viewModel = await consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.consultationListViewModel.Consultations.First().SubmissionCount.ShouldBe(1);
		}

		[Fact]
		public async Task ConsultationListPageModel_HasDocumentIdAndChapterSlugPopulatedCorrectly()
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), GetFakeUserService(), _fakeFeatureManager, null);

			//Act
			ConsultationListViewModel viewModel = (await consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() })).consultationListViewModel;

			//Assert
			viewModel.Consultations.First().DocumentId.ShouldBe(1);
			viewModel.Consultations.First().ChapterSlug.ShouldBe("my-chapter-slug");
		}

		[Fact]
		public async Task ConsultationListPageModel_HasFilterOptions()
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), GetFakeUserService(), _fakeFeatureManager, null);

			//Act
			ConsultationListViewModel viewModel = (await consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) {Status = new List<ConsultationStatus>()})).consultationListViewModel;

			//Assert
			var firstFilter = viewModel.OptionFilters.First();
			firstFilter.Title.ShouldBe("Status");
			firstFilter.Options.Count.ShouldBe(3);
			firstFilter.Options.First().Label.ShouldBe("Open");
			firstFilter.Options.Skip(1).First().Label.ShouldBe("Closed");
			firstFilter.Options.Skip(2).First().Label.ShouldBe("Upcoming");
		}

		[Fact]
		public async Task ConsultationListPageModel_HasFilterOptionOpenSetToSelected()
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), GetFakeUserService(), _fakeFeatureManager, null);
			var viewModel = new ConsultationListViewModel(null, null, null, null, null);
			viewModel.Status = new List<ConsultationStatus>(){ ConsultationStatus.Open };

			//Act
			var updatedViewModel = (await consultationListService.GetConsultationListViewModel(viewModel)).consultationListViewModel;

			//Assert
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Open").IsSelected.ShouldBeTrue();
		}

		[Fact]
		public async Task ConsultationListPageModel_HasFilterOptionClosedSetToSelected()
		{
			//Arrange
			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 123 });
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), GetFakeUserService(), _fakeFeatureManager, null);
			var viewModel = new ConsultationListViewModel(null, null, null, null, null);
			viewModel.Status = new List<ConsultationStatus>() { ConsultationStatus.Closed };

			//Act
			var updatedViewModel = (await consultationListService.GetConsultationListViewModel(viewModel)).consultationListViewModel;

			//Assert
			updatedViewModel.OptionFilters.First().Options.Skip(1).First(f => f.Id == "Closed").IsSelected.ShouldBeTrue();
		}

		[Fact]
		public async Task ConsultationListPageModel_HasFilterOptionUpcomingSetToSelected()
		{
			//Arrange
			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 123 });
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), GetFakeUserService(), _fakeFeatureManager, null);
			var viewModel = new ConsultationListViewModel(null, null, null, null, null);
			viewModel.Status = new List<ConsultationStatus>() { ConsultationStatus.Upcoming };

			//Act
			var updatedViewModel = (await consultationListService.GetConsultationListViewModel(viewModel)).consultationListViewModel;

			//Assert
			updatedViewModel.OptionFilters.First().Options.Skip(1).First(f => f.Id == "Upcoming").IsSelected.ShouldBeTrue();
		}

		[Fact]
		public async Task ConsultationListPageModel_WithFilterOptionOpenHasCorrectResultCounts()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus> {ConsultationStatus.Open}
			};

			//Act
			var updatedViewModel = (await consultationListService.GetConsultationListViewModel(viewModel)).consultationListViewModel;

			//Assert
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Open").FilteredResultCount.ShouldBe(1);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Open").UnfilteredResultCount.ShouldBe(3);
		}


		[Fact]
		public async Task ConsultationListPageModel_WithFilterOptionClosedHasCorrectResultCounts()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus> { ConsultationStatus.Closed }
			};

			//Act
			var updatedViewModel = (await consultationListService.GetConsultationListViewModel(viewModel)).consultationListViewModel;

			//Assert
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Closed").FilteredResultCount.ShouldBe(1);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Closed").UnfilteredResultCount.ShouldBe(3);
		}

		[Fact]
		public async Task ConsultationListPageModel_WithFilterOptionUpcomingHasCorrectResultCounts()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus> { ConsultationStatus.Upcoming }
			};

			//Act
			var updatedViewModel = (await consultationListService.GetConsultationListViewModel(viewModel)).consultationListViewModel;

			//Assert
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Upcoming").FilteredResultCount.ShouldBe(1);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Upcoming").UnfilteredResultCount.ShouldBe(3);
		}

		[Fact]
		public async Task ConsultationListPageModel_WithFilterOptionOpenAndClosedHasCorrectResultCounts()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus> { ConsultationStatus.Closed, ConsultationStatus.Open, ConsultationStatus.Upcoming }
			};

			//Act
			var updatedViewModel = (await consultationListService.GetConsultationListViewModel(viewModel)).consultationListViewModel;

			//Assert
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Open").FilteredResultCount.ShouldBe(1);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Open").UnfilteredResultCount.ShouldBe(3);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Closed").FilteredResultCount.ShouldBe(1);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Closed").UnfilteredResultCount.ShouldBe(3);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Upcoming").FilteredResultCount.ShouldBe(1);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Upcoming").UnfilteredResultCount.ShouldBe(3);
		}

		[Fact]
   		public async Task ConsultationListPageModel_WithFilterReferenceTextSetToValue()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = "GID-1"
			};

			//Act
			var updatedViewModel = (await consultationListService.GetConsultationListViewModel(viewModel)).consultationListViewModel;

			//Assert
			updatedViewModel.TextFilter.FilteredResultCount.ShouldBe(1);
			updatedViewModel.TextFilter.UnfilteredResultCount.ShouldBe(3);
		}

		[Fact]
		public async Task ConsultationListPageModel_WithFilterReferenceTextSetToPartialValue()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = "gid"
			};

			//Act
			var updatedViewModel = (await consultationListService.GetConsultationListViewModel(viewModel)).consultationListViewModel;

			//Assert
			updatedViewModel.TextFilter.FilteredResultCount.ShouldBe(3);
			updatedViewModel.TextFilter.UnfilteredResultCount.ShouldBe(3);
		}

		[Theory]
		[InlineData(null)]
		[InlineData(" ")]
		[InlineData("")]
		public async Task ConsultationListPageModel_WithFilterReferenceTextSetToNull(string reference)
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = reference
			};

			//Act
			var updatedViewModel = (await consultationListService.GetConsultationListViewModel(viewModel)).consultationListViewModel;

			//Assert
			updatedViewModel.TextFilter.IsSelected.ShouldBeFalse();
		}

		[Fact]
		public async Task ConsultationListPageModel_WithFilterReferenceTextSetToPartialTitle()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = "consultation title"
			};

			//Act
			var updatedViewModel = (await consultationListService.GetConsultationListViewModel(viewModel)).consultationListViewModel;

			//Assert
			updatedViewModel.TextFilter.FilteredResultCount.ShouldBe(3);
			updatedViewModel.TextFilter.UnfilteredResultCount.ShouldBe(3);
		}

		[Fact]
		public async Task ConsultationListPageModel_WithFilterReferenceTextSetToSpecificTitle()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = "consultation title 3"
			};

			//Act
			var updatedViewModel = (await consultationListService.GetConsultationListViewModel(viewModel)).consultationListViewModel;

			//Assert
			updatedViewModel.TextFilter.FilteredResultCount.ShouldBe(1);
			updatedViewModel.TextFilter.UnfilteredResultCount.ShouldBe(3);
		}

		[Fact]
		public async Task ConsultationListPageModel_FilterDataByKeyword()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = "consultation title 3"
			};

			//Act
			var updatedViewModel = await consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.consultationListViewModel.Consultations.Count(c => c.Show).ShouldBe(1);
		}

		[Fact]
		public async Task ConsultationListPageModel_FilterDataByOpenStatus()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus> { ConsultationStatus.Open }
			};

			//Act
			var updatedViewModel = await consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.consultationListViewModel.Consultations.Count(c => c.Show).ShouldBe(1);
		}

		[Fact]
		public async Task ConsultationListPageModel_FilterDataByOpenandUpcomingStatus()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus> { ConsultationStatus.Upcoming, ConsultationStatus.Open }
			};

			//Act
			var updatedViewModel = await consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.consultationListViewModel.Consultations.Count(c => c.Show).ShouldBe(2);
		}

		[Fact]
		public async Task ConsultationListPageModel_FilterDataByOpenStatusAndKeyword()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = "consultation title 1",
				Status = new List<ConsultationStatus> { ConsultationStatus.Open }
			};

			//Act
			var updatedViewModel = await consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.consultationListViewModel.Consultations.Count(c => c.Show).ShouldBe(1);
		}

		[Fact]
		public async Task ConsultationListPageModel_FilterDataByGidReferenceKeyword()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = "GID-1"
			};

			//Act
			var updatedViewModel = await consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.consultationListViewModel.Consultations.Count(c => c.Show).ShouldBe(1);
		}
		
		private ConsultationListService GetConsultationListService()
		{
			const int organisationId = 1;
			const string organisationName = "Sherman Oaks";
			var fakeOrganisationService = new FakeOrganisationService(new Dictionary<int, string> { { organisationId, organisationName } });

			var consultationList = AddConsultationsToList();
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), GetFakeUserService(), _fakeFeatureManager, fakeOrganisationService);
			return consultationListService;
		}

		[Theory]
		[InlineData(TestUserType.CustomFictionalRole)]
		[InlineData(TestUserType.Administrator)]
		public void ConsultationListServiceConstructorWorksWithCorrectSecurity(TestUserType testUserType)
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var userId = Guid.NewGuid().ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId, testUserType: testUserType);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);

			//Act
			var consultationService = new ConsultationListService(consultationContext, new FakeFeedService(consultationList), _fakeUserService, _fakeFeatureManager, null);

			//Assert
			consultationService.ShouldNotBeNull();
		}

		#endregion earlier tests


		[Fact]
		public async Task CHTETeamMemberCanOnlySeeTheirOwnConsultationsWhenTheFilterIsChecked()
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var userService = FakeUserService.Get(true, "Jeffrey Goines", Guid.Empty.ToString(), TestUserType.ConsultationListTestRole);
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), userService, _fakeFeatureManager, null);

			//Act
			ConsultationListViewModel viewModel = (await consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus>(),
				Team = new List<TeamStatus>() { TeamStatus.MyTeam }
			})).consultationListViewModel;
			var serialisedViewModel = JsonConvert.SerializeObject(viewModel); //doing this in order to validate the filters coming back in the model.

			//Assert
			viewModel.Consultations.Count().ShouldBe(3);
			viewModel.Consultations.Count(c => c.Show).ShouldBe(2);
			serialisedViewModel.ShouldMatchApproved(new Func<string, string>[] { Scrubbers.ScrubStartDate, Scrubbers.ScrubEndDate });
		}

		[Fact]
		public async Task CHTETeamMemberSeeAllConsultationsWhenTheFilterIsUnchecked()
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var userService = FakeUserService.Get(true, "Jeffrey Goines", Guid.NewGuid().ToString(), TestUserType.ConsultationListTestRole);
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), userService, _fakeFeatureManager, null);

			//Act
			ConsultationListViewModel viewModel = (await consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() })).consultationListViewModel;

			//Assert
			viewModel.Consultations.Count().ShouldBe(3);
			viewModel.Consultations.Count(c => c.Show).ShouldBe(3);
		}
	}

	public class ConsultationListOrganisationCodeTests : TestBase
	{
		private readonly ConsultationsContext _consultationListContext;

		public ConsultationListOrganisationCodeTests() : base(TestUserType.Administrator, Feed.ConsultationCommentsPublishedDetailMulitpleDoc, enableOrganisationalCommentingFeature: true)
		{
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
			AppSettings.Feed.IndevBasePath = new Uri("http://www.nice.org.uk");
			_consultationListContext = new ConsultationListContext(_options, _fakeUserService, _fakeEncryption);
		}

		[Fact]
		public async Task ConsultationListPageModel_HasOrganisationCodesSetCorrectly()
		{
			//Arrange
			const string collationCode = "FAKE CODE";
			var consultationList = AddConsultationsToList();
			const int organisationId = 1;
			const string organisationName = "Sherman Oaks";
			var fakeOrganisationService =  new FakeOrganisationService(new Dictionary<int, string> {{ organisationId, organisationName } });

			using (var consultationListContext = new ConsultationListContext(_options, _fakeUserService, _fakeEncryption))
			{
				var firstConsultation = consultationList.First();
				var sourceURI = ConsultationsUri.CreateConsultationURI(firstConsultation.ConsultationId);

				var location = new Models.Location(sourceURI, null, null, null, null, null, null, null, null, null, null, null);
				consultationListContext.Location.Add(location);
				consultationListContext.SaveChanges();

				consultationListContext.OrganisationAuthorisation.Add(new OrganisationAuthorisation(Guid.Empty.ToString(),
					DateTime.Now, organisationId: organisationId, locationId: location.LocationId, collationCode: collationCode));
				consultationListContext.SaveChanges();

				var consultationListService = new ConsultationListService(consultationListContext, new FakeFeedService(consultationList), GetFakeUserService(), _fakeFeatureManager, fakeOrganisationService);

				//Act
				ConsultationListViewModel viewModel = (await consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() })).consultationListViewModel;
				var serialisedViewModel = JsonConvert.SerializeObject(viewModel);

				//Assert
				var consultationRow = viewModel.Consultations.Single(c => c.ConsultationId == firstConsultation.ConsultationId);
				var organisationCode = consultationRow.OrganisationCodes.Single();
				organisationCode.CollationCode.ShouldBe(collationCode);
				serialisedViewModel.ShouldMatchApproved(new Func<string, string>[] { Scrubbers.ScrubStartDate, Scrubbers.ScrubEndDate, Scrubbers.ScrubUserId });
			}
		}

		[Fact]
		public async Task ConsultationListPageModel_HasCorrectlySetSubmissionToLeadCount()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			const string sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid().ToString();
			const int organisationId = 1;
			const string organisationName = "Sherman Oaks";

			var fakeOrganisationService = new FakeOrganisationService(new Dictionary<int, string> { { organisationId, organisationName } });
			var fakeUserService = FakeUserService.Get(true, "Benjamin Button", userId, TestUserType.Authenticated, true, null, organisationId);

			var context = CreateContext(fakeUserService, 2);
			AddSubmittedCommentsAndAnswers(sourceURI, "Comment Label", "Question Label", "Answer Label", userId, context, (int)StatusName.SubmittedToLead);
			AddSubmittedCommentsAndAnswers(sourceURI, "Second Comment Label", "Second Question Label", " Second Answer Label", userId, context, (int)StatusName.SubmittedToLead);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), fakeUserService, _fakeFeatureManager, fakeOrganisationService);

			//Act
			var viewModel = await consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.consultationListViewModel.Consultations.First().SubmissionToLeadCount.ShouldBe(2);
		}

		[Fact]
		public async Task ConsultationListPageModel_SubmissionToLeadCountIsNullWhenNoOrganisationsAssignedToUser()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			const string sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid().ToString();
			const int organisationId = 1;
			const string organisationName = "Sherman Oaks";

			var fakeOrganisationService = new FakeOrganisationService(new Dictionary<int, string> { { organisationId, organisationName } });
			var fakeUserService = FakeUserService.Get(true, "Benjamin Button", userId, TestUserType.Authenticated, true, null, null);

			var context = CreateContext(fakeUserService, 2);
			AddSubmittedCommentsAndAnswers(sourceURI, "Comment Label", "Question Label", "Answer Label", userId, context, (int)StatusName.SubmittedToLead);
			AddSubmittedCommentsAndAnswers(sourceURI, "Second Comment Label", "Second Question Label", " Second Answer Label", userId, context, (int)StatusName.SubmittedToLead);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), fakeUserService, _fakeFeatureManager, fakeOrganisationService);

			//Act
			var viewModel = await consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.consultationListViewModel.Consultations.First().SubmissionToLeadCount.ShouldBe(null);
		}

		[Fact]
		public async Task ConsultationListPageModel_SubmissionToLeadCountIs0IfNoSubmissionsToOrg()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var userId = Guid.NewGuid().ToString();
			const int organisationId = 2; 
			const string organisationName = "Sherman Oaks";

			var fakeOrganisationService = new FakeOrganisationService(new Dictionary<int, string> { { organisationId, organisationName } });
			var fakeUserService = FakeUserService.Get(true, "Benjamin Button", userId, TestUserType.Authenticated, true, null, organisationId);

			var context = CreateContext(fakeUserService, 2);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), fakeUserService, _fakeFeatureManager, fakeOrganisationService);

			//Act
			var viewModel = await consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.consultationListViewModel.Consultations.First().SubmissionToLeadCount.ShouldBe(0);
		}
	}

	public class ConsultationListFilterTests : TestBase
	{
		public ConsultationListFilterTests(bool useRealSubmitService = false, TestUserType testUserType = TestUserType.Authenticated, bool useFakeConsultationService = false, IList<SubmittedCommentsAndAnswerCount> submittedCommentsAndAnswerCounts = null, bool bypassAuthentication = true) : base(useRealSubmitService, testUserType, useFakeConsultationService, submittedCommentsAndAnswerCounts, bypassAuthentication)
		{
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
			AppSettings.Feed.IndevBasePath = new Uri("http://www.nice.org.uk");
		}

		[Fact]
		public async Task OnlySeeConsultationsWhereCurrentUserHasCommentedOnWhenFilterIsChecked()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var sourceURIConsultation1 = "consultations://./consultation/1/document/1/chapter/introduction";
			var sourceURIConsultation1DocumentLevel = "consultations://./consultation/1/document/1";
			var sourceURIConsultation2 = "consultations://./consultation/2";
			var userId = Guid.Empty.ToString();

			var locationId1 = AddLocation(sourceURIConsultation1, _context);
			AddComment(locationId1, "the current users comment", userId, 2, _context);

			var locationId1Doc = AddLocation(sourceURIConsultation1DocumentLevel, _context);
			AddComment(locationId1Doc, "another comment similar source uri", userId, 2, _context);

			var locationId2 = AddLocation(sourceURIConsultation2, _context);
			AddComment(locationId2, "a comment that should be filtered out", "not the current user's id", 2, _context);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			consultationList.Add(new ConsultationList { ConsultationId = 2 });

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId, TestUserType.Authenticated);

			var context = new ConsultationsContext(_options, userService, _fakeEncryption);

			const int organisationId = 1;
			const string organisationName = "Sherman Oaks";
			var fakeOrganisationService = new FakeOrganisationService(new Dictionary<int, string> { { organisationId, organisationName } });

			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), userService, _fakeFeatureManager, fakeOrganisationService);

			//Act
			var viewModel = (await consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus>(),
				Contribution = new List<ContributionStatus>() { ContributionStatus.HasContributed }
			}));
			var serialisedViewModel = JsonConvert.SerializeObject(viewModel); //saves checking all the properties


			//Assert
			viewModel.consultationListViewModel.Consultations.Count().ShouldBe(2);
			viewModel.consultationListViewModel.Consultations.Count(c => c.Show).ShouldBe(1);
			serialisedViewModel.ShouldMatchApproved();
		}
	}
}
