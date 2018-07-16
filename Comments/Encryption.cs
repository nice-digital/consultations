using Microsoft.AspNetCore.DataProtection;

namespace Comments
{

	public interface IEncryption
	{
		string EncryptString(string stringToEncrypt);
		string DecryptString(string stringToDecrypt);
	}

    public class Encryption : IEncryption
    {
	    readonly IDataProtector _protector;

		public Encryption(IDataProtectionProvider provider)
		{
			_protector = provider.CreateProtector("Consultations.Encryption");
		}

	    public string EncryptString(string stringToEncrypt)
	    {
		    return _protector.Protect(stringToEncrypt);
		}

	    public string DecryptString(string stringToDecrypt)
	    {
		    return _protector.Unprotect(stringToDecrypt);
	    }
	}
}
