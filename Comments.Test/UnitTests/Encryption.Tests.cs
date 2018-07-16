using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
    public class EncryptionTests : Comments.Test.Infrastructure.TestBase
	{
	    [Fact]
	    public void Encrypt_String()
	    {
		    var stringToEncrypt = "Hello World!";
		
		    var serviceCollection = new ServiceCollection();
		    serviceCollection.AddDataProtection();
		    var services = serviceCollection.BuildServiceProvider();

		    // create an instance of MyClass using the service provider
		    var instance = ActivatorUtilities.CreateInstance<Encryption>(services);
		    var encryptedString = instance.EncryptString(stringToEncrypt);

		    encryptedString.ShouldNotBe(stringToEncrypt);
	    }

		[Fact]
		public void Decrypt_String()
		{
			var stringToEncrypt = "Hello World!";

			var serviceCollection = new ServiceCollection();
			serviceCollection.AddDataProtection();
			var services = serviceCollection.BuildServiceProvider();

			// create an instance of MyClass using the service provider
			var instance = ActivatorUtilities.CreateInstance<Encryption>(services);
			var encryptedString = instance.EncryptString(stringToEncrypt);
			var decryptedString = instance.DecryptString(encryptedString);

			decryptedString.ShouldBe(stringToEncrypt);
		}
	}
}
