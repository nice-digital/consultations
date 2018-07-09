using System.Collections.Generic;
using System.Linq;

namespace Comments.Common
{
	public static class ModelConverters
    {
	    public static (IList<ViewModels.Comment> comments, IList<ViewModels.Question> questions) ConvertLocationsToCommentsAndQuestionsViewModels(IEnumerable<Models.Location> locations)
	    {
		    var commentsData = new List<ViewModels.Comment>();
		    var questionsData = new List<ViewModels.Question>();
		    foreach (var location in locations)
		    {
			    commentsData.AddRange(location.Comment.Select(comment => new ViewModels.Comment(location, comment)));
			    questionsData.AddRange(location.Question.Select(question => new ViewModels.Question(location, question)));
		    }
		    return (commentsData, questionsData);
	    }
	}
}
