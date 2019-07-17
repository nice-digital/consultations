using Comments.Common;
using Comments.Models;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
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
            using (var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
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
            using (var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
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
			    ConsultationsUri.ConvertToConsultationsUri("/1/Review", CommentOn.Consultation)
		    };

			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
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
		    var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
		    var results = consultationsContext.GetAllCommentsAndQuestionsForDocument(sourceURIs, false);

		    //Assert
		    results.Count().ShouldBe(3);
	    }

	    [Fact]
	    public void Get_Questions_For_SourceURI()
	    {
		    // Arrange
		    ResetDatabase();

		    var expectedQuestionIdsInResultSet = new List<int>();

			//these questions should appear in the resultset
		    var locationId = AddLocation("consultations://./consultation/1/document/1/chapter/introduction");
			var questionTypeId = AddQuestionType("Question Type", false, true);
		    expectedQuestionIdsInResultSet.Add(AddQuestion(locationId, questionTypeId, "Question Label"));		

			locationId = AddLocation("consultations://./consultation/1/document/1");
		    expectedQuestionIdsInResultSet.Add(AddQuestion(locationId, questionTypeId, "Question Label"));

			locationId = AddLocation("consultations://./consultation/1");
			expectedQuestionIdsInResultSet.Add(AddQuestion(locationId, questionTypeId, "Question Label"));

			//these questions shouldn't appear in the resultset.
		    var differetDocumentLocationId = AddLocation("consultations://./consultation/1/document/2");
		    AddQuestion(differetDocumentLocationId, questionTypeId, "Question Label");

			var differentChapterLocationId = AddLocation("consultations://./consultation/1/document/1/chapter/overview");
		    AddQuestion(differentChapterLocationId, questionTypeId, "Question Label");

			var differentConsultationLocationId = AddLocation("consultations://./consultation/2");
			AddQuestion(differentConsultationLocationId, questionTypeId, "Question Label");

			var sourceURIs = new List<string>
		    {
			    ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Consultation),
			    ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Document),
			    ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Chapter)
		    };

		    // Act
		    var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
		    var results = consultationsContext.GetQuestionsForDocument(sourceURIs, false).ToList();
			
		    //Assert
		    results.Count.ShouldBe(expectedQuestionIdsInResultSet.Count);
		    foreach (var result in results)
		    {
			    foreach (var question in result.Question)
			    {
				    expectedQuestionIdsInResultSet.Contains(question.QuestionId).ShouldBeTrue();
			    }
		    }
		}

		[Fact]
		public void Get_Questions_For_Document_SourceURI()
		{
			// Arrange
			ResetDatabase();

			var expectedQuestionIdsInResultSet = new List<int>();

			//these questions should appear in the resultset
			var locationId = AddLocation("consultations://./consultation/1/document/1/chapter/introduction");
			var questionTypeId = AddQuestionType("Question Type", false, true);
			expectedQuestionIdsInResultSet.Add(AddQuestion(locationId, questionTypeId, "Question Label"));

			locationId = AddLocation("consultations://./consultation/1/document/1");
			expectedQuestionIdsInResultSet.Add(AddQuestion(locationId, questionTypeId, "Question Label"));

			var differentChapterLocationId = AddLocation("consultations://./consultation/1/document/1/chapter/overview");
			expectedQuestionIdsInResultSet.Add(AddQuestion(differentChapterLocationId, questionTypeId, "Question Label"));

			//these questions shouldn't appear in the resultset.

			var consultationLevelLocationId = AddLocation("consultations://./consultation/1");
			AddQuestion(consultationLevelLocationId, questionTypeId, "Question Label");

			var differetDocumentLocationId = AddLocation("consultations://./consultation/1/document/2");
			AddQuestion(differetDocumentLocationId, questionTypeId, "Question Label");

			var differentConsultationLocationId = AddLocation("consultations://./consultation/2");
			AddQuestion(differentConsultationLocationId, questionTypeId, "Question Label");


			var sourceURIs = new List<string>
			{
			    ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Document),
		    };

			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
			var results = consultationsContext.GetQuestionsForDocument(sourceURIs, true).ToList();

			//Assert
			results.Count.ShouldBe(expectedQuestionIdsInResultSet.Count);
			foreach (var result in results)
			{
				foreach (var question in result.Question)
				{
					expectedQuestionIdsInResultSet.Contains(question.QuestionId).ShouldBeTrue();
				}
			}
		}

		[Fact]
		public void Get_Questions_For_Consultation_SourceURI()
		{
			// Arrange
			ResetDatabase();

			var expectedQuestionIdsInResultSet = new List<int>();

			var questionTypeId = AddQuestionType("Question Type", false, true);

			//these questions should appear in the resultset
			var consultationLevelLocationId = AddLocation("consultations://./consultation/1");
			expectedQuestionIdsInResultSet.Add(AddQuestion(consultationLevelLocationId, questionTypeId, "Question Label"));
			
			//these questions shouldn't appear in the resultset.

			var locationId = AddLocation("consultations://./consultation/1/document/1/chapter/introduction");
			AddQuestion(locationId, questionTypeId, "Question Label");

			locationId = AddLocation("consultations://./consultation/1/document/1");
			AddQuestion(locationId, questionTypeId, "Question Label");

			var differentChapterLocationId = AddLocation("consultations://./consultation/1/document/1/chapter/overview");
			AddQuestion(differentChapterLocationId, questionTypeId, "Question Label");

			var differetDocumentLocationId = AddLocation("consultations://./consultation/1/document/2");
			AddQuestion(differetDocumentLocationId, questionTypeId, "Question Label");

			var differentConsultationLocationId = AddLocation("consultations://./consultation/2");
			AddQuestion(differentConsultationLocationId, questionTypeId, "Question Label");


			var sourceURIs = new List<string>
			{
				ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Consultation),
			};

			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
			var results = consultationsContext.GetQuestionsForDocument(sourceURIs, false).ToList();

			//Assert
			results.Count.ShouldBe(expectedQuestionIdsInResultSet.Count);
			foreach (var result in results)
			{
				foreach (var question in result.Question)
				{
					expectedQuestionIdsInResultSet.Contains(question.QuestionId).ShouldBeTrue();
				}
			}
		}


		[Fact]
		public void Get_Questions_For_Consultation_Partially_Matching_SourceURI()
		{
			// Arrange
			ResetDatabase();

			var expectedQuestionIdsInResultSet = new List<int>();

			var questionTypeId = AddQuestionType("Question Type", false, true);

			//these questions should appear in the resultset
			var consultationLevelLocationId = AddLocation("consultations://./consultation/1");
			expectedQuestionIdsInResultSet.Add(AddQuestion(consultationLevelLocationId, questionTypeId, "Question Label"));

			var locationId = AddLocation("consultations://./consultation/1/document/1/chapter/introduction");
			expectedQuestionIdsInResultSet.Add(AddQuestion(locationId, questionTypeId, "Question Label"));

			locationId = AddLocation("consultations://./consultation/1/document/1");
			expectedQuestionIdsInResultSet.Add(AddQuestion(locationId, questionTypeId, "Question Label"));

			var differentChapterLocationId = AddLocation("consultations://./consultation/1/document/1/chapter/overview");
			expectedQuestionIdsInResultSet.Add(AddQuestion(differentChapterLocationId, questionTypeId, "Question Label"));

			var differetDocumentLocationId = AddLocation("consultations://./consultation/1/document/2");
			expectedQuestionIdsInResultSet.Add(AddQuestion(differetDocumentLocationId, questionTypeId, "Question Label"));

			//these questions shouldn't appear in the resultset.

			var differentConsultationLocationId = AddLocation("consultations://./consultation/2");
			AddQuestion(differentConsultationLocationId, questionTypeId, "Question Label");


			var sourceURIs = new List<string>
			{
				ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Consultation),
			};

			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
			var results = consultationsContext.GetQuestionsForDocument(sourceURIs, true).ToList();

			//Assert
			results.Count.ShouldBe(expectedQuestionIdsInResultSet.Count);
			foreach (var result in results)
			{
				foreach (var question in result.Question)
				{
					expectedQuestionIdsInResultSet.Contains(question.QuestionId).ShouldBeTrue();
				}
			}
		}

		[Fact]
		public void Get_All_QuestionTypes_Where_There_Is_Only_A_Single_QuestionType()
		{
			// Arrange
			ResetDatabase();

			var textQuestionTypeId = AddQuestionType("Text", false, true);

			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
			var results = consultationsContext.GetQuestionTypes();

			//Assert
			results.Single().QuestionTypeId.ShouldBe(textQuestionTypeId);
		}

	    [Fact]
	    public void Get_All_QuestionTypes_Where_There_Are_Multiple_QuestionTypes()
	    {
		    // Arrange
		    ResetDatabase();

		    var textQuestionTypeId = AddQuestionType("Text", hasBooleanAnswer: false, hasTextAnswer: true);
		    var booleanQuestionTypeId = AddQuestionType("Boolean", hasBooleanAnswer: true, hasTextAnswer: false);

			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
		    var results = consultationsContext.GetQuestionTypes().ToList();

		    //Assert
			results.Count().ShouldBe(2);
		    results.First().QuestionTypeId.ShouldBe(textQuestionTypeId);
		    results.Skip(1).First().QuestionTypeId.ShouldBe(booleanQuestionTypeId);
		}

		[Fact]
		public void Get_Previous_Questions_Filtering_out_duplicates()
		{
			// Arrange
			ResetDatabase();

			var questionTypeId = AddQuestionType("Question Type", false, true);
			var identicalQuestionText =
				"This question text is used in multiple questions. It should be distincted out so only 1 such question appears in the set";

			var locationId1 = AddLocation("consultations://./consultation/1");
			var questionId1 = AddQuestion(locationId1, questionTypeId, identicalQuestionText);

			var locationId2 = AddLocation("consultations://./consultation/2");
			AddQuestion(locationId2, questionTypeId, identicalQuestionText);

			var questionId2 = AddQuestion(locationId2, questionTypeId, "unique question");
			
			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
			var results = consultationsContext.GetAllPreviousUniqueQuestions().ToList();

			//Assert
			results.Count.ShouldBe(2);
			results[0].QuestionId.ShouldBe(questionId1);
			results[1].QuestionId.ShouldBe(questionId2);
		}

		[Fact]
		public void Get_Previous_Questions_Filtering_out_deletions()
		{
			// Arrange
			ResetDatabase();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: Guid.NewGuid());
			var context = CreateContext(userService);

			var questionTypeId = AddQuestionType("Question Type", false, true, 1, context);

			var locationId = AddLocation("consultations://./consultation/1", context);

			var questionId1 = AddQuestion(locationId, questionTypeId, "question 1", context);
			var questionId2 = AddQuestion(locationId, questionTypeId, "question 2", context);

			var question2 = context.GetQuestion(questionId2);
			question2.IsDeleted = true;

			// Act
			var results = context.GetAllPreviousUniqueQuestions().ToList();

			//Assert
			results.Single().QuestionId.ShouldBe(questionId1);
		}
	}
}
