using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Comments.Migrations
{
    public partial class AddSubmissionTables : Migration
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
                    SubmissionDateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "date('now')")
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
                    AnswerId = table.Column<int>(nullable: false, defaultValueSql: "AnswerId"),
                    SubmissionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionAnswer", x => x.SubmissionCommentId);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionComment",
                columns: table => new
                {
                    SubmissionCommentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CommentId = table.Column<int>(nullable: false, defaultValueSql: "CommentId"),
                    SubmissionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionComment", x => x.SubmissionCommentId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_StatusID",
                table: "Comment",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_StatusID",
                table: "Answer",
                column: "StatusID");

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
                name: "Submission");

            migrationBuilder.DropTable(
                name: "SubmissionAnswer");

            migrationBuilder.DropTable(
                name: "SubmissionComment");

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
