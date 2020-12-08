using Comments.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
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

			var sessions = new Dictionary<int, Guid>();

			if (request.Cookies != null && request.Cookies.Count > 0)
			{
				var cookies = request.Cookies.Where(cookie => cookie.Key.StartsWith(Constants.SessionCookieName)).ToList();

				if (cookies.Any())
				{
					foreach (var (cookieKey, cookieValue) in cookies)
					{
						if (int.TryParse(cookieKey.Substring(Constants.SessionCookieName.Length), out var consultationId) && Guid.TryParse(cookieValue, out var sessionId))
						{
							sessions.Add(consultationId, sessionId);
						}
					}
				}
			}
			bindingContext.Result = ModelBindingResult.Success(new Session(sessions));

			return Task.CompletedTask;
		}
	}
}
