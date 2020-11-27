using System.Text.RegularExpressions;

namespace Comments.Test.Infrastructure
{
    public static class Scrubbers 
    {
        public static string ScrubHashFromJavascriptFileName(string str)
        {
            return Regex.Replace(str, "(src=\\\".*.)(.[a-z0-9]{8}.)(js\\\")", "$1.$3"); //unescaped regex is: src=\".*.([a-z0-9]{8}.)js
        }

        public static string ScrubLastModifiedDate(string str)
        {
            return Regex.Replace(str, "\\\"lastModifiedDate\\\":\\\"([0-9\\-TZ+:\\.]+)\\\"", "\"lastModifiedDate\":\"scrubbed by ScrubLastModifiedDate\""); //unescaped regex is: \"lastModifiedDate\":\"([0-9\-TZ:\.]+)\"
        }

        public static string ScrubStartDate(string str)
        {
	        return Regex.Replace(str, "\\\"StartDate\\\":\\\"([0-9\\-TZ+:\\.]+)\\\"", "\"StartDate\":\"scrubbed by ScrubStartDate\""); //unescaped regex is: \"StartDate\":\"([0-9\-TZ:\.]+)\"
		}

        public static string ScrubEndDate(string str)
        {
	        return Regex.Replace(str, "\\\"EndDate\\\":\\\"([0-9\\-TZ+:\\.]+)\\\"", "\"EndDate\":\"scrubbed by ScrubEndDate\""); //unescaped regex is: \"StartDate\":\"([0-9\-TZ:\.]+)\"
		}

		public static string ScrubCommentId(string str)
	    {
		    return Regex.Replace(str, @"""commentId"":(\d+)", @"""commentId"":""scrubbed by ScrubCommentId""");
	    }

	    public static string ScrubLocationId(string str)
	    {
		    return Regex.Replace(str, @"""locationId"":(\d+)", @"""commentId"":""scrubbed by ScrubLocationId""");
	    }

	    public static string ScrubAnswerId(string str)
	    {
		    return Regex.Replace(str, @"""answerId"":(\d+)", @"""answerId"":""scrubbed by ScrubAnswerId""");
	    }

	    public static string ScrubQuestionId(string str)
	    {
		    return Regex.Replace(str, @"""questionId"":(\d+)", @"""questionId"":""scrubbed by ScrubQuestionId""");
	    }

	    public static string ScrubQuestionTypeId(string str)
	    {
		    return Regex.Replace(str, @"""questionTypeId"":(\d+)", @"""questionTypeId"":""scrubbed by ScrubQuestionTypeId""");
	    }

	    public static string ScrubUserId(string str)
	    {
		    return Regex.Replace(str, @"""UserId"":""([a-z0-9A-Z-]+)""", @"""UserId"":""scrubbed by ScrubUserId""");
	    }

	    public static string ScrubCollationCode(string str)
	    {
		    return Regex.Replace(str, @"\d{4}\s\d{4}\s\d{4}", @"1234 1234 1234");
	    }

	    public static string ScrubOrganisationAuthorisationId(string str)
	    {
			return Regex.Replace(str, @"""organisationAuthorisationId"":(\d+)", @"""organisationAuthorisationId"":""scrubbed by ScrubUserId""");
		}

		public static string ScrubStackTraceString(string str)
		{
			//unescaped regex is: "StackTraceString":"[^"]+"
			return Regex.Replace(str, @"""StackTraceString"":""[^""]+""", @"""StackTraceString"":""scrubbed by ScrubStackTraceString""");
		}

		public static string ScrubIds(string str)
	    {
		    str = ScrubCommentId(str);
		    str = ScrubLocationId(str);
		    str = ScrubAnswerId(str);
		    str = ScrubQuestionId(str);
		    str = ScrubQuestionTypeId(str);
			return str;
	    }

		public static string ScrubErrorMessage(string str)
	    {
		    return Regex.Replace(str, "(<!--)([\\d\\D]+)(-->)", "\"Error Message\":\"scrubbed by ScrubErrorMessage\"");

		}


		
    }
}
