using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class AddSubmissionResponseColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.AddColumn<string>(
				name: "OrganisationName",
				table: "Submission",
				nullable: true);

	        migrationBuilder.AddColumn<string>(
		        name: "TobaccoDisclosure",
		        table: "Submission",
		        nullable: true);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropColumn(
				name: "OrganisationName",
				table: "Submission");

	        migrationBuilder.DropColumn(
		        name: "TobaccoDisclosure",
		        table: "Submission");
		}
    }
}
