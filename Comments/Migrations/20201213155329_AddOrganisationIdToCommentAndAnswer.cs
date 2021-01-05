using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class AddOrganisationIdToCommentAndAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganisationID",
                table: "Comment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganisationID",
                table: "Answer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganisationID",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "OrganisationID",
                table: "Answer");
        }
    }
}
