using Microsoft.AspNetCore.Authentication;

namespace Comments.Auth
{
    public class AuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "NICE Accounts";
        public string Scheme => DefaultScheme;
    }
}
