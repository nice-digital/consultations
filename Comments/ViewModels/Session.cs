using Comments.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comments.ViewModels
{
	[ModelBinder(BinderType = typeof(SessionModelBinder))]
	public class Session
	{
		public Session(Dictionary<int, Guid> sessionCookies)
		{
			SessionCookies = sessionCookies;
		}

		public Dictionary<int, Guid> SessionCookies { get; set; }
	}

	/// <summary>
	/// Custom model binder for the Session object above. Maps all the organisation session cookies, and consultation ids into a dictionary.
	/// </summary>
	public class SessionModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
				throw new ArgumentNullException(nameof(bindingContext));

			var request = bindingContext.HttpContext.Request;
			if (request == null)
				throw new ArgumentNullException(nameof(bindingContext.HttpContext.Request));

			var sessions = request.Cookies.GetSessionCookies();

			bindingContext.Result = ModelBindingResult.Success(new Session(sessions));

			return Task.CompletedTask;
		}
	}
}
