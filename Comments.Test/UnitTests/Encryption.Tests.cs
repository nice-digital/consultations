using System;
using System.Security.Cryptography;
using FluentAssertions;
using Shouldly;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.UnitTests
{
    public class EncryptionTests : TestBase
	{
		[Fact]
		public void Encrypt_And_Decrypt_String()
		{
			//Arrange
			string original = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus id ullamcorper massa. Integer ut tincidunt sapien. Vivamus tincidunt dui ac blandit blandit. Pellentesque id commodo purus. Nulla pulvinar est et ultricies tincidunt. Nulla eget semper nisl. Mauris non posuere mi, et condimentum augue. Quisque erat libero, gravida quis arcu non, lobortis porttitor quam. Suspendisse cursus lorem turpis, at ullamcorper quam ornare ut.";
			string encrypted, decrypted = null;
			var encryption = new Encryption();

			//Act
			using (Aes myAes = Aes.Create()) // Create a new instance of the Aes class. This generates a new key and initialization vector (IV).
			{
				encrypted = encryption.EncryptString(original, myAes.Key, myAes.IV);
				decrypted = encryption.DecryptString(encrypted, myAes.Key, myAes.IV);
			}

			//Assert
			encrypted.ShouldNotBe(original);
			decrypted.ShouldBe(original);
		}

		[Fact]
		public void Blank_String_Throws_Error()
		{
			//Arrange
			string original = "";
			var encryption = new Encryption();

			using (Aes myAes = Aes.Create())
			{
				//Act + Assert
				Should.Throw<ArgumentNullException>(() => encryption.EncryptString(original, myAes.Key, myAes.IV));
			}
		}
	}
}
