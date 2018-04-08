using System;
using System.Linq;
using Comments.Services;
using Microsoft.EntityFrameworkCore;

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

        private Guid? _currentUserId;
        //protected ConsultationsContext(IUserService userService)
        //{
        //    _userService = userService;
        //    _currentUserId = _userService.GetCurrentUser().UserId;
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _currentUserId = _userService.GetCurrentUser().UserId;

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.Property(e => e.AnswerId).HasColumnName("AnswerID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastModifiedByUserId).HasColumnName("LastModifiedByUserID");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answer)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Answer_Question");

                modelBuilder.Entity<Answer>().Property<Guid>("CreatedByUserId").HasField("_currentUserId");

                modelBuilder.Entity<Answer>().HasQueryFilter(b => EF.Property<Guid>(b, "CreatedByUserID") == _currentUserId);

                entity.HasQueryFilter(e => !e.IsDeleted); //JW. automatically filter out deleted rows. this filter can be ignored using IgnoreQueryFilters. There's a unit test for this.
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CommentId).HasColumnName("CommentID");

                entity.Property(e => e.CommentText).IsRequired();

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastModifiedByUserId).HasColumnName("LastModifiedByUserID");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_Location");

                modelBuilder.Entity<Comment>().Property<Guid>("CreatedByUserId").HasField("_currentUserId");

                modelBuilder.Entity<Comment>().HasQueryFilter(b => EF.Property<Guid>(b, "CreatedByUserID") == _currentUserId);

                entity.HasQueryFilter(e => !e.IsDeleted); //JW. automatically filter out deleted rows. this filter can be ignored using IgnoreQueryFilters. There's a unit test for this.
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.SourceURI).HasColumnName("SourceURI");

            });

            modelBuilder.Entity<Question>(entity =>
            {
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
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            foreach (var item in ChangeTracker.Entries().Where(
                e =>
                    e.State == EntityState.Added && e.Metadata.GetProperties().Any(p => p.Name == "CreatedByUserId")))
            {
                item.CurrentValues["CreatedByUserId"] = _currentUserId;
            }

            return base.SaveChanges();
        }
    }
}