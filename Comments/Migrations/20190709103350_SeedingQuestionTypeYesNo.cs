using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class SeedingQuestionTypeYesNo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "QuestionType",
                columns: new[] { "QuestionTypeID", "Description", "HasBooleanAnswer", "HasTextAnswer" },
                values: new object[] { 99, "A yes / no answer without a text answer", true, true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "QuestionType",
                keyColumn: "QuestionTypeID",
                keyValue: 99);
        }
    }
}
