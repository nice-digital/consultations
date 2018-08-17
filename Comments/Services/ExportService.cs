using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Common;
using Comments.Models;
using Comments.ViewModels;
using Location = Comments.Models.Location;

namespace Comments.Services
{
	public interface IExportService
	{
		(IEnumerable<Location>, Validate) GetAllDataForConsulation(int consultationId);
		(string, IEnumerable<Document>, string) GetConsultationDetails(int consultationId);
	}

    public class ExportService : IExportService
    {
	    private readonly ConsultationsContext _context;
	    private readonly IUserService _userService;
	    private readonly User _currentUser;
	    private readonly IConsultationService _consultationService;

		public ExportService(ConsultationsContext consultationsContext, IUserService userService, IConsultationService consultationService)
	    {
			_context = consultationsContext;
		    _userService = userService;
		    _currentUser = _userService.GetCurrentUser();
		    _consultationService = consultationService;
		}

	    public (IEnumerable<Location>, Validate) GetAllDataForConsulation(int consultationId)
	    {
		    var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);
		    var dataInDB = _context.GetAllCommentsAndQuestionsForDocument(new[] {sourceURI}, true, true);

		    if (dataInDB == null)
			    return (null, new Validate(valid: false, notFound: true, message: $"Consultation id:{consultationId} not found trying to get answer for user id: {_currentUser.UserId} display name: {_currentUser.DisplayName}"));

			return (dataInDB, null);
	    }

	    public (string, IEnumerable<Document>, string) GetConsultationDetails(int consultationId)
	    {
		    var consultation = _consultationService.GetConsultation(consultationId, false);

		    var documents = _consultationService.GetDocuments(consultationId);
		    return (consultation.ConsultationName, documents, "Chapter");
	    }
    }
}
