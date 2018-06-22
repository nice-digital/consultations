using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class SubmissionTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                table: "Comment",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                table: "Answer",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    StatusID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.StatusID);
                });

            migrationBuilder.CreateTable(
                name: "Submission",
                columns: table => new
                {
                    SubmissionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubmissionDateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "(getdate())"),
                    SubmissionByUserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submission", x => x.SubmissionID);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionAnswer",
                columns: table => new
                {
                    SubmissionCommentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubmissionID = table.Column<int>(nullable: false),
                    AnswerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionAnswer", x => x.SubmissionCommentID);
                    table.ForeignKey(
                        name: "FK_SubmissionAnswer_AnswerID",
                        column: x => x.AnswerID,
                        principalTable: "Answer",
                        principalColumn: "AnswerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubmissionAnswer_SubmissionID",
                        column: x => x.SubmissionID,
                        principalTable: "Submission",
                        principalColumn: "SubmissionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionComment",
                columns: table => new
                {
                    SubmissionCommentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubmissionID = table.Column<int>(nullable: false),
                    CommentID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionComment", x => x.SubmissionCommentID);
                    table.ForeignKey(
                        name: "FK_SubmissionComment_CommentID",
                        column: x => x.CommentID,
                        principalTable: "Comment",
                        principalColumn: "CommentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubmissionComment_SubmissionID",
                        column: x => x.SubmissionID,
                        principalTable: "Submission",
                        principalColumn: "SubmissionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "StatusID", "Name" },
                values: new object[] { 1, "Draft" });

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "StatusID", "Name" },
                values: new object[] { 2, "Submitted" });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_StatusID",
                table: "Comment",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_StatusID",
                table: "Answer",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionAnswer_AnswerID",
                table: "SubmissionAnswer",
                column: "AnswerID");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionAnswer_SubmissionID",
                table: "SubmissionAnswer",
                column: "SubmissionID");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionComment_CommentID",
                table: "SubmissionComment",
                column: "CommentID");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionComment_SubmissionID",
                table: "SubmissionComment",
                column: "SubmissionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_Status",
                table: "Answer",
                column: "StatusID",
                principalTable: "Status",
                principalColumn: "StatusID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Status",
                table: "Comment",
                column: "StatusID",
                principalTable: "Status",
                principalColumn: "StatusID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_Status",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Status",
                table: "Comment");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "SubmissionAnswer");

            migrationBuilder.DropTable(
                name: "SubmissionComment");

            migrationBuilder.DropTable(
                name: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Comment_StatusID",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Answer_StatusID",
                table: "Answer");

            migrationBuilder.DropColumn(
                name: "StatusID",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "StatusID",
                table: "Answer");
        }
    }
}
