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

	    public static string ScrubErrorMessage(string str)
	    {
		    return Regex.Replace(str, "(<!--)([\\d\\D]+)(-->)", "\"Error Message\":\"scrubbed by ScrubErrorMessage\"");

		}
	}
}
