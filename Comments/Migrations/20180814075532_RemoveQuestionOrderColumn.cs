using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class RemoveQuestionOrderColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropColumn(
				name: "QuestionOrder",
				table: "Question");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.AddColumn<byte>(
		        name: "QuestionOrder",
		        table: "Question",
		        nullable: true);
		}
    }
}
