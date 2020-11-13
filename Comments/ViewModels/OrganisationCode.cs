namespace Comments.ViewModels
{
	public class OrganisationCode
	{
		public OrganisationCode(int organisationId, string organisationName, string collationCode)
		{
			OrganisationId = organisationId;
			OrganisationName = organisationName;
			CollationCode = collationCode;
		}

		public int OrganisationId { get; private set; }
		public string OrganisationName { get; private set; }
		public string CollationCode { get; private set; }
	}
}
