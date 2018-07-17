//using Microsoft.EntityFrameworkCore.Migrations;

//namespace Comments.Migrations
//{
//    public partial class QuestionInsertOne : Migration
//    {
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//	        migrationBuilder.Sql(@"
//				DECLARE @questionTypeID AS int
//				DECLARE @locationID1 AS int, @locationID2 AS int, @locationID3 AS int
//				DECLARE @userID as uniqueidentifier

//				SELECT @userID = cast(cast(0 AS binary) AS uniqueidentifier)

//				--question type insert
//				INSERT INTO QuestionType (Description, HasBooleanAnswer, HasTextAnswer)
//				VALUES ('A text question requiring a text area.', 0, 1)

//				SET @questionTypeID = SCOPE_IDENTITY();

//				--3 location inserts
//				INSERT INTO Location (SourceURI)
//				VALUES ('consultations://./consultation/1')

//				SET @locationID1 = SCOPE_IDENTITY();

//				INSERT INTO Location (SourceURI)
//				VALUES ('consultations://./consultation/1/document/1')

//				SET @locationID2 = SCOPE_IDENTITY();

//				INSERT INTO Location (SourceURI)
//				VALUES ('consultations://./consultation/1/document/2')

//				SET @locationID3 = SCOPE_IDENTITY();

//				--now the question inserts

//				INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, QuestionOrder, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
//				VALUES (@locationID1, 'Which areas will have the biggest impact on practice and be challenging to implement? Please say for whom and why.', @questionTypeID, 1, @userID, @userID, GETDATE())


//				INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, QuestionOrder, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
//				VALUES (@locationID2, 'Would implementation of any of the draft recommendations have significant cost implications?', @questionTypeID, 2, @userID, @userID, GETDATE())


//				INSERT INTO Question (LocationID, QuestionText, QuestionTypeID, QuestionOrder, CreatedByUserID, LastModifiedByUserID, LastModifiedDate)
//				VALUES (@locationID3, 'Would implementation of any of the draft recommendations have cost implications?', @questionTypeID, 3, @userID, @userID, GETDATE())			
//			");
//		}

//        protected override void Down(MigrationBuilder migrationBuilder)
//        {

//        }
//    }
//}
