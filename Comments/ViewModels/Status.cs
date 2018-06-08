using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.ViewModels
{
	public static class StatusName
	{
		public const int Draft = 1;
		public const int Submitted = 2;
	}

	public class Status
    {
		//public Status() {}//only here for model binding. don't use it in code.

	    public Status(Models.Status status)
	    {
		    StatusId = status.StatusId;
		    Name = status.Name;
	    }

		public int StatusId { get; set; }
	    public string Name { get; set; }
    }
}
