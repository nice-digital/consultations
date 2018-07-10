using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Comments.Migrations
{
    public static class MigrationConstants 
    {
	    public static class Tables
	    {
		    public static class Status
		    {
			    public const string TableName = "Status";

			    public static class Columns
			    {
				    public const string StatusID = "StatusID";
				    public const string Name = "Name";
			    }
		    }

		    public static class Comment
		    {
			    public const string TableName = "Comment";
				public static class Columns
			    {
				    
				    public const string StatusID = "StatusID";
			    }
		    }

		    public static class Answer
		    {
			    public const string TableName = "Answer";

			    public static class Columns
			    {
				    public const string StatusID = "StatusID";
			    }
		    }

		    public static class QuestionType
		    {
			    public const string TableName = "QuestionType";

				public static class Columns
				{
					public const string QuestionTypeID = "QuestionTypeID";
				    public const string Description = "Description";
				    public const string HasBooleanAnswer = "HasBooleanAnswer";
				    public const string HasTextAnswer = "HasTextAnswer";
				}
		    }

			public static class Question
		    {
			    public const string TableName = "Question";

			    public static class Columns
			    {
				    public const string QuestionID = "QuestionID";
				    public const string CreatedByUserID = "CreatedByUserID";
				    public const string CreatedDate = "CreatedDate";
				    public const string IsDeleted = "IsDeleted";
				    public const string LastModifiedByUserID = "LastModifiedByUserID";
				    public const string LastModifiedDate = "LastModifiedDate";
				    public const string LocationID = "LocationID";
				    public const string QuestionOrder = "QuestionOrder";
				    public const string QuestionText = "QuestionText";
				    public const string QuestionTypeID = "QuestionTypeID";
				}
		    }

			//add classes and constants for the other tables when needed.
	    }

		/// <summary>
		/// Annotations are here to support targetting different DB providers.
		/// see: https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/providers
		/// </summary>
		public static class Annotations
	    {
		    public static readonly KeyValuePair<string, object> Identity =
			    new KeyValuePair<string, object>("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
	    }
    }
}
