using System.Collections.Generic;
using Comments.Models;
using Comments.Services;
using Microsoft.EntityFrameworkCore;

namespace Comments.Test.Infrastructure
{
	public class ConsultationListContext : ConsultationsContext
	{
		private readonly IList<SubmittedCommentsAndAnswerCount> _commentsAndAnswerCounts;
		private readonly IList<SubmittedCommentsAndAnswerCount> _toLeadCommentsAndAnswerCount;

		public ConsultationListContext(DbContextOptions options, IUserService userService, IEncryption encryption) : base(options, userService, encryption)
		{}

		public ConsultationListContext(DbContextOptions options, IUserService userService, IEncryption encryption, IList<SubmittedCommentsAndAnswerCount> commentsAndAnswerCounts, IList<SubmittedCommentsAndAnswerCount> toLeadCommentsAndAnswerCount) : base(options, userService, encryption)
		{
			_commentsAndAnswerCounts = commentsAndAnswerCounts;
			_toLeadCommentsAndAnswerCount = toLeadCommentsAndAnswerCount;
		}

		public override IList<SubmittedCommentsAndAnswerCount> GetSubmittedCommentsAndAnswerCounts(bool SubmittedToLead = false)
		{
			if (SubmittedToLead)
				return _toLeadCommentsAndAnswerCount ?? new List<SubmittedCommentsAndAnswerCount>();
			else
				return _commentsAndAnswerCounts ?? new List<SubmittedCommentsAndAnswerCount>();
		}
	}
}
