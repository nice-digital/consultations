using System;

namespace Comments.Test.Infrastructure
{
    public class FakeEncryption : IEncryption
    {
	    public string EncryptString(string stringToEncrypt, byte[] key, byte[] iv)
		{
		    throw new NotImplementedException();
	    }

		public string DecryptString(string stringToDecrypt, byte[] key, byte[] iv)
		{
		    throw new NotImplementedException();
	    }

	    public string ConvertByteArrayToString(byte[] cipherText)
	    {
			throw new NotImplementedException();
		}

	    public Byte[] ConvertStringToByteArray(string text)
	    {
			throw new NotImplementedException();
		}
	}
}
