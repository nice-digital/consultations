using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class SubmissionAllowNulls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "RespondingAsOrganisation",
                table: "Submission",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "HasTobaccoLinks",
                table: "Submission",
                nullable: true,
                oldClrType: typeof(bool));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "RespondingAsOrganisation",
                table: "Submission",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HasTobaccoLinks",
                table: "Submission",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
