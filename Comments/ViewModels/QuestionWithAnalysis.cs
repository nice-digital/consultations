using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
	/// <summary>
	/// This view model is here to provide the answer with analysis on. only for use by the admin side, not the user side.
	/// </summary>
	public class QuestionWithAnalysis : Question
	{
		public QuestionWithAnalysis(Models.Location location, Models.Question question) : base(location, question)
		{
			if (!(question.Answer is null))
			{
				Answers = question.Answer.Select(answer => new AnswerWithAnalysis(answer)).ToList();

				Analysed = Answers.Any(answer => answer.Analysed);
			}
		}

		public new IList<ViewModels.AnswerWithAnalysis> Answers { get; private set; }
		public bool Analysed { get; private set; }
	}
}
