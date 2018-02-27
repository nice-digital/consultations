using System;
using System.Linq;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
    public class UnitUnitTests : UnitTestBase
    {
        [Fact]
        public void Comments_CanBeRead()
        {
            ReinitialiseDatabase();
            var consultationId = RandomNumber();
            var documentId = RandomNumber();
            var commentText = Guid.NewGuid().ToString();

            var locationId = AddLocation(consultationId, documentId);
            AddComment(locationId, commentText);

            DocumentViewModel viewModel;
            using (var consultationsContext = new ConsultationsContext(_options))
            {
                var consultationService = new ConsultationService(consultationsContext);
                viewModel = consultationService.GetAllCommentsAndQuestionsForDocument(consultationId, documentId);
            }
            viewModel.Comments.Single().CommentText.ShouldBe(commentText);
        }

        [Fact]
        public void CommentsQuestionsAndAnswers_CanBeRead()
        {
            ReinitialiseDatabase();
            var consultationId = RandomNumber();
            var documentId = RandomNumber();
            var commentText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var answerText = Guid.NewGuid().ToString();

            AddCommentsAndQuestionsAndAnswers(consultationId, documentId, commentText, questionText, answerText);

            DocumentViewModel viewModel;
            using (var consultationsContext = new ConsultationsContext(_options))
            {
                var consultationService = new ConsultationService(consultationsContext);
                viewModel = consultationService.GetAllCommentsAndQuestionsForDocument(consultationId, documentId);
            }

            viewModel.Comments.Single().CommentText.ShouldBe(commentText);
            var question = viewModel.Questions.Single();
            question.QuestionText.ShouldBe(questionText);
            question.Answers.Single().AnswerText.ShouldBe(answerText);
        }
    }
}
