using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class PrefixUserIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql($@"
				DECLARE @prefix nvarchar(10)
				SET @prefix = 'auth0|' 

				UPDATE Answer
				SET CreatedByUserID = @prefix + LOWER(CreatedByUserID),
					LastModifiedByUserID = @prefix + LOWER(LastModifiedByUserID)

				UPDATE Comment
				SET CreatedByUserID = @prefix + LOWER(CreatedByUserID),
					LastModifiedByUserID = @prefix + LOWER(LastModifiedByUserID)

				UPDATE Question
				SET CreatedByUserID = @prefix + LOWER(CreatedByUserID),
					LastModifiedByUserID = @prefix + LOWER(LastModifiedByUserID)
			");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
