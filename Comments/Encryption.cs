using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Comments.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Comments
{

	public interface IEncryption
	{
		string EncryptString(string stringToEncrypt, byte[] key, byte[] iv);
		string DecryptString(string stringToDecrypt, byte[] key, byte[] iv);
		string ConvertByteArrayToString(byte[] cipherText);
		Byte[] ConvertStringToByteArray(string text);
	}

    public class Encryption : IEncryption
    {
	    readonly IDataProtector _protector;
		private byte[] _mykey = new byte[16];
	    private byte[] _myIV = new byte[16];

		public Encryption(IDataProtectionProvider provider)
		{
			_protector = provider.CreateProtector("Consultations.Encryption");
		}

	    public string EncryptString(string plainText, byte[] key, byte[] iv)
	    {
		    byte[] encrypted;

		    using (Aes aesAlg = Aes.Create())
		    {
			    aesAlg.Key = key;
			    aesAlg.IV = iv;

			    // Create a decrytor to perform the stream transform.
			    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key
				    , aesAlg.IV);

			    // Create the streams used for encryption.
			    using (MemoryStream msEncrypt = new MemoryStream())
			    {
				    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt
					    , encryptor, CryptoStreamMode.Write))
				    {
					    using (StreamWriter swEncrypt = new StreamWriter(
						    csEncrypt))
					    {

						    //Write all data to the stream.
						    swEncrypt.Write(plainText);
					    }
					    encrypted = msEncrypt.ToArray();
				    }
			    }
		    }

		    // Return the encrypted bytes from the memory stream.
		    return ConvertByteArrayToString(encrypted);
		}

	    public string DecryptString(string cipherText, byte[] key, byte[] iv)
	    {
		    var cipherBytes = ConvertStringToByteArray(cipherText);
			string plaintext = null;

			using (Aes aesAlg = Aes.Create())
		    {
			    aesAlg.Key = key;
			    aesAlg.IV = iv;

			    // Create a decrytor to perform the stream transform.
			    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

			    // Create the streams used for decryption.
			    using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
			    {
				    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt
					    , decryptor, CryptoStreamMode.Read))
				    {
					    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
					    {
						    // Read the decrypted bytes from the decrypting stream
							// and place them in a string.
							plaintext = srDecrypt.ReadToEnd();
					    }
				    }
			    }
		    }

		    return plaintext;
		}

	    public string ConvertByteArrayToString(byte[] cipherText)
	    {
		    var convertedString = Convert.ToBase64String(cipherText);
			return convertedString;
	    }

	    public Byte[] ConvertStringToByteArray(string text)
	    {
		    var convertedBytes = Convert.FromBase64String(text);
		    return convertedBytes;
	    }
	}
}
