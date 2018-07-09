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

	    public AdminController(ISubmitService submitService)
	    {
		    _submitService = submitService;
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
	}
}
