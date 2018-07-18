using System;
using System.Security.Cryptography;
using System.Text;
using Comments.Configuration;
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
		public void Encrypt_And_Decrypt_String()
		{
			string original = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus id ullamcorper massa. Integer ut tincidunt sapien. Vivamus tincidunt dui ac blandit blandit. Pellentesque id commodo purus. Nulla pulvinar est et ultricies tincidunt. Nulla eget semper nisl. Mauris non posuere mi, et condimentum augue. Quisque erat libero, gravida quis arcu non, lobortis porttitor quam. Suspendisse cursus lorem turpis, at ullamcorper quam ornare ut.";

			// Create a new instance of the Aes class. This generates a new key and initialization vector (IV).
			using (Aes myAes = Aes.Create())
			{
				// Encrypt the string to an array of bytes.
				var encrypted = _fakeEncryption.EncryptString(original, myAes.Key, myAes.IV);

				// Decrypt the bytes to a string.
				string roundtrip = _fakeEncryption.DecryptString(encrypted, myAes.Key, myAes.IV);

				encrypted.ShouldNotBe(original);
				roundtrip.ShouldBe(original);
			}
		}
	}
}
