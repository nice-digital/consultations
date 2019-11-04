using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class UpdateUserIdColumnsForIdAM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql($@"
				ALTER TABLE Answer
				ALTER COLUMN CreatedByUserID nvarchar(100)

				ALTER TABLE Answer
				ALTER COLUMN LastModifiedByUserID nvarchar(100)

				ALTER TABLE Comment
				ALTER COLUMN CreatedByUserID nvarchar(100)

				ALTER TABLE Comment
				ALTER COLUMN LastModifiedByUserID nvarchar(100)

				ALTER TABLE Question
				ALTER COLUMN CreatedByUserID nvarchar(100)

				ALTER TABLE Question
				ALTER COLUMN LastModifiedByUserID nvarchar(100)
			");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql($@"
				ALTER TABLE Answer
				ALTER COLUMN CreatedByUserID uniqueidentifier

				ALTER TABLE Answer
				ALTER COLUMN LastModifiedByUserID uniqueidentifier

				ALTER TABLE Comment
				ALTER COLUMN CreatedByUserID uniqueidentifier

				ALTER TABLE Comment
				ALTER COLUMN LastModifiedByUserID uniqueidentifier

				ALTER TABLE Question
				ALTER COLUMN CreatedByUserID uniqueidentifier

				ALTER TABLE Question
				ALTER COLUMN LastModifiedByUserID uniqueidentifier
			");
		}
    }
}
