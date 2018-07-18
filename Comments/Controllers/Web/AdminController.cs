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

	    public AdminController(ISubmitService submitService, IQuestionService questionService)
	    {
		    _submitService = submitService;
		    _questionService = questionService;
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
