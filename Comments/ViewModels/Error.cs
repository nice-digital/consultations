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
	    public Exception ErrorException { get; }

	    public Error(string requestedPath, Exception errorException)
	    {
		    RequestedPath = requestedPath;
		    ErrorException = errorException;
		}
	}
}
