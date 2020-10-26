﻿// <auto-generated />
using System;
using Comments.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Comments.Migrations
{
    [DbContext(typeof(ConsultationsContext))]
    partial class ConsultationsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Comments.Models.Answer", b =>
                {
                    b.Property<int>("AnswerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AnswerID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("AnswerBoolean");

                    b.Property<string>("AnswerText");

                    b.Property<string>("CreatedByUserId")
                        .HasColumnName("CreatedByUserID");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("LastModifiedByUserId")
                        .HasColumnName("LastModifiedByUserID");

                    b.Property<DateTime>("LastModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("QuestionId")
                        .HasColumnName("QuestionID");

                    b.Property<int>("StatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("StatusID")
                        .HasDefaultValueSql("((1))");

                    b.HasKey("AnswerId");

                    b.HasIndex("QuestionId");

                    b.HasIndex("StatusId");

                    b.ToTable("Answer");
                });

            modelBuilder.Entity("Comments.Models.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CommentID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CommentText")
                        .IsRequired();

                    b.Property<string>("CreatedByUserId")
                        .HasColumnName("CreatedByUserID");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("LastModifiedByUserId")
                        .HasColumnName("LastModifiedByUserID");

                    b.Property<DateTime>("LastModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("LocationId")
                        .HasColumnName("LocationID");

                    b.Property<int>("StatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("StatusID")
                        .HasDefaultValueSql("((1))");

                    b.HasKey("CommentId");

                    b.HasIndex("LocationId");

                    b.HasIndex("StatusId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("Comments.Models.Location", b =>
                {
                    b.Property<int>("LocationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("LocationID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("HtmlElementID")
                        .HasColumnName("HtmlElementID");

                    b.Property<string>("Order");

                    b.Property<string>("Quote");

                    b.Property<string>("RangeEnd");

                    b.Property<int?>("RangeEndOffset");

                    b.Property<string>("RangeStart");

                    b.Property<int?>("RangeStartOffset");

                    b.Property<string>("Section");

                    b.Property<string>("SourceURI")
                        .HasColumnName("SourceURI");

                    b.HasKey("LocationId");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("Comments.Models.Question", b =>
                {
                    b.Property<int>("QuestionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("QuestionID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedByUserId")
                        .HasColumnName("CreatedByUserID");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(getdate())");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("LastModifiedByUserId")
                        .HasColumnName("LastModifiedByUserID");

                    b.Property<DateTime>("LastModifiedDate");

                    b.Property<int>("LocationId")
                        .HasColumnName("LocationID");

                    b.Property<string>("QuestionText")
                        .IsRequired();

                    b.Property<int>("QuestionTypeId")
                        .HasColumnName("QuestionTypeID");

                    b.HasKey("QuestionId");

                    b.HasIndex("LocationId");

                    b.HasIndex("QuestionTypeId");

                    b.ToTable("Question");
                });

            modelBuilder.Entity("Comments.Models.QuestionType", b =>
                {
                    b.Property<int>("QuestionTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("QuestionTypeID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<bool>("HasBooleanAnswer");

                    b.Property<bool>("HasTextAnswer");

                    b.HasKey("QuestionTypeId");

                    b.ToTable("QuestionType");

                    b.HasData(
                        new { QuestionTypeId = 99, Description = "A yes / no answer without a text answer", HasBooleanAnswer = true, HasTextAnswer = true }
                    );
                });

            modelBuilder.Entity("Comments.Models.Status", b =>
                {
                    b.Property<int>("StatusId")
                        .HasColumnName("StatusID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("StatusId");

                    b.ToTable("Status");

                    b.HasData(
                        new { StatusId = 1, Name = "Draft" },
                        new { StatusId = 2, Name = "Submitted" }
                    );
                });

            modelBuilder.Entity("Comments.Models.Submission", b =>
                {
                    b.Property<int>("SubmissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SubmissionID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("HasTobaccoLinks");

                    b.Property<bool?>("OrganisationExpressionOfInterest");

                    b.Property<string>("OrganisationName");

                    b.Property<bool>("RespondingAsOrganisation");

                    b.Property<string>("SubmissionByUserId")
                        .HasColumnName("SubmissionByUserID");

                    b.Property<DateTime>("SubmissionDateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("TobaccoDisclosure");

                    b.HasKey("SubmissionId");

                    b.ToTable("Submission");
                });

            modelBuilder.Entity("Comments.Models.SubmissionAnswer", b =>
                {
                    b.Property<int>("SubmissionAnswerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SubmissionAnswerID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AnswerId")
                        .HasColumnName("AnswerID");

                    b.Property<int>("SubmissionId")
                        .HasColumnName("SubmissionID");

                    b.HasKey("SubmissionAnswerId");

                    b.HasIndex("AnswerId");

                    b.HasIndex("SubmissionId");

                    b.ToTable("SubmissionAnswer");
                });

            modelBuilder.Entity("Comments.Models.SubmissionComment", b =>
                {
                    b.Property<int>("SubmissionCommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SubmissionCommentID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CommentId")
                        .HasColumnName("CommentID");

                    b.Property<int>("SubmissionId")
                        .HasColumnName("SubmissionID");

                    b.HasKey("SubmissionCommentId");

                    b.HasIndex("CommentId");

                    b.HasIndex("SubmissionId");

                    b.ToTable("SubmissionComment");
                });

            modelBuilder.Entity("Comments.Models.Answer", b =>
                {
                    b.HasOne("Comments.Models.Question", "Question")
                        .WithMany("Answer")
                        .HasForeignKey("QuestionId")
                        .HasConstraintName("FK_Answer_Question");

                    b.HasOne("Comments.Models.Status", "Status")
                        .WithMany("Answer")
                        .HasForeignKey("StatusId")
                        .HasConstraintName("FK_Answer_Status");
                });

            modelBuilder.Entity("Comments.Models.Comment", b =>
                {
                    b.HasOne("Comments.Models.Location", "Location")
                        .WithMany("Comment")
                        .HasForeignKey("LocationId")
                        .HasConstraintName("FK_Comment_Location");

                    b.HasOne("Comments.Models.Status", "Status")
                        .WithMany("Comment")
                        .HasForeignKey("StatusId")
                        .HasConstraintName("FK_Comment_Status");
                });

            modelBuilder.Entity("Comments.Models.Question", b =>
                {
                    b.HasOne("Comments.Models.Location", "Location")
                        .WithMany("Question")
                        .HasForeignKey("LocationId")
                        .HasConstraintName("FK_Question_Location");

                    b.HasOne("Comments.Models.QuestionType", "QuestionType")
                        .WithMany("Question")
                        .HasForeignKey("QuestionTypeId")
                        .HasConstraintName("FK_Question_QuestionType");
                });

            modelBuilder.Entity("Comments.Models.SubmissionAnswer", b =>
                {
                    b.HasOne("Comments.Models.Answer", "Answer")
                        .WithMany("SubmissionAnswer")
                        .HasForeignKey("AnswerId")
                        .HasConstraintName("FK_SubmissionAnswer_AnswerID");

                    b.HasOne("Comments.Models.Submission", "Submission")
                        .WithMany("SubmissionAnswer")
                        .HasForeignKey("SubmissionId")
                        .HasConstraintName("FK_SubmissionAnswer_SubmissionID");
                });

            modelBuilder.Entity("Comments.Models.SubmissionComment", b =>
                {
                    b.HasOne("Comments.Models.Comment", "Comment")
                        .WithMany("SubmissionComment")
                        .HasForeignKey("CommentId")
                        .HasConstraintName("FK_SubmissionComment_CommentID");

                    b.HasOne("Comments.Models.Submission", "Submission")
                        .WithMany("SubmissionComment")
                        .HasForeignKey("SubmissionId")
                        .HasConstraintName("FK_SubmissionComment_SubmissionID");
                });
#pragma warning restore 612, 618
        }
    }
}
