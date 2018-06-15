using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Comments.Migrations
{
    public partial class AddSumbissionTables : Migration
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
                    SubmissionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubmissionByUserId = table.Column<Guid>(nullable: false),
                    SubmissionDateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submission", x => x.SubmissionId);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionAnswer",
                columns: table => new
                {
                    SubmissionCommentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnswerId = table.Column<int>(nullable: false),
                    SubmissionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionAnswer", x => x.SubmissionCommentId);
                    table.ForeignKey(
                        name: "FK_SubmissionAnswer_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answer",
                        principalColumn: "AnswerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubmissionAnswer_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submission",
                        principalColumn: "SubmissionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionComment",
                columns: table => new
                {
                    SubmissionCommentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CommentId = table.Column<int>(nullable: false),
                    SubmissionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionComment", x => x.SubmissionCommentId);
                    table.ForeignKey(
                        name: "FK_SubmissionComment_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comment",
                        principalColumn: "CommentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubmissionComment_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submission",
                        principalColumn: "SubmissionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_StatusID",
                table: "Comment",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_StatusID",
                table: "Answer",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionAnswer_AnswerId",
                table: "SubmissionAnswer",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionAnswer_SubmissionId",
                table: "SubmissionAnswer",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionComment_CommentId",
                table: "SubmissionComment",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionComment_SubmissionId",
                table: "SubmissionComment",
                column: "SubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_Status",
                table: "Answer",
                column: "StatusID",
                principalTable: "Status",
                principalColumn: "StatusID",
                onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Comment_Status",
            //    table: "Comment",
            //    column: "StatusID",
            //    principalTable: "Status",
            //    principalColumn: "StatusID",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_Status",
                table: "Answer");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Comment_Status",
            //    table: "Comment");

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
