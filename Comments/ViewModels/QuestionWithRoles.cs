using System.Collections.Generic;
using System.Linq;
using Comments.Common;
using Comments.Configuration;

namespace Comments.ViewModels
{
	public class QuestionWithRoles : Question
	{
		public QuestionWithRoles(Models.Location location, Models.Question question, IEnumerable<string> createdByRoles, IEnumerable<string> lastModifiedByRoles) : base(location, question)
		{
			CreatedByRoles = createdByRoles.FilterRoles();
			LastModifiedByRoles = lastModifiedByRoles.FilterRoles();
		}

		public IEnumerable<string> CreatedByRoles { get; private set; }
		public IEnumerable<string> LastModifiedByRoles { get; private set; }

		public IEnumerable<string> AllRoles => CreatedByRoles.Concat(LastModifiedByRoles).Distinct();
	}
}
