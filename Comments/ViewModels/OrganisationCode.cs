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

		public int OrganisationAuthorisationId { get; private set; }
		public int OrganisationId { get; private set; }
		public string OrganisationName { get; private set; }
		public string CollationCode { get; private set; }
	}
}
