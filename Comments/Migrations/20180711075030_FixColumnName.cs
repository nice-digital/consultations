using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class FixColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubmissionCommentID",
                table: "SubmissionAnswer",
                newName: "SubmissionAnswerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubmissionAnswerID",
                table: "SubmissionAnswer",
                newName: "SubmissionCommentID");
        }
    }
}
