using Comments.Test.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace Comments.Test.IntegrationTests.API.OrganisationAuthorisation
{
	public class OrganisationAuthorisationTests : TestBase
	{
		[Fact]
		public async Task GenerateCode()
		{
			//Arrange (in the base constructor for this one.)

			// Act
			var response = await _client.PostAsync("/consultations/api/OrganisationAuthorisation?organisationId=1&consultationId=1", null);
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			responseString.ShouldMatchApproved();
		}

	}
}
