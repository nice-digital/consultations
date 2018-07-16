using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Xunit.Sdk;

namespace Comments.Test.UnitTests
{
    public class EncryptionTests : Comments.Test.Infrastructure.TestBase
	{
		public EncryptionTests() : base(useRealSubmitService: false, useRealEncryption: true) { }

		[Fact]
	    public void Encrypt_String()
	    {
			//Arrange
		    var stringToEncrypt = "Hello World!";

			//Act
			var encryptedString = _fakeEncryption.EncryptString(stringToEncrypt);

			//Assert
			encryptedString.ShouldNotBe(stringToEncrypt);
	    }

		[Fact]
		public void Decrypt_String()
		{
			//Arrange
			var stringToEncrypt = "Hello World!";

			//Act
			var encryptedString = _fakeEncryption.EncryptString(stringToEncrypt);
			var decryptedString = _fakeEncryption.DecryptString(encryptedString);

			//Assert
			decryptedString.ShouldBe(stringToEncrypt);
		}
	}
}
