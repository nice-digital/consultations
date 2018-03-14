using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using NICE.Feeds;
using NICE.Feeds.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
    public class Tests : TestBase
    {
        [Fact]
        public void Comments_CanBeRead()
        { 
            // Arrange
            ResetDatabase();
            var sourceURL = "/consultations/1/1/introduction";
            var commentText = Guid.NewGuid().ToString();

            var locationId = AddLocation(sourceURL);
            AddComment(locationId, commentText, isDeleted: false);
            var feedReaderService = new FakeFeedReaderService(Feed.ConsultationCommentsListDetailMulitpleDoc);
            var commentService = new CommentService(new ConsultationsContext(_options), new ConsultationService(feedReaderService, new FeedConverterConverterService(feedReaderService)));
            
            // Act
            var viewModel = commentService.GetCommentsAndQuestions(sourceURL);

            //Assert
            viewModel.Comments.Single().CommentText.ShouldBe(commentText);
        }

        [Fact]
        public void CommentsQuestionsAndAnswers_CanBeRead()
        {
            // Arrange
            ResetDatabase();
            var sourceURL = "/consultations/1/1/introduction";
            var commentText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var answerText = Guid.NewGuid().ToString();

            AddCommentsAndQuestionsAndAnswers(sourceURL, commentText, questionText, answerText);
            var feedReaderService = new FakeFeedReaderService(Feed.ConsultationCommentsListDetailMulitpleDoc);
            var commentService = new CommentService(new ConsultationsContext(_options), new ConsultationService(feedReaderService, new FeedConverterConverterService(feedReaderService)));

            // Act    
            var viewModel = commentService.GetCommentsAndQuestions(sourceURL);

            //Assert
            viewModel.Comments.Single().CommentText.ShouldBe(commentText);
            var question = viewModel.Questions.Single();
            question.QuestionText.ShouldBe(questionText);
            question.Answers.Single().AnswerText.ShouldBe(answerText);
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
                });
            var feedReaderService = new FakeFeedReaderService(Feed.ConsultationCommentsListDetailMulitpleDoc);
            var consultationService = new ConsultationService(feedReaderService, new FeedConverterConverterService(feedReaderService));

            // Act
            var (validatedDocumentId, validatedChapterSlug) = consultationService.ValidateDocumentAndChapterWithinConsultation(consultation, documentIdIn, chapterSlugIn);

            //Assert
            validatedDocumentId.ShouldBe(documentIdOut);
            validatedChapterSlug.ShouldBe(chapterSlugOut);
        }
    }
}
