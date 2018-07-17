using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class FixDefaults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.AlterColumn<bool>(
				name: MigrationConstants.Tables.Comment.Columns.IsDeleted,
				table: MigrationConstants.Tables.Comment.TableName,
				defaultValue: false);

	        migrationBuilder.AlterColumn<bool>(
				name: MigrationConstants.Tables.Question.Columns.IsDeleted,
				table: MigrationConstants.Tables.Question.TableName,
				defaultValue: false);

	        migrationBuilder.AlterColumn<bool>(
				name: MigrationConstants.Tables.Answer.Columns.IsDeleted,
				table: MigrationConstants.Tables.Answer.TableName,
				defaultValue: false);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
		}
    }
}
