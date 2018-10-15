using System;
using Comments.ViewModels;
using Microsoft.AspNetCore.Http;
using NICE.Auth.NetCore.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Comments.Services
{
	public interface ISecurityService
	{
		Validate IsAllowedAccess(ICollection<string> permittedRoles);
	}

    public class SecurityService : ISecurityService
    {
	    private readonly IUserService _userService;
	    private readonly IHttpContextAccessor _httpContextAccessor;

	    public SecurityService(IUserService userService, IHttpContextAccessor httpContextAccessor)
	    {
		    _userService = userService;
		    _httpContextAccessor = httpContextAccessor;
	    }

	    public Validate IsAllowedAccess(ICollection<string> permittedRoles)
	    {
		    if (!permittedRoles.Any())
		    {
			    throw new ArgumentException("There is expected to be at least one permitted role", nameof(permittedRoles));
		    }

		    var user = _userService.GetCurrentUser();
		    if (!user.IsAuthorised)
		    {
			    return new Validate(false, true, false, "User is not authorised");
		    }
		    var niceUser = _httpContextAccessor.HttpContext.User;
		    if (!niceUser.Identity.IsAuthenticated)
		    {
			    return new Validate(false, false, false, "Not authenticated");
		    }
		    var userRoles = niceUser.Roles();

		    if (!userRoles.Any(permittedRoles.Contains))
		    {
			    return new Validate(false, true, false, "NICE user is not permitted to download this file");
		    }
			return new Validate(valid: true);
		}
    }
}
