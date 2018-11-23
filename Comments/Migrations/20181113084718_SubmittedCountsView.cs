using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class SubmittedCountsView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql($@"
				CREATE VIEW {MigrationConstants.Views.SubmittedCommentAndAnswerCount} AS 

				SELECT	SUBSTRING(l.SourceURI, 0, CASE WHEN (PATINDEX('%document%', l.SourceURI) = 0) THEN LEN(l.SourceURI) + 1 ELSE PATINDEX('%document%', l.SourceURI) - 1 END) AS SourceURI, 
						COUNT(DISTINCT(sc.SubmissionID)) AS CommentCount,
						COUNT(DISTINCT(sa.SubmissionID)) AS AnswerCount,
						COUNT(DISTINCT(sc.SubmissionID)) + COUNT(DISTINCT(sa.SubmissionID)) AS TotalCount

				FROM [Location] l

				LEFT JOIN Comment c ON l.LocationID = c.LocationID AND c.IsDeleted = CAST(0 AS bit)
				LEFT JOIN SubmissionComment sc ON c.CommentID = sc.CommentID

				LEFT JOIN Question q ON l.LocationID = q.LocationID AND q.IsDeleted = CAST(0 AS bit)
				LEFT JOIN Answer a ON q.QuestionID = a.QuestionID AND a.IsDeleted = CAST(0 AS bit)
				LEFT JOIN SubmissionAnswer sa ON a.AnswerID = sa.AnswerID

				GROUP BY SUBSTRING(l.SourceURI, 0, CASE WHEN (PATINDEX('%document%', l.SourceURI) = 0) THEN LEN(l.SourceURI) + 1 ELSE PATINDEX('%document%', l.SourceURI) - 1 END)
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql($"DROP VIEW {MigrationConstants.Views.SubmittedCommentAndAnswerCount}");
		}
    }
}
