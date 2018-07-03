using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Comments.ViewModels
{
    public class Error
    {
	    public string RequestedPath { get; }
		public string CustomMessage { get; }
	    public Exception ErrorException { get; }

	    public Error(string requestedPath, Exception errorException, string urlEncodedMessage)
	    {
		    RequestedPath = requestedPath;
		    ErrorException = errorException;
		    CustomMessage = WebUtility.UrlDecode(urlEncodedMessage);
		}
	}
}
