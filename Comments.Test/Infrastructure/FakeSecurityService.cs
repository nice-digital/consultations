using Comments.Services;
using Comments.ViewModels;
using System.Collections.Generic;

namespace Comments.Test.Infrastructure
{
	public class FakeSecurityService : ISecurityService
    {
	    private readonly bool _valid;
	    private readonly bool _unauthorised;
	    private readonly bool _notFound;
	    private readonly string _message;

	    public FakeSecurityService(bool valid, bool unauthorised, bool notFound, string message)
	    {
		    _valid = valid;
		    _unauthorised = unauthorised;
		    _notFound = notFound;
		    _message = message;
	    }

	    public Validate IsAllowedAccess(ICollection<string> permittedRoles)
	    {
		    return new Validate(_valid, _unauthorised, _notFound, _message);
	    }
    }
}
