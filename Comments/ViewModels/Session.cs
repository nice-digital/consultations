using System;
using System.Collections.Generic;

namespace Comments.ViewModels
{
	public class Session
	{
		public Session(Dictionary<int, Guid> sessionCookies)
		{
			SessionCookies = sessionCookies;
		}

		public Dictionary<int, Guid> SessionCookies { get; set; }
	}
}
