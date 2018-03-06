using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
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
            var consultationId = RandomNumber();
            var documentId = RandomNumber();
            var commentText = Guid.NewGuid().ToString();

            var locationId = AddLocation(consultationId, documentId);
            AddComment(locationId, commentText, isDeleted: false);

            // Act
            DocumentViewModel viewModel;
            using (var consultationsContext = new ConsultationsContext(_options))
            {
                var consultationService = new CommentService(consultationsContext);
                viewModel = consultationService.GetAllCommentsAndQuestionsForDocument(consultationId, documentId, "chapter-slug");
            }

            //Assert
            viewModel.Comments.Single().CommentText.ShouldBe(commentText);
        }

        [Fact]
        public void CommentsQuestionsAndAnswers_CanBeRead()
        {
            // Arrange
            ResetDatabase();
            var consultationId = RandomNumber();
            var documentId = RandomNumber();
            var commentText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var answerText = Guid.NewGuid().ToString();

            AddCommentsAndQuestionsAndAnswers(consultationId, documentId, commentText, questionText, answerText);

            // Act
            DocumentViewModel viewModel;
            using (var consultationsContext = new ConsultationsContext(_options))
            {
                var consultationService = new CommentService(consultationsContext);
                viewModel = consultationService.GetAllCommentsAndQuestionsForDocument(consultationId, documentId, "chapter-slug");
            }

            //Assert
            viewModel.Comments.Single().CommentText.ShouldBe(commentText);
            var question = viewModel.Questions.Single();
            question.QuestionText.ShouldBe(questionText);
            question.Answers.Single().AnswerText.ShouldBe(answerText);
        }

        [Theory]
        [InlineData(1, 1, "first-document-first-chapter-slug", "first-document-first-chapter-slug")]
        //[InlineData(1, 1, "first-document-second-chapter-slug", "first-document-second-chapter-slug")]
        //[InlineData(2, 2, "first-document-first-chapter-slug", "second-document-first-chapter-slug")]
        //[InlineData(1, 1, null, "first-document-first-chapter-slug")]
        //[InlineData(null, 1, null, "first-document-first-chapter-slug")]
        //[InlineData(-1, 1, null, "first-document-first-chapter-slug")]
        //[InlineData(int.MaxValue, 1, null, "first-document-first-chapter-slug")]
        public void EnsureDocumentAndChapterAreValidWithinConsultation_Validation(int? documentIdIn, int? documentIdOut, string chapterSlugIn, string chapterSlugOut)
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
                    new Document(2, true, "second commentable document", new List<Chapter>
                    {
                        new Chapter("second-document-first-chapter-slug", "second-document-first-chapter-title")
                    })
                });

            // Act
            using (var consultationsContext = new ConsultationsContext(_options))
            {
                var consultationService = new CommentService(consultationsContext);
                consultationService.EnsureDocumentAndChapterAreValidWithinConsultation(consultation, ref documentIdIn, ref chapterSlugIn);
            }

            //Assert
            documentIdIn.ShouldBe(documentIdOut);
            chapterSlugIn.ShouldBe(chapterSlugOut);
        }
    }
}
