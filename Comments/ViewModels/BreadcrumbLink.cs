using System.Collections.Generic;

namespace Comments.ViewModels
{
	public class BreadcrumbLink
	{
		public BreadcrumbLink(string label, string url)
		{
			Label = label;
			Url = url;
		}

		public string Label { get; private set; }
		public string Url { get; private set; }
	}
}
