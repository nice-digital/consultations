using Comments.Services;
using Comments.ViewModels;
using System.Collections.Generic;
using Answer = Comments.Models.Answer;
using Comment = Comments.Models.Comment;
using Location = Comments.Models.Location;
using Question = Comments.Models.Question;

namespace Comments.Test.Infrastructure
{
	public class FakeExportService: IExportService
	{
		private readonly IEnumerable<Comment> _comments;
		private readonly IEnumerable<Answer> _answers;
		private readonly IEnumerable<Question> _questions;

		public FakeExportService(IEnumerable<Models.Comment> comments, IEnumerable<Models.Answer> answers, IEnumerable<Models.Question> questions)
		{
			_comments = comments;
			_answers = answers;
			_questions = questions;
		}

		public (IEnumerable<Comment> comment, IEnumerable<Answer> answer, IEnumerable<Question> question, Validate valid) GetAllDataForConsulation(
			int consultationId)
		{
			return (_comments, _answers, _questions, new Validate(true));
		}

		public (IEnumerable<Comment> comment, IEnumerable<Answer> answer, IEnumerable<Question> question, Validate valid) GetAllDataForConsulationForCurrentUser(
			int consultationId)
		{
			return GetAllDataForConsulation(consultationId);
		}

		public (string ConsultationName, string DocumentName, string ChapterName) GetLocationData(Location location)
		{
			return ("Test consultation", "Test document", "Test chapter");
		}

		public string GetConsultationName(Location location)
		{
			return "Test consultation";
		}
	}
}
