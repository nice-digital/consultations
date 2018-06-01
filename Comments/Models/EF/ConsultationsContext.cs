using System;
using System.Linq;
using Comments.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Comments.Models
{
	public partial class ConsultationsContext : DbContext
	{
		private readonly IUserService _userService;
		public virtual DbSet<Answer> Answer { get; set; }
		public virtual DbSet<Comment> Comment { get; set; }
		public virtual DbSet<Location> Location { get; set; }
		public virtual DbSet<Question> Question { get; set; }
		public virtual DbSet<QuestionType> QuestionType { get; set; }
		public virtual DbSet<Status> Status { get; set; }

		private Guid? _createdByUserID;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Answer>(entity =>
			{
				entity.HasIndex(e => e.QuestionId);

				entity.HasIndex(e => e.StatusId);

				entity.Property(e => e.AnswerId).HasColumnName("AnswerID");

				entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

				entity.Property(e => e.CreatedDate).HasDefaultValueSql("date(\'now\')");

				entity.Property(e => e.LastModifiedByUserId).HasColumnName("LastModifiedByUserID");

				entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("date(\'now\')");

				entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

				entity.Property(e => e.StatusId)
					.HasColumnName("StatusID")
					.HasDefaultValueSql("((1))");

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
				entity.HasQueryFilter(c => !c.IsDeleted && c.CreatedByUserId == _createdByUserID);
			});

			modelBuilder.Entity<Comment>(entity =>
			{
				entity.HasIndex(e => e.LocationId);

				entity.HasIndex(e => e.StatusId);

				entity.Property(e => e.CommentId).HasColumnName("CommentID");

				entity.Property(e => e.CommentText).IsRequired();

				entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

				entity.Property(e => e.CreatedDate).HasDefaultValueSql("date(\'now\')");

				entity.Property(e => e.LastModifiedByUserId).HasColumnName("LastModifiedByUserID");

				entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("date(\'now\')");

				entity.Property(e => e.LocationId).HasColumnName("LocationID");

				entity.Property(e => e.StatusId)
					.HasColumnName("StatusID")
					.HasDefaultValueSql("((1))");

				entity.HasOne(d => d.Location)
					.WithMany(p => p.Comment)
					.HasForeignKey(d => d.LocationId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Comment_Location");

				entity.HasOne(d => d.Status)
					.WithMany(p => p.Comment)
					.HasForeignKey(d => d.StatusId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Comment_Status");

				//JW. automatically filter out deleted rows and other people's comments. this filter can be ignored using IgnoreQueryFilters. There's a unit test for this.
				//note: only 1 filter is supported. you must combine the logic into one expression.
				entity.HasQueryFilter(c => !c.IsDeleted && c.CreatedByUserId == _createdByUserID);
			});

			modelBuilder.Entity<Location>(entity =>
			{
				entity.Property(e => e.LocationId).HasColumnName("LocationID");

				entity.Property(e => e.HtmlElementID).HasColumnName("HtmlElementID"); 

				entity.Property(e => e.SourceURI).HasColumnName("SourceURI");

			});

			modelBuilder.Entity<Question>(entity =>
			{
				entity.HasIndex(e => e.LocationId);

				entity.HasIndex(e => e.QuestionTypeId);

				entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

				entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

				entity.Property(e => e.CreatedDate).HasDefaultValueSql("date(\'now\')");

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

			modelBuilder.Entity<Status>(entity =>
			{
				entity.Property(e => e.StatusId)
					.HasColumnName("StatusID")
					.ValueGeneratedNever();

				entity.Property(e => e.Name)
					.IsRequired()
					.HasMaxLength(100);
			});
		}
	}
}
