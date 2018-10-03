namespace Comments.Models
{
	/// <summary>
	/// This isn't to do with the Status table. This is for the status (health check) page.
	/// </summary>
	public class StatusModel
	{
		public StatusModel(int totalComments, int totalAnswers, int totalSubmissions, long databaseQueryTimeInMilliSeconds)
		{
			TotalComments = totalComments;
			TotalAnswers = totalAnswers;
			TotalSubmissions = totalSubmissions;
			DatabaseQueryTimeInMilliSeconds = databaseQueryTimeInMilliSeconds;
		}

		public int TotalComments { get; private set; }

		public int TotalAnswers { get; private set; }

		public int TotalSubmissions { get; private set; }

		public long DatabaseQueryTimeInMilliSeconds { get; private set; }
	}
}
