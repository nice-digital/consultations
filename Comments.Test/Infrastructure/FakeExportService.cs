using System;
using Comments.Services;
using Comments.ViewModels;
using System.Collections.Generic;
using Comments.Models;
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
		private readonly IEnumerable<OrganisationUser> _organisationUsers;

		public FakeExportService(IEnumerable<Models.Comment> comments, IEnumerable<Models.Answer> answers, IEnumerable<Models.Question> questions, IEnumerable<OrganisationUser> organisationUsers)
		{
			_comments = comments;
			_answers = answers;
			_questions = questions;
			_organisationUsers = organisationUsers;
		}

		public (IEnumerable<Comment> comment, IEnumerable<Answer> answer, IEnumerable<Question> question, Validate valid) GetAllDataForConsultation(
			int consultationId)
		{
			return (_comments, _answers, _questions, new Validate(true));
		}

		public (IEnumerable<Comment> comment, IEnumerable<Answer> answer, IEnumerable<Question> question, Validate valid) GetAllDataForConsultationForCurrentUser(
			int consultationId)
		{
			return GetAllDataForConsultation(consultationId);
		}

		public (string ConsultationName, string DocumentName, string ChapterName) GetLocationData(Location location)
		{
			return ("Test consultation", "Test document", "Test chapter");
		}

		public string GetConsultationName(Location location)
		{
			return "Test consultation";
		}

		public IEnumerable<Models.OrganisationUser> GetOrganisationUsersByOrganisationUserIds(IEnumerable<int> organisationUserIds)
		{
			return _organisationUsers;
		}
	}
}
