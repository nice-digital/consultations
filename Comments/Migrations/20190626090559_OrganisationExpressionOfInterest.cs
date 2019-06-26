using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class OrganisationExpressionOfInterest : Migration
    {
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "OrganisationExpressionOfInterest",
				table: "Submission",
				nullable: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "OrganisationExpressionOfInterest",
				table: "Submission");

		}
	}
}
