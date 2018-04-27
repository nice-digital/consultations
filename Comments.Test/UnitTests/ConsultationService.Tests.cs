using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Newtonsoft.Json;
using NICE.Feeds;
using NICE.Feeds.Tests.Infrastructure;
using Shouldly;
using Xunit;
using Question = Comments.Models.Question;

namespace Comments.Test.UnitTests
{
    public class Tests : Comments.Test.Infrastructure.TestBase
    {
        [Fact]
        public void Comments_CanBeRead()
        { 
            // Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var commentText = Guid.NewGuid().ToString();
            var createdByUserId = Guid.NewGuid();

            var userService = FakeUserService.Get(true, "Benjamin Button", createdByUserId);

            var locationId = AddLocation(sourceURI);
            AddComment(locationId, commentText, isDeleted: false, createdByUserId: createdByUserId);
            
            var commentService = new CommentService(new ConsultationsContext(_options, userService), userService);
            
            // Act
            var viewModel = commentService.GetCommentsAndQuestions(sourceURI);

            //Assert
            viewModel.Comments.Single().CommentText.ShouldBe(commentText);
        }

        [Fact]
        public async void CommentsQuestionsAndAnswers_CanBeRead()
        {
            // Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var commentText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var answerText = Guid.NewGuid().ToString();
            var createdByUserId = Guid.Empty;

            AddCommentsAndQuestionsAndAnswers(sourceURI, commentText, questionText, answerText, createdByUserId, _context);
            var commentService = new CommentService(_context, _fakeUserService);

            // Act    
            var viewModel = commentService.GetCommentsAndQuestions(sourceURI);

            //Assert
            viewModel.Comments.Single().CommentText.ShouldBe(commentText);
            var question = viewModel.Questions.Single();
            question.QuestionText.ShouldBe(questionText);
            question.Answers.Single().AnswerText.ShouldBe(answerText);


            //sample code:
            
            var resp = await _client.GetAsync($"/consultations/api/Comments?sourceURI={WebUtility.UrlEncode(sourceURI)}");
            resp.EnsureSuccessStatusCode();
            var responseString = await resp.Content.ReadAsStringAsync();
            var anotherQuestion = JsonConvert.DeserializeObject<CommentsAndQuestions>(responseString);
            anotherQuestion.Questions.Single().Answers.Single().AnswerText.ShouldBe(answerText);
            
        }

        

        [Theory]
        [InlineData(1, 1, null, null)]
        [InlineData(1, 1, "a chapter for a supporting document - not valid", null)]
        [InlineData(2, 2, "first-document-first-chapter-slug", "first-document-first-chapter-slug")]
        [InlineData(2, 2, "first-document-second-chapter-slug", "first-document-second-chapter-slug")]
        [InlineData(3, 3, "first-document-first-chapter-slug", "second-document-first-chapter-slug")]
        [InlineData(2, 2, null, "first-document-first-chapter-slug")]
        [InlineData(-1, 2, null, "first-document-first-chapter-slug")]
        [InlineData(int.MaxValue, 2, null, "first-document-first-chapter-slug")]
        public void EnsureDocumentAndChapterAreValidWithinConsultation_Validation(int documentIdIn, int documentIdOut, string chapterSlugIn, string chapterSlugOut)
        {
            // Arrange
            var consultation = new ConsultationDetail(null, null, null, DateTime.MinValue, DateTime.MaxValue, null, null, null,
                null, null, null,  1, null, true, null, null, 
                new List<Document>
                {
                    new Document(1, false, "supporting document", null),
                    new Document(2, true, "first commentable document", new List<Chapter>
                    {
                        new Chapter("first-document-first-chapter-slug", "first-document-first-chapter-title"),
                        new Chapter("first-document-second-chapter-slug", "first-document-second-chapter-title")
                    }),
                    new Document(3, true, "second commentable document", new List<Chapter>
                    {
                        new Chapter("second-document-first-chapter-slug", "second-document-first-chapter-title")
                    })
                }, 
                user: null);
            var feedReaderService = new FeedReader(Feed.ConsultationCommentsListDetailMulitpleDoc);
            var consultationService = new ConsultationService(new FeedConverterService(feedReaderService), new FakeLogger<ConsultationService>(), FakeUserService.Get(false));

            // Act
            var (validatedDocumentId, validatedChapterSlug) = consultationService.ValidateDocumentAndChapterWithinConsultation(consultation, documentIdIn, chapterSlugIn);

            //Assert
            validatedDocumentId.ShouldBe(documentIdOut);
            validatedChapterSlug.ShouldBe(chapterSlugOut);
        }

    }
}
