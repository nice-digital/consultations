using Comments.Models;

namespace Comments.ViewModels
{
	public class OrganisationCode
	{
		public OrganisationCode(int organisationAuthorisationId, int organisationId, string organisationName, string collationCode)
		{
			OrganisationAuthorisationId = organisationAuthorisationId;
			OrganisationId = organisationId;
			OrganisationName = organisationName;
			CollationCode = collationCode;
		}

		public OrganisationCode(Models.OrganisationAuthorisation organisationAuthorisation, string organisationName)
		{
			OrganisationAuthorisationId = organisationAuthorisation.OrganisationAuthorisationId;
			OrganisationId = organisationAuthorisation.OrganisationId;
			CollationCode = organisationAuthorisation.CollationCode;

			OrganisationName = organisationName;
		}

		public int OrganisationAuthorisationId { get; private set; }
		public int OrganisationId { get; private set; }
		public string OrganisationName { get; private set; }
		public string CollationCode { get; private set; }
	}
}
