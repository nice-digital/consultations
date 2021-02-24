using Comments.Common;
using Comments.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Comments.Controllers.Web
{
	public class RedirectController : Controller
    {
	    private readonly IConsultationService _consultationService;

	    public RedirectController(IConsultationService consultationService)
	    {
		    _consultationService = consultationService;
	    }

	    public async Task<IActionResult> PublishedRedirectWithoutDocument(int consultationId)
	    {
		    if (consultationId <= 0)
			    throw new ArgumentException(nameof(consultationId));

		    var documentIdAndChapterSlug = await _consultationService.GetFirstConvertedDocumentAndChapterSlug(consultationId);

		    if (!documentIdAndChapterSlug.documentId.HasValue)
			    throw new Exception($"No converted document found for consultation: {consultationId}");

			if (string.IsNullOrWhiteSpace(documentIdAndChapterSlug.chapterSlug))
			    throw new Exception($"No chapter found for consultation: {consultationId} with doc id: {documentIdAndChapterSlug.documentId}");

			var redirectUrl = string.Format(Constants.ConsultationsReplaceableRelativeUrl, consultationId, documentIdAndChapterSlug.documentId, documentIdAndChapterSlug.chapterSlug);

		    return Redirect(redirectUrl);
	    }

		public async Task<IActionResult> PublishedDocumentWithoutChapter(int consultationId, int documentId)
        {
			if (consultationId <= 0)
				throw new ArgumentException(nameof(consultationId));

	        if (documentId <= 0)
		        throw new ArgumentException(nameof(documentId));

	        var chapterSlug = await _consultationService.GetFirstChapterSlug(consultationId, documentId);
	        if (string.IsNullOrWhiteSpace(chapterSlug))
		        throw new Exception($"No chapter found for consultation: {consultationId} with doc id: {documentId}");

			var redirectUrl = string.Format(Constants.ConsultationsReplaceableRelativeUrl, consultationId, documentId, chapterSlug);

	        return Redirect(redirectUrl);
        }

	    public async Task<IActionResult> PreviewDocumentWithoutChapter(string reference, int consultationId, int documentId)
	    {
			if (string.IsNullOrWhiteSpace(reference))
				throw new ArgumentNullException(nameof(reference));

		    if (consultationId <= 0)
			    throw new ArgumentException(nameof(consultationId));

		    if (documentId <= 0)
			    throw new ArgumentException(nameof(documentId));

		    var chapterSlug = await _consultationService.GetFirstChapterSlugFromPreviewDocument(reference, consultationId, documentId);
		    if (string.IsNullOrWhiteSpace(chapterSlug))
			    throw new Exception($"No chapter found for reference: {reference} consultation: {consultationId} with doc id: {documentId}");

		    var redirectUrl = string.Format(Constants.ConsultationsPreviewReplaceableRelativeUrl, reference, consultationId, documentId, chapterSlug);

		    return Redirect(redirectUrl);
	    }
	}
}
