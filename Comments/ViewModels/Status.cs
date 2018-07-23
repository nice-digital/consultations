using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.ViewModels
{
	public enum StatusName
	{
		Draft = 1,
		Submitted
	}

	public class Status
    {
		public Status() {}//only here for model binding. don't use it in code.

	    public Status(Models.Status status)
	    {
		    StatusId = status.StatusId;
		    Name = status.Name;
	    }

		public int StatusId { get; set; }
	    public string Name { get; set; }
    }
}
