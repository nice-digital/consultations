using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Comments.Configuration;
using Comments.Migrations;
using Comments.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace Comments.Models
{
    public partial class ConsultationsContext : DbContext
    {
		private readonly IUserService _userService;
		public virtual DbSet<Answer> Answer { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<OrganisationAuthorisation> OrganisationAuthorisation { get; set; }
        public virtual DbSet<OrganisationUser> OrganisationUser { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<QuestionType> QuestionType { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<Submission> Submission { get; set; }
        public virtual DbSet<SubmissionAnswer> SubmissionAnswer { get; set; }
        public virtual DbSet<SubmissionComment> SubmissionComment { get; set; }

		/// <summary>
		/// Query type - see here for more info https://docs.microsoft.com/en-us/ef/core/modeling/query-types
		/// </summary>
		public virtual DbQuery<SubmittedCommentsAndAnswerCount> SubmittedCommentsAndAnswerCounts { get; set; }

		private string _createdByUserID;
		private IEnumerable<int> _organisationUserIDs;
//		private IList<int> _OrganisationalLeadOrganisationIDs;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasIndex(e => e.QuestionId);

                entity.HasIndex(e => e.StatusId);

                entity.Property(e => e.AnswerId).HasColumnName("AnswerID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

				entity.Property(e => e.AnswerText)
					.HasConversion(
						v => _encryption.EncryptString(v, Encoding.ASCII.GetBytes(AppSettings.EncryptionConfig.Key), Encoding.ASCII.GetBytes(AppSettings.EncryptionConfig.IV)),
						v => _encryption.DecryptString(v, Encoding.ASCII.GetBytes(AppSettings.EncryptionConfig.Key), Encoding.ASCII.GetBytes(AppSettings.EncryptionConfig.IV)));

				entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastModifiedByUserId).HasColumnName("LastModifiedByUserID");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OrganisationUserId).HasColumnName("OrganisationUserID");

                entity.Property(e => e.ParentAnswerId).HasColumnName("ParentAnswerID");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.Property(e => e.OrganisationId).HasColumnName("OrganisationID");

				entity.Property(e => e.StatusId)
                    .HasColumnName("StatusID")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.ParentAnswer)
                    .WithMany(p => p.ChildAnswers)
                    .HasForeignKey(d => d.ParentAnswerId)
                    .HasConstraintName("FK_Answer_Answer");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answer)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Answer_Question");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Answer)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Answer_Status");

				//JW. automatically filter out deleted rows and other people's comments. this filter can be ignored using IgnoreQueryFilters. There's a unit test for this.
				//note: only 1 filter is supported. you must combine the logic into one expression.
				entity.HasQueryFilter(c => c.CreatedByUserId == _createdByUserID);
			});

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasIndex(e => e.LocationId);

                entity.HasIndex(e => e.StatusId);

                entity.Property(e => e.CommentId).HasColumnName("CommentID");

				entity.Property(e => e.CommentText)
					.HasConversion(
						v => _encryption.EncryptString(v, Encoding.ASCII.GetBytes(AppSettings.EncryptionConfig.Key), Encoding.ASCII.GetBytes(AppSettings.EncryptionConfig.IV)),
						v => _encryption.DecryptString(v, Encoding.ASCII.GetBytes(AppSettings.EncryptionConfig.Key), Encoding.ASCII.GetBytes(AppSettings.EncryptionConfig.IV)))
					.IsRequired();

				entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastModifiedByUserId).HasColumnName("LastModifiedByUserID");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.OrganisationUserId).HasColumnName("OrganisationUserID");

                entity.Property(e => e.ParentCommentId).HasColumnName("ParentCommentID");

                entity.Property(e => e.OrganisationId).HasColumnName("OrganisationID");

				entity.Property(e => e.StatusId)
                    .HasColumnName("StatusID")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_Location");

                entity.HasOne(d => d.ParentComment)
                    .WithMany(p => p.ChildComments)
                    .HasForeignKey(d => d.ParentCommentId)
                    .HasConstraintName("FK_Comment_Comment");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_Status");

				//JW. automatically filter out other people's comments. this filter can be ignored using IgnoreQueryFilters. There's a unit test for this.
				//note: only 1 filter is supported. you must combine the logic into one expression.
				entity.HasQueryFilter(c => (c.CreatedByUserId != null && _createdByUserID != null && c.CreatedByUserId == _createdByUserID)
				                           || (_organisationUserIDs != null && c.OrganisationUserId.HasValue && _organisationUserIDs.Any(organisationUserID => organisationUserID.Equals(c.OrganisationUserId)))); 
				                         //  || (c.OrganisationId.HasValue && _OrganisationalLeadOrganisationIDs.Any(orgId => orgId.Equals(c.OrganisationId.Value))));
			});

            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.HtmlElementID).HasColumnName("HtmlElementID");

                entity.Property(e => e.SourceURI).HasColumnName("SourceURI");
            });

            modelBuilder.Entity<OrganisationAuthorisation>(entity =>
            {
				entity.Property(e => e.OrganisationAuthorisationId)
					.HasColumnName("OrganisationAuthorisationID");

                entity.Property(e => e.CreatedByUserId)
                    .IsRequired()
                    .HasColumnName("CreatedByUserID");

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.OrganisationId).HasColumnName("OrganisationID");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.OrganisationAuthorisation)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganisationAuthorisation_Location");
            });

            modelBuilder.Entity<OrganisationUser>(entity =>
            {
				entity.Property(e => e.OrganisationUserId)
					.HasColumnName("OrganisationUserID");

                entity.Property(e => e.EmailAddress).HasMaxLength(100);

                entity.Property(e => e.OrganisationAuthorisationId).HasColumnName("OrganisationAuthorisationID");

                entity.Property(e => e.CreatedDate).IsRequired().HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExpirationDate).IsRequired();

				entity.HasOne(d => d.OrganisationAuthorisation)
	                .WithMany(p => p.OrganisationUsers)
	                .HasForeignKey(d => d.OrganisationAuthorisationId)
	                .HasConstraintName("FK_OrganisationUser_OrganisationAuthorisation");

			});

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasIndex(e => e.LocationId);

                entity.HasIndex(e => e.QuestionTypeId);

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastModifiedByUserId).HasColumnName("LastModifiedByUserID");

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.QuestionText).IsRequired();

                entity.Property(e => e.QuestionTypeId).HasColumnName("QuestionTypeID");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Question)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question_Location");

                entity.HasOne(d => d.QuestionType)
                    .WithMany(p => p.Question)
                    .HasForeignKey(d => d.QuestionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question_QuestionType");

				entity.HasQueryFilter(e => !e.IsDeleted); //JW. automatically filter out deleted rows. this filter can be ignored using IgnoreQueryFilters. There's a unit test for this.
			});

            modelBuilder.Entity<QuestionType>(entity =>
            {
                entity.Property(e => e.QuestionTypeId).HasColumnName("QuestionTypeID");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100);
            });

			modelBuilder.Entity<QuestionType>().HasData(
				new { QuestionTypeId = 99, Description = "A yes / no answer without a text answer", HasTextAnswer = true, HasBooleanAnswer = true });

			modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(e => e.StatusId)
                    .HasColumnName("StatusID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

			modelBuilder.Entity<Status>().HasData(
				new {StatusId = 1, Name = "Draft"},
				new {StatusId = 2, Name = "Submitted"});

			modelBuilder.Entity<Submission>(entity =>
            {
                entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");

                entity.Property(e => e.SubmissionByUserId).HasColumnName("SubmissionByUserID");

                entity.Property(e => e.SubmissionDateTime).HasDefaultValueSql("(getdate())");
            });

			modelBuilder.Entity<SubmissionComment>(entity =>
			{
				entity.HasKey(e => e.SubmissionCommentId);

				entity.HasIndex(e => e.CommentId);

				entity.HasIndex(e => e.SubmissionId);

				entity.Property(e => e.SubmissionCommentId).HasColumnName("SubmissionCommentID");

				entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");

				entity.Property(e => e.CommentId).HasColumnName("CommentID");

				entity.HasOne(d => d.Submission)
					.WithMany(p => p.SubmissionComment)
					.HasForeignKey(d => d.SubmissionId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_SubmissionComment_SubmissionID");

				entity.HasOne(d => d.Comment)
					.WithMany(p => p.SubmissionComment)
					.HasForeignKey(d => d.CommentId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_SubmissionComment_CommentID");
			});

			modelBuilder.Entity<SubmissionAnswer>(entity =>
            {
				entity.HasKey(e => e.SubmissionAnswerId);

				entity.HasIndex(e => e.AnswerId);

                entity.HasIndex(e => e.SubmissionId);

                entity.Property(e => e.SubmissionAnswerId).HasColumnName("SubmissionAnswerID");

                entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");

				entity.Property(e => e.AnswerId).HasColumnName("AnswerID");

				entity.HasOne(d => d.Submission)
                    .WithMany(p => p.SubmissionAnswer)
                    .HasForeignKey(d => d.SubmissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubmissionAnswer_SubmissionID");

				entity.HasOne(d => d.Answer)
					.WithMany(p => p.SubmissionAnswer)
					.HasForeignKey(d => d.AnswerId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_SubmissionAnswer_AnswerID");
			});

			modelBuilder
				.Query<SubmittedCommentsAndAnswerCount>()
				.ToView(MigrationConstants.Views.SubmittedCommentAndAnswerCount);
		}
	}
}
