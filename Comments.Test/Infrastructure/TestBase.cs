using System;
using System.Collections.Generic;
using Comments.Models;
using Microsoft.EntityFrameworkCore;
using Moq;


namespace Comments.Test.Infrastructure
{
    public class TestBase
    {
        protected readonly DbContextOptions<ConsultationsContext> _options;

        public TestBase()
        {
            _options = new DbContextOptionsBuilder<ConsultationsContext>()
                .UseInMemoryDatabase(databaseName: "test_db")
                .Options;
        }

        #region database stuff

        protected void ReinitialiseDatabase()
        {
            using (var context = new ConsultationsContext(_options))
            {
                context.Database.EnsureDeleted();
            }
        }
        protected int AddLocation(int consultationId, int documentId)
        {
            var location = new Location(consultationId, documentId, null, null, null, null, null, null, null, null, null);
            using (var context = new ConsultationsContext(_options))
            {
                context.Location.Add(location);
                context.SaveChanges();
            }
            return location.LocationId;
        }
        protected int AddComment(int locationId, string commentText)
        {
            var comment = new Comment(locationId, Guid.Empty, commentText, DateTime.Now, null);
            using (var context = new ConsultationsContext(_options))
            {
                context.Comment.Add(comment);
                context.SaveChanges();
            }
            return comment.CommentId;
        }
        protected int AddQuestionType(string description, bool hasBooleanAnswer, bool hasTextAnswer, int questionTypeId = 1)
        {
            var questionType = new QuestionType(description, hasTextAnswer, hasBooleanAnswer, null);
            using (var context = new ConsultationsContext(_options))
            {
                context.QuestionType.Add(questionType);
                context.SaveChanges();
            }
            return questionType.QuestionTypeId;
        }
        protected int AddQuestion(int locationId, int questionTypeId, string questionText, int questionId = 1)
        {
            var question = new Question(locationId, questionText, questionTypeId, null, null, null, null);
            using (var context = new ConsultationsContext(_options))
            {
                context.Question.Add(question);
                context.SaveChanges();
            }
            return question.QuestionId;
        }
        protected int AddAnswer(int questionId, Guid userId, string answerText)
        {
            var answer = new Answer(questionId, userId, answerText, null, null);
            using (var context = new ConsultationsContext(_options))
            {
                answer.LastModifiedDate = DateTime.Now;
                context.Answer.Add(answer);
                context.SaveChanges();
            }
            return answer.AnswerId;
        }
        protected void AddCommentsAndQuestionsAndAnswers(int consultationId, int documentId, string commentText, string questionText, string answerText)
        {
            var locationId = AddLocation(consultationId, documentId);
            AddComment(locationId, commentText);
            var questionTypeId = AddQuestionType(description: "text", hasBooleanAnswer: false, hasTextAnswer: true);
            var questionId = AddQuestion(locationId, questionTypeId, questionText);
            AddAnswer(questionId, Guid.Empty, answerText);
        }

        #endregion database stuff

        #region Helper

        protected int RandomNumber()
        {
            var rnd = new Random();
            return rnd.Next(1, int.MaxValue);
        }

        #endregion Helper
    }
}
