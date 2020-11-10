using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class HardDeleteComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql($@"
				DELETE
				FROM Comment
				WHERE [IsDeleted] = 1
			");

			migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Comment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Comment",
                nullable: false,
                defaultValue: false);
        }
    }
}
