using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class AddReviewCheckboxColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RespondingAsOrganisation",
                table: "Submission",
                nullable: false,
	            defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasTobaccoLinks",
                table: "Submission",
                nullable: false,
	            defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RespondingAsOrganisation",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "HasTobaccoLinks",
                table: "Submission");
        }
    }
}
