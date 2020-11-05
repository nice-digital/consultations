using System;
using System.Collections.Generic;

namespace Comments.Models
{
	public partial class OrganisationAuthorisation
	{
		private OrganisationAuthorisation() //Just for EF
		{
			Answer = new HashSet<Answer>();
			Comment = new HashSet<Comment>();
		}
	}
}
