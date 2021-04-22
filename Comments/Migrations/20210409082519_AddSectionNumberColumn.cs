using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class AddSectionNumberColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SectionNumber",
                table: "Location",
                nullable: true);


            migrationBuilder.RenameColumn("Section", "Location", "SectionHeader");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectionNumber",
                table: "Location");

            migrationBuilder.RenameColumn("SectionHeader", "Location", "Section");
        }
    }
}
