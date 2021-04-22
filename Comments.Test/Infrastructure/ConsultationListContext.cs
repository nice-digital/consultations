using System.Collections.Generic;
using Comments.Models;
using Comments.Services;
using Microsoft.EntityFrameworkCore;

namespace Comments.Test.Infrastructure
{
	public class ConsultationListContext : ConsultationsContext
	{
		private readonly IList<SubmittedCommentsAndAnswerCount> _commentsAndAnswerCounts;
		private readonly SubmittedToLeadCommentsAndAnswerCount _toLeadCommentsAndAnswerCount;

		public ConsultationListContext(DbContextOptions options, IUserService userService, IEncryption encryption) : base(options, userService, encryption)
		{}

		public ConsultationListContext(DbContextOptions options, IUserService userService, IEncryption encryption, IList<SubmittedCommentsAndAnswerCount> commentsAndAnswerCounts, SubmittedToLeadCommentsAndAnswerCount toLeadCommentsAndAnswerCount) : base(options, userService, encryption)
		{
			_commentsAndAnswerCounts = commentsAndAnswerCounts;
			_toLeadCommentsAndAnswerCount = toLeadCommentsAndAnswerCount;
		}

		public override IList<SubmittedCommentsAndAnswerCount> GetSubmittedCommentsAndAnswerCounts()
		{
			return _commentsAndAnswerCounts ?? new List<SubmittedCommentsAndAnswerCount>();
		}

		public override SubmittedToLeadCommentsAndAnswerCount GetSubmittedToLeadCommentsAndAnswerCounts(string sourceURI, int organisationId)
		{
			return _toLeadCommentsAndAnswerCount ?? new SubmittedToLeadCommentsAndAnswerCount();
		}
	}
}
