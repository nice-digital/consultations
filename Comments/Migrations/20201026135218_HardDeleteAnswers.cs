using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class HardDeleteAnswers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql($@"
				DELETE
				FROM Answer
				WHERE [IsDeleted] = 1
			");

			migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Answer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Answer",
                nullable: false,
                defaultValue: false);
        }
    }
}
