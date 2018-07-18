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
		public void Encrypt_String()
		{
			//Arrange
			byte[] encrypted;
			var stringToEncrypt = "Hello World";
			//	"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus id ullamcorper massa. Integer ut tincidunt sapien. Vivamus tincidunt dui ac blandit blandit. Pellentesque id commodo purus. Nulla pulvinar est et ultricies tincidunt. Nulla eget semper nisl. Mauris non posuere mi, et condimentum augue. Quisque erat libero, gravida quis arcu non, lobortis porttitor quam. Suspendisse cursus lorem turpis, at ullamcorper quam ornare ut.";

			//Act
			using (Aes myAes = Aes.Create())
			{
				// Encrypt the string to an array of bytes.
				encrypted = _fakeEncryption.EncryptString(stringToEncrypt, myAes.Key, myAes.IV);

			}

			//Assert
			encrypted.ShouldNotBeSameAs(stringToEncrypt);
		}

		[Fact]
		public void Decrypt_String()
		{
			//Arrange
			string decryptedString = "";
			var stringToEncrypt = "Hello World";
			//	"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus id ullamcorper massa. Integer ut tincidunt sapien. Vivamus tincidunt dui ac blandit blandit. Pellentesque id commodo purus. Nulla pulvinar est et ultricies tincidunt. Nulla eget semper nisl. Mauris non posuere mi, et condimentum augue. Quisque erat libero, gravida quis arcu non, lobortis porttitor quam. Suspendisse cursus lorem turpis, at ullamcorper quam ornare ut.";


			//Act
			using (Aes myAes = Aes.Create())
			{ 
				var encryptedString = _fakeEncryption.EncryptString(stringToEncrypt, myAes.Key, myAes.IV);
				decryptedString = _fakeEncryption.DecryptString(encryptedString, myAes.Key, myAes.IV);
			}

			//Assert
			decryptedString.ShouldBe(stringToEncrypt);
		}

		[Fact]
		public void Test_AES()
		{
			var key = Encoding.ASCII.GetBytes("15CV1/ZOnVI3rY4wk4INBg==");
			var iv = Encoding.ASCII.GetBytes("3fd05ba338d0be68");

			//string original = "Here is some data to encrypt!";
			string original = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus id ullamcorper massa. Integer ut tincidunt sapien. Vivamus tincidunt dui ac blandit blandit. Pellentesque id commodo purus. Nulla pulvinar est et ultricies tincidunt. Nulla eget semper nisl. Mauris non posuere mi, et condimentum augue. Quisque erat libero, gravida quis arcu non, lobortis porttitor quam. Suspendisse cursus lorem turpis, at ullamcorper quam ornare ut.";

			// Create a new instance of the Aes class. This generates a new key and initialization vector (IV).
			using (Aes myAes = Aes.Create())
			{
				myAes.Key = key;
				myAes.IV = iv;

				// Encrypt the string to an array of bytes.
				byte[] encrypted = _fakeEncryption.EncryptString(original, myAes.Key, myAes.IV);
				string encryptedString = _fakeEncryption.ConvertByteArrayToString(encrypted);

				// Decrypt the bytes to a string.
				byte[] encryptedBytes = _fakeEncryption.ConvertStringToByteArray(encryptedString);
				string roundtrip = _fakeEncryption.DecryptString(encrypted, myAes.Key, myAes.IV);

				roundtrip.ShouldBe(original);
				encrypted.ShouldBe(encryptedBytes);
			}
		}
	}
}
