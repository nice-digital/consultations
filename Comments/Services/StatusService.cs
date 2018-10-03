using Comments.Models;
using System.Diagnostics;

namespace Comments.Services
{
	public interface IStatusService
	{
		StatusModel GetStatusModel();
	}

	public class StatusService : IStatusService
	{
		private readonly ConsultationsContext _context;
		public StatusService(ConsultationsContext context)
		{
			_context = context;
		}

		public StatusModel GetStatusModel()
		{
			var timerStopwatch = Stopwatch.StartNew();

			var statusData = _context.GetStatusData();

			timerStopwatch.Stop();
			return new StatusModel(statusData.totalComments, statusData.totalAnswers, statusData.totalSubmissions, timerStopwatch.ElapsedMilliseconds);
		}
	}
}
