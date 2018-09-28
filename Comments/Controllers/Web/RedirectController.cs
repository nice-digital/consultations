using Comments.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using Comments.Common;

namespace Comments.Controllers.Web
{
	public class RedirectController : Controller
    {
	    private readonly IConsultationService _consultationService;

	    public RedirectController(IConsultationService consultationService)
	    {
		    _consultationService = consultationService;
	    }

	    public IActionResult PublishedDocumentWithoutChapter(int consultationId, int documentId)
        {
			if (consultationId <= 0)
				throw new ArgumentException(nameof(consultationId));

	        if (documentId <= 0)
		        throw new ArgumentException(nameof(documentId));

	        var chapterSlug = _consultationService.GetFirstChapterSlug(consultationId, documentId);
	        if (string.IsNullOrWhiteSpace(chapterSlug))
		        throw new Exception($"No chapter found for consultation: {consultationId} with doc id: {documentId}");

			var redirectUrl = string.Format(Constants.ConsultationsReplaceableRelativeUrl, consultationId, documentId, chapterSlug);

	        return Redirect(redirectUrl);
        }

	    public IActionResult PreviewDocumentWithoutChapter(string reference, int consultationId, int documentId)
	    {
			if (string.IsNullOrWhiteSpace(reference))
				throw new ArgumentNullException(nameof(reference));

		    if (consultationId <= 0)
			    throw new ArgumentException(nameof(consultationId));

		    if (documentId <= 0)
			    throw new ArgumentException(nameof(documentId));

		    var chapterSlug = _consultationService.GetFirstChapterSlugFromPreviewDocument(reference, consultationId, documentId);
		    if (string.IsNullOrWhiteSpace(chapterSlug))
			    throw new Exception($"No chapter found for reference: {reference} consultation: {consultationId} with doc id: {documentId}");

		    var redirectUrl = string.Format(Constants.ConsultationsPreviewReplaceableRelativeUrl, reference, consultationId, documentId, chapterSlug);

		    return Redirect(redirectUrl);
	    }
	}
}
