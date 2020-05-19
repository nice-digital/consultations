using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace Comments.Test.Infrastructure
{
	public class FakeUrlHelper : IUrlHelper
	{
		public string _actionUrl { get; set; }
		public FakeUrlHelper(string actionUrl = null)
		{
			_actionUrl = actionUrl;
		}

		public string Action(UrlActionContext actionContext)
		{
			return _actionUrl ?? "/fake/route";
		}

		public string Content(string contentPath)
		{
			throw new NotImplementedException();
		}

		public bool IsLocalUrl(string url)
		{
			throw new NotImplementedException();
		}

		public string RouteUrl(UrlRouteContext routeContext)
		{
			throw new NotImplementedException();
		}

		public string Link(string routeName, object values)
		{
			throw new NotImplementedException();
		}

		public ActionContext ActionContext { get; }
	}
}
