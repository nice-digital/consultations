using Comments.Services;
using Comments.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Comments.Models;

namespace Comments.Test.Infrastructure
{
	public class FakeOrganisationService : IOrganisationService
	{
		private readonly Dictionary<int, string> _organisations;

		public FakeOrganisationService(Dictionary<int, string> organisations = null)
		{
			_organisations = organisations;
		}

		public OrganisationCode GenerateOrganisationCode(int organisationId, int consultationId)
		{
			throw new NotImplementedException();
		}

		public Task<OrganisationCode> CheckValidCodeForConsultation(string collationCode, int consultationId)
		{
			throw new NotImplementedException();
		}

		public (Guid sessionId, DateTime expirationDate) CreateOrganisationUserSession(int organisationAuthorisationId,
			string collationCode)
		{
			throw new NotImplementedException();
		}

		public Task<(bool valid, string organisationName)> CheckOrganisationUserSession(int consultationId)
		{
			throw new NotImplementedException();
		}

		public IList<ValidatedSession> CheckValidCodesForConsultation(Session unvalidatedSessions)
		{
			throw new NotImplementedException();
		}

		public Task<Dictionary<int, string>> GetOrganisationNames(IEnumerable<int> organisationIds)
		{
			return Task.Run((() => _organisations));
		}

		public List<string> GetEmailAddressForComment(CommentsAndQuestions commentsAndQuestions)
		{
			throw new NotImplementedException();
		}
	}
}
