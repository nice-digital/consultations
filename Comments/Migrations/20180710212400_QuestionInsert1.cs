using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class QuestionInsert1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: MigrationConstants.Tables.QuestionType.TableName,
                columns: new[] {
	                MigrationConstants.Tables.QuestionType.Columns.Description,
					MigrationConstants.Tables.QuestionType.Columns.HasBooleanAnswer,
					MigrationConstants.Tables.QuestionType.Columns.HasTextAnswer
				},
                values: new object[]
                {
	                "A text question requiring a text area.",
					false,
					true
                });

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
