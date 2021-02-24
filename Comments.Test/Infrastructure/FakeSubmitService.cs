using Comments.Models;
using Comments.Services;
using Comments.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Location = Comments.Models.Location;

namespace Comments.Test.Infrastructure
{
	public class FakeSubmitService : ISubmitService
    {
	    public async Task<(int rowsUpdated, Validate validate)> Submit(ViewModels.Submission submission)
	    {
		    throw new NotImplementedException();
	    }

	    public int DeleteAllSubmissionsFromUser(Guid usersSubmissionsToDelete)
	    {
		    throw new NotImplementedException();
	    }

	    public bool HasSubmittedCommentsOrQuestions(string consultationSourceURI, Guid userId)
	    {
		    throw new NotImplementedException();
	    }

	    public ConsultationState GetConsultationState(string sourceURI, IEnumerable<Location> locations = null)
	    {
		    return new ConsultationState(DateTime.MinValue, DateTime.MaxValue, true, true, true, DateTime.MaxValue, null);
	    }

		public async Task<(int rowsUpdated, Validate validate, ConsultationsContext context)> SubmitToLead(SubmissionToLead submission)
		{
			throw new NotImplementedException();
		}
	}
}
