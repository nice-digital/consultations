using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Comments.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Consultations");

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    LocationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HtmlElementID = table.Column<string>(nullable: true),
                    Quote = table.Column<string>(nullable: true),
                    RangeEnd = table.Column<string>(nullable: true),
                    RangeEndOffset = table.Column<int>(nullable: true),
                    RangeStart = table.Column<string>(nullable: true),
                    RangeStartOffset = table.Column<int>(nullable: true),
                    SourceURI = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.LocationID);
                });

            migrationBuilder.CreateTable(
                name: "QuestionType",
                columns: table => new
                {
                    QuestionTypeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    HasBooleanAnswer = table.Column<bool>(nullable: false),
                    HasTextAnswer = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionType", x => x.QuestionTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    CommentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CommentText = table.Column<string>(nullable: false),
                    CreatedByUserID = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "(getdate())"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedByUserID = table.Column<Guid>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "(getdate())"),
                    LocationID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.CommentID);
                    table.ForeignKey(
                        name: "FK_Comment_Location",
                        column: x => x.LocationID,
                        principalTable: "Location",
                        principalColumn: "LocationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    QuestionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedByUserID = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "(getdate())"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedByUserID = table.Column<Guid>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: false),
                    LocationID = table.Column<int>(nullable: false),
                    QuestionOrder = table.Column<byte>(nullable: true),
                    QuestionText = table.Column<string>(nullable: false),
                    QuestionTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.QuestionID);
                    table.ForeignKey(
                        name: "FK_Question_Location",
                        column: x => x.LocationID,
                        principalTable: "Location",
                        principalColumn: "LocationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Question_QuestionType",
                        column: x => x.QuestionTypeID,
                        principalTable: "QuestionType",
                        principalColumn: "QuestionTypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Answer",
                columns: table => new
                {
                    AnswerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnswerBoolean = table.Column<bool>(nullable: true),
                    AnswerText = table.Column<string>(nullable: true),
                    CreatedByUserID = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "(getdate())"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedByUserID = table.Column<Guid>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "(getdate())"),
                    QuestionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answer", x => x.AnswerID);
                    table.ForeignKey(
                        name: "FK_Answer_Question",
                        column: x => x.QuestionID,
                        principalTable: "Question",
                        principalColumn: "QuestionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answer_QuestionID",
                table: "Answer",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_LocationID",
                table: "Comment",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Question_LocationID",
                table: "Question",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionTypeID",
                table: "Question",
                column: "QuestionTypeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answer");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "QuestionType");

            migrationBuilder.CreateTable(
                name: "Consultations",
                columns: table => new
                {
                    ConsultationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EndDate = table.Column<DateTime>(nullable: false),
                    GuidanceReference = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultations", x => x.ConsultationId);
                });
        }
    }
}
