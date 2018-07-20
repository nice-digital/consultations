using System.Collections.Generic;

namespace Comments.ViewModels
{
	public class Breadcrumb
    {
	    public Breadcrumb(IList<BreadcrumbLink> links)
	    {
		    Links = links;
	    }

	    public IList<BreadcrumbLink> Links { get; private set; }
    }

	public class BreadcrumbLink
	{
		public BreadcrumbLink(string text, string url)
		{
			Text = text;
			Url = url;
		}

		public string Text { get; private set; }
		public string Url { get; private set; }
	}
}
