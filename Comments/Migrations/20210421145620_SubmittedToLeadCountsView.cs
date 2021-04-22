using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class SubmittedToLeadCountsView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
				CREATE VIEW {MigrationConstants.Views.SubmittedToLeadCommentAndAnswerCount} AS 
				SELECT	SUBSTRING(l.SourceURI, 0, CASE WHEN (PATINDEX('%document%', l.SourceURI) = 0) THEN LEN(l.SourceURI) + 1 ELSE PATINDEX('%document%', l.SourceURI) - 1 END) AS SourceURI, 
                        COALESCE(c.OrganisationID, a.OrganisationID) AS OrganisationId,
						COUNT(DISTINCT(sc.SubmissionID)) AS CommentCount,
						COUNT(DISTINCT(sa.SubmissionID)) AS AnswerCount,
					    COUNT(DISTINCT(s.SubmissionID)) AS TotalCount
				FROM [Location] l
				LEFT JOIN Comment c ON l.LocationID = c.LocationID AND c.StatusID = 3
				LEFT JOIN SubmissionComment sc ON c.CommentID = sc.CommentID
				LEFT JOIN Question q ON l.LocationID = q.LocationID AND q.IsDeleted = CAST(0 AS bit)
				LEFT JOIN Answer a ON q.QuestionID = a.QuestionID AND a.StatusID = 3
				LEFT JOIN SubmissionAnswer sa ON a.AnswerID = sa.AnswerID
                LEFT JOIN Submission s ON (s.SubmissionID = sc.SubmissionID OR s.SubmissionID = sa.SubmissionID)
                WHERE s.RespondingAsOrganisation = 1
				GROUP BY SUBSTRING(l.SourceURI, 0, CASE WHEN (PATINDEX('%document%', l.SourceURI) = 0) THEN LEN(l.SourceURI) + 1 ELSE PATINDEX('%document%', l.SourceURI) - 1 END),
                        COALESCE(c.OrganisationID, a.OrganisationID)
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP VIEW {MigrationConstants.Views.SubmittedToLeadCommentAndAnswerCount}");
        }
    }
}
