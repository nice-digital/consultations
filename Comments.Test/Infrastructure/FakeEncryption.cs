using System;

namespace Comments.Test.Infrastructure
{
    public class FakeEncryption : IEncryption
    {
	    public string EncryptString(string stringToEncrypt)
		{
		    throw new NotImplementedException();
	    }

		public string DecryptString(string stringToDecrypt)
		{
		    throw new NotImplementedException();
	    }
    }
}
