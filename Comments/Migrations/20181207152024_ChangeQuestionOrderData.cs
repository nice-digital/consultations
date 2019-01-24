using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class ChangeQuestionOrderData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"
				DECLARE
				  @Uri varchar(MAX),

				  @LocationID varchar(MAX),
				  @SourceURI varchar(MAX),
				  @Order varchar(MAX),
				  @QuestionID int,
				  @LastModifiedDate datetime2,

				  @ConsultationAndDocument varchar(MAX),
				  @ConsultationIdExists varchar(MAX),
				  @ConsultationId varchar(MAX),
				  @DocumentIdExists varchar(MAX),
				  @DocumentId varchar(MAX),

				  @QuestionOrder int = 0,
				  @QuestionOrderData varchar(MAX);

				DECLARE uri_cursor CURSOR FOR
				  SELECT DISTINCT l.SourceURI AS Uri
				  FROM Location AS l

				OPEN uri_cursor

				FETCH NEXT FROM uri_cursor
				INTO @Uri

				WHILE @@FETCH_STATUS = 0
				  BEGIN

					DECLARE order_cursor CURSOR FOR
					  SELECT l.LocationID, l.SourceURI, l.[Order], q.QuestionID, q.LastModifiedDate
					  FROM Location AS l JOIN Question AS q ON (l.LocationID = q.LocationID)
					  WHERE l.SourceURI = @Uri
					  ORDER BY q.LastModifiedDate

					OPEN order_cursor
					FETCH NEXT FROM order_cursor
					INTO @LocationID, @SourceURI, @Order, @QuestionID, @LastModifiedDate

					SET @QuestionOrder = 0;

					WHILE @@FETCH_STATUS = 0
					  BEGIN

						SET @ConsultationAndDocument = REPLACE(REPLACE(@SourceURI, 'consultations://./consultation/', ''), '/document/', '.')

						SET @ConsultationIdExists = PARSENAME(@ConsultationAndDocument, 2)
						SET @DocumentIdExists = PARSENAME(@ConsultationAndDocument, 1)

						IF @ConsultationIdExists IS NULL
						  BEGIN
							SET @ConsultationId = @DocumentIdExists;
							SET @DocumentId = '000';
						  END
						ELSE
						  BEGIN
							SET @ConsultationId = @ConsultationIdExists;
							SET @DocumentId = @DocumentIdExists;
						  END

						SET @QuestionOrderData = RIGHT('000'+ CAST (@ConsultationId AS varchar), 3) + '.' +
												 RIGHT('000'+ CAST (@DocumentId AS varchar), 3) + '.' +
												 RIGHT('000'+ CAST (@QuestionOrder AS varchar), 3);

						UPDATE dbo.Location
						  SET [Order] = @QuestionOrderData
						  WHERE LocationID = @LocationID

						SET @QuestionOrder = @QuestionOrder + 1;
						FETCH NEXT FROM order_cursor
						INTO @LocationID, @SourceURI, @Order, @QuestionID, @LastModifiedDate
					  END

					CLOSE order_cursor
					DEALLOCATE order_cursor

					FETCH NEXT FROM uri_cursor
					INTO @Uri
				  END

				CLOSE uri_cursor;
				DEALLOCATE uri_cursor;
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
