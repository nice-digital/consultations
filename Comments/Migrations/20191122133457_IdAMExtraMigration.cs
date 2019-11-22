using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class IdAMExtraMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql($@"
				DECLARE @prefix nvarchar(10)
				SET @prefix = 'auth0|' 

				UPDATE Submission
				SET SubmissionByUserID = @prefix + LOWER(SubmissionByUserID)
			");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
