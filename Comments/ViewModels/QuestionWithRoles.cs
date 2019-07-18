using System.Collections.Generic;
using System.Linq;
using Comments.Configuration;

namespace Comments.ViewModels
{
	public class QuestionWithRoles : Question
	{
		public QuestionWithRoles(Models.Location location, Models.Question question, IEnumerable<string> createdByRoles, IEnumerable<string> lastModifiedByRoles) : base(location, question)
		{
			CreatedByRoles = FilterRoles(createdByRoles);
			LastModifiedByRoles = FilterRoles(lastModifiedByRoles);
		}

		public IEnumerable<string> CreatedByRoles { get; private set; }
		public IEnumerable<string> LastModifiedByRoles { get; private set; }

		public IEnumerable<string> AllRoles => CreatedByRoles.Concat(LastModifiedByRoles).Distinct();

		private static IEnumerable<string> FilterRoles(IEnumerable<string> roles)
		{
			if (roles == null)
				return new List<string>();

			var roleList = roles.ToList();
			if (!roleList.Any())
				return new List<string>();

			return roleList.Where(role => AppSettings.ConsultationListConfig.DownloadRoles.AllRoles.Contains(role));
		}
	}
}
