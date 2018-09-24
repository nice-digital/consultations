using System.Collections.Generic;

namespace Comments.ViewModels
{
	public class BreadcrumbLink
	{
		public BreadcrumbLink(string label, string url, bool localRoute = false)
		{
			Label = label;
			Url = url;
			LocalRoute = localRoute;
		}

		public string Label { get; private set; }
		public string Url { get; private set; }
		public bool LocalRoute { get; private set; }
	}
}
