using System;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Routing;

namespace Comments.ViewModels
{
	public class Error : LayoutBaseModel
    {
	    public string RequestedPath { get; }
	    public Exception ErrorException { get; }

	    public Error(string requestedPath, Exception errorException, IPrincipal user, string signInURL, string signOutURL) : base(user, signInURL, signOutURL)
	    {
		    RequestedPath = requestedPath;
		    ErrorException = errorException;
		}
	}
}
