using Comments.Models;

namespace Comments.ViewModels
{
	public class OrganisationCode
	{
		public OrganisationCode(){}

		public OrganisationCode(int organisationAuthorisationId, int organisationId, string organisationName, string collationCode)
		{
			OrganisationAuthorisationId = organisationAuthorisationId;
			OrganisationId = organisationId;
			OrganisationName = organisationName;
			_collationCode = collationCode;
		}

		public OrganisationCode(Models.OrganisationAuthorisation organisationAuthorisation, string organisationName)
		{
			OrganisationAuthorisationId = organisationAuthorisation.OrganisationAuthorisationId;
			OrganisationId = organisationAuthorisation.OrganisationId;
			_collationCode = organisationAuthorisation.CollationCode;

			OrganisationName = organisationName;
		}

		public int OrganisationAuthorisationId { get; private set; }
		public int OrganisationId { get; private set; }
		public string OrganisationName { get; private set; }


		private string _collationCode;
		/// <summary>
		/// The user in the front end should see the collation code chunked up like this: "1234 1234 1234".
		/// database-wise though, we ignore spaces and just save the 12 numbers.
		/// </summary>
		public string CollationCode
		{
			get
			{
				if (!string.IsNullOrEmpty(_collationCode) && _collationCode.Length == 12)
				{
					return $"{_collationCode.Substring(0, 4)} {_collationCode.Substring(4, 4)} {_collationCode.Substring(8, 4)}";
				}
				return _collationCode;
			}
			set => _collationCode = !string.IsNullOrEmpty(value) ? value.Replace(" ", "") : value;
		}
	}
}
