using System;
using System.IO;
using System.Security.Cryptography;

namespace Comments
{
	public interface IEncryption
	{
		string EncryptString(string stringToEncrypt, byte[] key, byte[] iv);
		string DecryptString(string stringToDecrypt, byte[] key, byte[] iv);
	}

    public class Encryption : IEncryption
    {
	    public string EncryptString(string plainText, byte[] key, byte[] iv)
	    {
		    if (plainText == null || plainText.Length <= 0)
			    return string.Empty; //throw new ArgumentNullException(nameof(plainText));
			if (key == null || key.Length <= 0)
			    throw new ArgumentNullException(nameof(key));
		    if (iv == null || iv.Length <= 0)
			    throw new ArgumentNullException(nameof(iv));

			byte[] encrypted;

		    using (Aes aes = Aes.Create())
		    {
			    aes.Key = key;
			    aes.IV = iv;

			    // Create a decrytor to perform the stream transform.
			    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);


			    using (MemoryStream memoryStream = new MemoryStream())
			    {
				    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				    {
					    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
					    {
						    streamWriter.Write(plainText);
					    }
					    encrypted = memoryStream.ToArray();
				    }
			    }
		    }
			
		    return Convert.ToBase64String(encrypted);
		}

	    public string DecryptString(string cipherText, byte[] key, byte[] iv)
	    {

		    if (cipherText == null || cipherText.Length <= 0)
			    return string.Empty; //throw new ArgumentNullException(nameof(cipherText));
		    if (key == null || key.Length <= 0)
			    throw new ArgumentNullException(nameof(key));
		    if (iv == null || iv.Length <= 0)
			    throw new ArgumentNullException(nameof(iv));

			var cipherBytes = Convert.FromBase64String(cipherText);
			string plaintext = null;

			using (Aes aes = Aes.Create())
		    {
			    aes.Key = key;
			    aes.IV = iv;

			    // Create a decrytor to perform the stream transform.
			    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
				
			    using (MemoryStream memoryStream = new MemoryStream(cipherBytes))
			    {
				    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
				    {
					    using (StreamReader streamReader = new StreamReader(cryptoStream))
					    {
							plaintext = streamReader.ReadToEnd();
					    }
				    }
			    }
		    }

		    return plaintext;
		}
	}
}
