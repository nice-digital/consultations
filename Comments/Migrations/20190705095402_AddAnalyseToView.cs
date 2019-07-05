using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class AddAnalyseToView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql($@"
				ALTER VIEW {MigrationConstants.Views.SubmittedCommentAndAnswerCount} AS 

				SELECT  SUBSTRING(l.SourceURI, 0, CASE WHEN (PATINDEX('%document%', l.SourceURI) = 0) THEN LEN(l.SourceURI) + 1 ELSE PATINDEX('%document%', l.SourceURI) - 1 END) AS SourceURI, 
						COUNT(DISTINCT sc.SubmissionID) AS CommentCount, 
						COUNT(DISTINCT sa.SubmissionID) AS AnswerCount, 
						COUNT(DISTINCT s.SubmissionID) AS TotalCount,
						CASE WHEN ((COUNT(c.Sentiment) + COUNT(a.Sentiment)) > 0) THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS Analysed

				FROM	dbo.Location AS l 

				LEFT OUTER JOIN dbo.Comment AS c ON l.LocationID = c.LocationID AND c.IsDeleted = CAST(0 AS bit) 
				LEFT OUTER JOIN dbo.SubmissionComment AS sc ON c.CommentID = sc.CommentID 
				LEFT OUTER JOIN dbo.Question AS q ON l.LocationID = q.LocationID AND q.IsDeleted = CAST(0 AS bit) 
				LEFT OUTER JOIN dbo.Answer AS a ON q.QuestionID = a.QuestionID AND a.IsDeleted = CAST(0 AS bit) 
				LEFT OUTER JOIN dbo.SubmissionAnswer AS sa ON a.AnswerID = sa.AnswerID 
				LEFT OUTER JOIN dbo.Submission AS s ON s.SubmissionID = sc.SubmissionID OR s.SubmissionID = sa.SubmissionID

				GROUP BY	SUBSTRING(l.SourceURI, 0, CASE WHEN (PATINDEX('%document%', l.SourceURI) = 0) THEN LEN(l.SourceURI) + 1 ELSE PATINDEX('%document%', l.SourceURI) - 1 END)
			");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
