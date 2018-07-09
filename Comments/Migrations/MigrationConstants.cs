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
			    public const string StatusID = "StatusID";
			    public const string Name = "Name";
			}

		    public static class Comment
		    {
			    public const string TableName = "Comment";
			    public const string StatusID = "StatusID";
		    }

		    public static class Answer
		    {
			    public const string TableName = "Answer";
			    public const string StatusID = "StatusID";
		    }

			//todo: add classes and constants for the other tables when needed.
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
