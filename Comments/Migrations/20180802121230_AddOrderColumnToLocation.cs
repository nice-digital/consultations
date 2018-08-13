using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class AddOrderColumnToLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Order",
                table: "Location",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Location");
        }
    }
}
