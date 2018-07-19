using Comments.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Comments.Controllers.Web
{
	[Authorize(Roles="Administrator")]
	
	public class AdminController : Controller
    {
	    private readonly ISubmitService _submitService;
	    private readonly IQuestionService _questionService;
	    private readonly IUserService _userService;

	    public AdminController(ISubmitService submitService, IQuestionService questionService, IUserService userService)
	    {
		    _submitService = submitService;
		    _questionService = questionService;
		    _userService = userService;
	    }

		/// <summary>
		/// /consultations/admin/DeleteAllSubmissionsFromUser?userId={some guid}
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[Route("consultations/admin/DeleteAllSubmissionsFromUser")]
		public ActionResult DeleteAllSubmissionsFromUser(string userId)
	    {
		    if (string.IsNullOrWhiteSpace(userId))
			    throw new ArgumentNullException(nameof(userId));

		    if (!Guid.TryParse(userId, out Guid parsedGuid))
			    throw new ArgumentException("Cannot parse guid", nameof(userId));


		    var rowCount = _submitService.DeleteAllSubmissionsFromUser(parsedGuid);

		    return Content($"Row count deleted/updated: {rowCount}");
	    }

		/// <summary>
		/// /consultations/admin/DeleteAllSubmissionsFromSelf
		/// </summary>
		/// <returns></returns>
		[Route("consultations/admin/DeleteAllSubmissionsFromSelf")]
	    public ActionResult DeleteAllSubmissionsFromSelf()
		{
			var user = _userService.GetCurrentUser();
			if (!user.IsAuthorised || !user.UserId.HasValue)
				throw new Exception("Cannot get logged on user id");

			var rowCount = _submitService.DeleteAllSubmissionsFromUser(user.UserId.Value);

		    return Content($"Row count deleted/updated: {rowCount}");
	    }

		/// <summary>
		/// /consultations/admin/InsertQuestionsForDocument1And2InConsultation?consultationId=1
		/// </summary>
		/// <param name="consultationId"></param>
		/// <returns></returns>
		[Route("consultations/admin/InsertQuestionsForDocument1And2InConsultation")]
	    public ActionResult InsertQuestionsForDocument1And2InThisConsultation(int consultationId)
	    {
		    if (consultationId < 1)
			    throw new ArgumentException("invalid consultation id", nameof(consultationId));

		    var rowCount = _questionService.InsertQuestionsForAdmin(consultationId);

			return Content($"Row count deleted/updated: {rowCount}");
	    }
	}
}
