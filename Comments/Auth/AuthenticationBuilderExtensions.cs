using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Comments.Auth
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddAuth(this AuthenticationBuilder builder, Action<AuthOptions> configureOptions)
        {
            return builder.AddScheme<AuthOptions, AuthHandler>(AuthOptions.DefaultScheme, configureOptions);
        }
    }
}
