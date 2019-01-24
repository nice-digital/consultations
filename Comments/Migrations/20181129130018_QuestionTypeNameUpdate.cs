using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class QuestionTypeNameUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			//Don't copy this as an example of how to seed data. It should be done like this: https://csharp.christiannagel.com/2018/09/12/efcoreseeding/
			//this update script is only here since we've already added the data, and the description's wrong.
			migrationBuilder.Sql(@"
				UPDATE QuestionType
				SET [Description] = 'Text'
				WHERE HasTextAnswer = CAST(1 AS bit) AND HasBooleanAnswer = CAST(0 AS bit)
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
