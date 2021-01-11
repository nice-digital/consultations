using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Configuration;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Review
{
    public class ReviewTests : TestBase
    {
	    private static ReviewConfig GetTestReviewConfig()
	    {
		    return new ReviewConfig() { Filters = new List<OptionFilterGroup>()
		    {
				new OptionFilterGroup(){ Id = "Type", Title = "Response type", Options = new List<FilterOption>()
				{
					new FilterOption("Questions", "Questions"),
					new FilterOption("Comments", "Comments"),
				}},
				new OptionFilterGroup(){Id =  "Document", Title = "Document", Options = new List<FilterOption>()}
		    } };
		}

        [Fact]
        public async Task Get_AllCommentsForConsultationForReview()
        {
            // Arrange
            ResetDatabase();
	        _context.Database.EnsureCreated();
	        AppSettings.ReviewConfig = GetTestReviewConfig();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
	        var relativeURIForReviewPage = "/1/review";
	        var userId = Guid.Empty.ToString();

	        AddCommentsAndQuestionsAndAnswers(sourceURI, "My Comment", "question 1", "answer 1", userId);
	        AddCommentsAndQuestionsAndAnswers(sourceURI, "My Second Comment", "question 2", "answer 2", userId);
	        AddCommentsAndQuestionsAndAnswers(sourceURI, "Another users Comment", "question 3", "answer 3", Guid.NewGuid().ToString());

			// Act
			var response = await _client.GetAsync(string.Format("/consultations/api/CommentsForReview?relativeURL={0}", relativeURIForReviewPage));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var deserialisedObject = JsonConvert.DeserializeObject<ViewModels.ReviewPageViewModel>(responseString);

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.OK);
	        responseString.ShouldMatchApproved(new Func<string, string>[] { Scrubbers.ScrubLastModifiedDate, Scrubbers.ScrubIds });
			deserialisedObject.CommentsAndQuestions.Comments.Count().ShouldBe(2);
		}
	}
}
