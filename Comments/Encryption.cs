using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace Comments
{

	public interface IEncryption
	{
		string EncryptString(string stringToEncrypt, byte[] key, byte[] iv);
		string DecryptString(string stringToDecrypt, byte[] key, byte[] iv);
	}

    public class Encryption : IEncryption
    {
		public Encryption(IDataProtectionProvider provider)
		{
		}

	    public string EncryptString(string plainText, byte[] key, byte[] iv)
	    {
		    // Check arguments.
		    if (plainText == null || plainText.Length <= 0)
			    throw new ArgumentNullException("plainText");
		    if (key == null || key.Length <= 0)
			    throw new ArgumentNullException("Key");
		    if (iv == null || iv.Length <= 0)
			    throw new ArgumentNullException("IV");

			byte[] encrypted;

		    using (Aes aesAlg = Aes.Create())
		    {
			    aesAlg.Key = key;
			    aesAlg.IV = iv;

			    // Create a decrytor to perform the stream transform.
			    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

			    // Create the streams used for encryption.
			    using (MemoryStream memoryStream = new MemoryStream())
			    {
				    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				    {
					    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
					    {
						    //Write all data to the stream.
						    streamWriter.Write(plainText);
					    }
					    encrypted = memoryStream.ToArray();
				    }
			    }
		    }

		    // Return the encrypted bytes from the memory stream.
		    return Convert.ToBase64String(encrypted);
		}

	    public string DecryptString(string cipherText, byte[] key, byte[] iv)
	    {
		    // Check arguments.
		    if (cipherText == null || cipherText.Length <= 0)
			    throw new ArgumentNullException("cipherText");
		    if (key == null || key.Length <= 0)
			    throw new ArgumentNullException("Key");
		    if (iv == null || iv.Length <= 0)
			    throw new ArgumentNullException("IV");

			var cipherBytes = Convert.FromBase64String(cipherText);
			string plaintext = null;

			using (Aes aesAlg = Aes.Create())
		    {
			    aesAlg.Key = key;
			    aesAlg.IV = iv;

			    // Create a decrytor to perform the stream transform.
			    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

			    // Create the streams used for decryption.
			    using (MemoryStream memoryStream = new MemoryStream(cipherBytes))
			    {
				    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
				    {
					    using (StreamReader streamReader = new StreamReader(cryptoStream))
					    {
						    // Read the decrypted bytes from the decrypting stream and place them in a string.
							plaintext = streamReader.ReadToEnd();
					    }
				    }
			    }
		    }

		    return plaintext;
		}
	}
}
