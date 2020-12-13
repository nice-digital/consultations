using System;

namespace Comments.Models
{
	public class ValidatedSession
	{
		public ValidatedSession(int organisationUserId, int consultationId, Guid sessionId, int organisationId)
		{
			OrganisationUserId = organisationUserId;
			ConsultationId = consultationId;
			SessionId = sessionId;
			OrganisationId = organisationId;
		}

		public int OrganisationUserId { get; private set; }
		public int ConsultationId { get; private set; }
		public Guid SessionId { get; private set; }
		public int OrganisationId { get; private set; }
	}
}
