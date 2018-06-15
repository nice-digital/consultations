using Comments.Common;
using Comments.Models;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Comments.Test.UnitTests
{
    public class ConsultationContext : TestBase
    {
        [Fact]
        public void Comments_IsDeleted_Flag_is_not_Filtering_in_the_context()
        {
            // Arrange
            ResetDatabase();
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var commentText = Guid.NewGuid().ToString();
            var createdByUserId = Guid.NewGuid();

            var locationId = AddLocation(sourceURI);
            AddComment(locationId, commentText, true, createdByUserId);

            // Act
            using (var consultationsContext = new ConsultationsContext(_options, _fakeUserService))
            {
                var unfilteredLocations = consultationsContext.Location.Where(l =>
                        l.SourceURI.Equals(sourceURI))
                    .Include(l => l.Comment)
                    .IgnoreQueryFilters()
                    .ToList();

                //Assert
                unfilteredLocations.First().Comment.Count.ShouldBe(1);
            }
        }

        [Fact]
        public void Comments_IsDeleted_Flag_is_Filtering_in_the_context()
        {
            // Arrange
            ResetDatabase();
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var commentText = Guid.NewGuid().ToString();
            var createdByUserId = Guid.NewGuid();

            var locationId = AddLocation(sourceURI);
            AddComment(locationId, commentText, true, createdByUserId);

            // Act
            using (var consultationsContext = new ConsultationsContext(_options, _fakeUserService))
            {
                var filteredLocations = consultationsContext.Location.Where(l => l.SourceURI.Equals(sourceURI))
                    .Include(l => l.Comment)
                    .ToList();

                //Assert
                //removed while filtering is commented out.
                filteredLocations.Single().Comment.Count.ShouldBe(0);
            }
        }

	    [Fact]
	    public void Get_All_Consultation_Comments_When_IsReview_Is_True()
	    {
		    // Arrange
		    ResetDatabase();
		    var commentText = Guid.NewGuid().ToString();
		    var createdByUserId = Guid.NewGuid();

		    var locationId = AddLocation("consultations://./consultation/1/document/1/chapter/introduction");
		    AddComment(locationId, commentText, true, createdByUserId);

		    locationId = AddLocation("consultations://./consultation/1/document/1");
		    AddComment(locationId, commentText, true, createdByUserId);

		    locationId = AddLocation("consultations://./consultation/1/document/2");
		    AddComment(locationId, commentText, true, createdByUserId);

			locationId = AddLocation("consultations://./consultation/1");
		    AddComment(locationId, commentText, true, createdByUserId);

		    locationId = AddLocation("consultations://./consultation/2");
		    AddComment(locationId, commentText, true, createdByUserId);


			var sourceURIs = new List<string>
		    {
			    ConsultationsUri.ConvertToConsultationsUri("/1/0/Review", CommentOn.Consultation)
		    };

			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService);
		    var results = consultationsContext.GetAllCommentsAndQuestionsForDocument(sourceURIs, true);

			//Assert
			results.Count().ShouldBe(4);
	    }

	    [Fact]
	    public void Get_Chapter_Comments_When_IsReview_Is_False()
	    {
		    // Arrange
		    ResetDatabase();
		    var commentText = Guid.NewGuid().ToString();
		    var createdByUserId = Guid.NewGuid();

		    var locationId = AddLocation("consultations://./consultation/1/document/1/chapter/introduction");
		    AddComment(locationId, commentText, true, createdByUserId);

		    locationId = AddLocation("consultations://./consultation/1/document/1/chapter/overview");
		    AddComment(locationId, commentText, true, createdByUserId);

			locationId = AddLocation("consultations://./consultation/1/document/1");
		    AddComment(locationId, commentText, true, createdByUserId);

		    locationId = AddLocation("consultations://./consultation/1/document/2");
		    AddComment(locationId, commentText, true, createdByUserId);

		    locationId = AddLocation("consultations://./consultation/1");
		    AddComment(locationId, commentText, true, createdByUserId);

		    locationId = AddLocation("consultations://./consultation/2");
		    AddComment(locationId, commentText, true, createdByUserId);


		    var sourceURIs = new List<string>
		    {
			    ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Consultation),
			    ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Document),
			    ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Chapter)
			};

		    // Act
		    var consultationsContext = new ConsultationsContext(_options, _fakeUserService);
		    var results = consultationsContext.GetAllCommentsAndQuestionsForDocument(sourceURIs, false);

		    //Assert
		    results.Count().ShouldBe(2);
	    }
	}
}
