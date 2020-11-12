using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Models
{
	public partial class OrganisationUser
	{
		public OrganisationUser() //Just for EF
		{
			Answer = new HashSet<Answer>();
			Comment = new HashSet<Comment>();
		}
	}
}
