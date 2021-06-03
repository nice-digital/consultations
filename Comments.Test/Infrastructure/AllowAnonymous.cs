using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Comments.Test.Infrastructure
{
	public class AllowAnonymous : IAuthorizationHandler
	{
		public Task HandleAsync(AuthorizationHandlerContext context)
		{
			foreach (IAuthorizationRequirement requirement in context.PendingRequirements.ToList())
				context.Succeed(requirement); //Simply pass all requirements

			return Task.CompletedTask;
		}
	}
}
